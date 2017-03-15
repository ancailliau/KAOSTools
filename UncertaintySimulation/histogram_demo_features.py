"""
Demo of the histogram (hist) function with a few features.

In addition to the basic histogram, this demo shows a few optional features:

    * Setting the number of data bins
    * The ``normed`` flag, which normalizes bin heights so that the integral of
      the histogram is 1. The resulting histogram is a probability density.
    * Setting the face color of the bars
    * Setting the opacity (alpha value).

"""
import numpy as np
import matplotlib.mlab as mlab
import matplotlib.pyplot as plt
from matplotlib import gridspec
import csv

data = [[], [], [], [], []]

with open('values.csv', 'rb') as f:
    reader = csv.reader(f)
    reader.next()
    for row in reader:
        data[0].append(float(row[0]))
        data[1].append(float(row[1]))
        data[2].append(float(row[2]))
        data[3].append(float(row[3]))
        data[4].append(float(row[4]))
        

plt.figure(1)

# amb_broken,amb_in_jam,amb_not_in_familiar_area,gps_broken,amb_on_scene_when_inc_reported
mapping = { "amb_broken" : 0, "amb_in_jam" : 1,"amb_not_in_familiar_area": 2,"gps_broken": 3,"amb_on_scene_when_inc_reported": 4}

num_bins = 10
# plt.subplot(321)
ax = plt.subplot2grid((3, 2), (0, 0))
n, bins, patches = plt.hist(data[mapping["amb_in_jam"]], num_bins, normed=1, facecolor='green', alpha=0.5)
plt.xlabel('Likelihood of occurence')
plt.title(r'AmbulanceStuckInTrafficJam')
plt.xlim(0, .05) 
frame = plt.gca()
frame.axes.get_yaxis().set_visible(False)
frame.axes.get_xaxis().set_ticks([0.01, 0.02, 0.04])

num_bins = 10
# plt.subplot(322)
ax = plt.subplot2grid((3, 2), (0, 1))
n, bins, patches = plt.hist(data[mapping["amb_not_in_familiar_area"]], num_bins, normed=1, facecolor='green', alpha=0.5)
plt.xlabel('Likelihood of occurence')
plt.title(r'AmbulanceNotInFamiliarArea')
plt.xlim(0,.4) 
frame = plt.gca()
frame.axes.get_yaxis().set_visible(False)
frame.axes.get_xaxis().set_ticks([0.1, 0.3])

num_bins = 14
# plt.subplot(323)
ax = plt.subplot2grid((3, 2), (1, 0))
n, bins, patches = plt.hist(data[mapping["gps_broken"]], num_bins, normed=1, facecolor='green', alpha=0.5)
plt.xlabel('Likelihood of occurence')
plt.title(r'GPSNotWorking')
plt.xlim(0,1) 
frame = plt.gca()
frame.axes.get_yaxis().set_visible(False)
frame.axes.get_xaxis().set_ticks([.1])

num_bins = 10
# plt.subplot(324)
ax = plt.subplot2grid((3, 2), (1, 1))
n, bins, patches = plt.hist(data[mapping["amb_broken"]], num_bins, normed=1, facecolor='green', alpha=0.5)
plt.xlabel('Likelihood of occurence')
plt.title(r'AmbulanceBrokenDown')
plt.xlim(0,.015) 
frame = plt.gca()
frame.axes.get_yaxis().set_visible(False)
frame.axes.get_xaxis().set_ticks([.001, .005, .01])

num_bins = 10
# plt.subplot(325)
ax = plt.subplot2grid((3, 2), (2, 0), colspan=2)
n, bins, patches = plt.hist(data[mapping["amb_on_scene_when_inc_reported"]], num_bins, normed=1, facecolor='red', alpha=0.5)
plt.xlabel('Probability of satisfaction')
plt.title(r'Achieve [Ambulance On Scene When Incident Reported]')
plt.xlim(0,1) 
frame = plt.gca()
frame.axes.get_yaxis().set_visible(False)
# frame.axes.get_xaxis().set_ticks([.001, .005, .01])


plt.subplots_adjust(wspace=.2, hspace=.8)

plt.show()
