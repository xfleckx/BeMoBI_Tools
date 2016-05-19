import os
import pandas as pd
import seaborn as sns

dataDir = '..\\Test_Data\\'
pilotMarkerDataFile = 'Pilot.csv'

df = pd.read_csv( dataDir + '\\' + pilotMarkerDataFile,sep='\t', engine='python') 

repr(df.head())


# TODO times per position

# plotting a heatmap http://stanford.edu/~mwaskom/software/seaborn/examples/many_pairwise_correlations.html
## Generate a custom diverging colormap
#cmap = sns.diverging_palette(220, 10, as_cmap=True)

# Draw the heatmap with the mask and correct aspect ratio
#sns.heatmap(timesAtPositions, mask=mask, cmap=cmap, vmax=.3,
#            square=True, xticklabels=5, yticklabels=5,
#            linewidths=.5, cbar_kws={"shrink": .5}, ax=ax)