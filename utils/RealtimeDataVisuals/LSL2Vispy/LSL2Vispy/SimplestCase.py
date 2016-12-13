# http://ipython-books.github.io/featured-06/

import numpy as np
from numpy import interp
from vispy import app
from vispy import gloo


import sys
sys.path.append('./pylsl') # help python find pylsl relative to this example program
from pylsl import StreamInfo, StreamInlet, resolve_streams

vertex = """
attribute vec2 a_position;
void main (void)
{
	gl_Position = vec4(a_position, 0.0, 1.0);
}
"""

fragment = """
void main()
{
	gl_FragColor = vec4(0.0, 0.0, 0.0, 1.0);
}
"""

c =  app.Canvas(keys='interactive')

program = gloo.Program(vertex, fragment)

#timer auto runs with 60Hz -> default monitor refresh rate - hardcodet!
timer = app.Timer('auto')

class Config(object):
	def __init__(self):
		#config values
		self.bgColor = (1,1,1,1)

		self._secondsToDisplay = 1
		self._dataRate = 60 
		self._useRandomSamples = False

	
	@property
	def UseRandomSamples(self):
		"""Samples per Second In Hertz"""
		return self._useRandomSamples

	@UseRandomSamples.setter
	def UseRandomSamples(self, value):
		self._useRandomSamples = value

	@property
	def DataRate(self):
		"""Samples per Second In Hertz"""
		return self._dataRate
		
	@DataRate.setter
	def DataRate(self, value):
		self._dataRate = value

	@property
	def SecondsToDisplay(self):
		"""The Time for which samples should be renderd """
		return self._secondsToDisplay
	
	@SecondsToDisplay.setter
	def SecondsToDisplay(self, value):
		self._secondsToDisplay = value

	@property
	def SamplesToDisplay(self):
		"""Read Only - The amount of samples actually displayed on the screen"""
		return self._secondsToDisplay * self._dataRate

# runtime state
class State(object):
	def __init__(self, *args, **kwargs):
		self.currentSampleOffset = 0 # index for the offset of the current samples within in the
							# interval which gets displayed (e.g. 60 samples per second for 1 second)
		self.expectedSamplesPerTick = 1 # e.g. data rate equals update rate
		self.sampleOffsetPerUpdate = 1 # e.g the update rate is equal to the data rate
		# sampleOffsetPerUpdate = 2 # e.g the update rate is half of the data rate

		self.tick = 0

		self.lowestAmplitude = 0
		self.highestAmplitude = 0


state = State()
config = Config()
config.DataRate = 100
config.SecondsToDisplay = 1

data =  np.c_[
		np.linspace(-1.0, +1.0, config.SamplesToDisplay, dtype=np.float32),
		np.random.uniform(-0.5, +0.5, config.SamplesToDisplay).astype(np.float32)]

program['a_position'] = data

@c.connect
def on_resize(event):
	gloo.set_viewport(0, 0, *event.size)

@c.connect
def on_draw(event):
	gloo.clear(config.bgColor)
	program.draw('line_strip')

@timer.connect
def on_timer(event):
	
	state.tick += 1 / config.DataRate
	state.tick = state.tick % config.DataRate
	
	state.lastSampleOffset = state.currentSampleOffset
	state.currentSampleOffset += state.expectedSamplesPerTick
	state.currentSampleOffset = state.currentSampleOffset % config.SamplesToDisplay
	sampleSet = []

	if config.UseRandomSamples:
		sampleSet = np.random.uniform(-0.5, +0.5, state.expectedSamplesPerTick).astype(np.float32)
		
	else:
		lslSampleSet = []
		sample,ts = inlet.pull_sample(0.0)
		while ts > 0.0:
			dataPoint = sample[0]

			if(dataPoint < state.lowestAmplitude):
				state.lowestAmplitude = dataPoint

			if(dataPoint > state.highestAmplitude):
					state.highestAmplitude = dataPoint

			remappedValue = interp(dataPoint, [state.lowestAmplitude, state.highestAmplitude], [-0.5, 0.5])
			lslSampleSet.append( remappedValue )# demo use only one channel
			
			sample,ts = inlet.pull_sample(0.0)

		sampleSet = lslSampleSet

	if any(sampleSet):
		data[state.lastSampleOffset:state.currentSampleOffset,1] = sampleSet
	

	program['a_position'].set_data(data)
	
	c.update()


streams = resolve_streams()

if not any(streams):
	print 'No Streams found'
	exit()

inlet = StreamInlet(streams[0])

c.show()
timer.start()
app.run()