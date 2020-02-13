import csv
import sys
import random

p0=0.5

file=open(sys.argv[1],'r')
#csv_reader=csv.reader(file, delimiter=',',lineterminator='\n')
nfile=open(r"tj_"+sys.argv[1],"w")
#csv_writer=csv.writer(nfile,delimiter=',',lineterminator='\n')
string=file.read().split('\n')
#print(string)
first = True
for row in string: 
	r=random.random()-p0
	if r>0:
		if not first:
			nfile.write("\n"+row)
		else:
			nfile.write(row)
			first=False
		
file.close();
nfile.close();

