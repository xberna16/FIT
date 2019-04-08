import sys
import wave
import numpy as np
import matplotlib.pyplot as plt
soubor = None
if(len(sys.argv) == 1):
    soubor = "xberna16.wav"
else:
    soubor = sys.argv[1]
opened = wave.open(soubor,mode='r')
sample = opened.readframes(opened.getnframes())
sample = np.fromstring(sample, 'Int16')
opened.close()
min = len(sample)
help=0
sure =0
for i in range(len(sample)):
    if(sample[i]>0):
        help+=1
    else:
        if (help>0 and help<min):
            if help<sure:
                sure=help
                continue
            min=help
            help=0
out = np.zeros(int(len(sample)/min),'Int16')
F = open("bonusout.txt","w") 
for i in range(int(len(sample)/min)):
    if sample[(i*min)+int(min/2)] > 0:
        out[i]= 1
    print(out[i],file=F)
F.close()
plt.figure(1)
plt.stem(np.linspace(7,310,20),out[:20]*30000, "orange")
plt.plot(sample[:320])
plt.show()
