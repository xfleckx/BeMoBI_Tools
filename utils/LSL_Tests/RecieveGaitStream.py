"""Example program to show how to read a marker time series from LSL."""
import sys
sys.path.append('./pylsl') # help python find pylsl relative to this example program
from pylsl import StreamInlet, resolve_stream

# first resolve an EEG stream on the lab network
print("looking for an Gait stream...")
streams = resolve_stream('type', 'Force')

streamsFound = len(streams)

if (streamsFound > 0):
	print 'found ' + str(streamsFound)
else:
	print 'found none'

# create a new inlet to read from the stream
inlet = StreamInlet(streams[0])

while True:
	
	sample, timestamp = inlet.pull_sample()

	if(sample):
		print(str(timestamp) + ' Gait Force:' + str(sample[0]))