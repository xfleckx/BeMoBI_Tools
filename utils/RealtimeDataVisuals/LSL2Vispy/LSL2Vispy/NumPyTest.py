
import numpy as np


anEmptyArray = np.array(None)

print(anEmptyArray) # None

print(anEmptyArray.any())

an1DArray = np.array([1,2,3])

print(an1DArray.any())

print(an1DArray)

aListOfLists = [[1], [2], [3]]

anColumnArray = np.array(aListOfLists)

print(anColumnArray)

an2ColumnArray = np.c_[anColumnArray, anColumnArray]

print(an2ColumnArray)