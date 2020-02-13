#Remove .csv
import os
os.system('cmd /c "dir /b > filenames.txt"')
filenames=open("filenames.txt",'r')
for line in filenames:
	if line.count(".csv"):
		nline=line.replace("\n","")
		os.rename(nline,nline.replace(".csv",""))
