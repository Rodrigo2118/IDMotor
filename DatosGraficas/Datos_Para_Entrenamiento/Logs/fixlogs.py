import sys
doc=open(sys.argv[1],'r')
sdoc=doc.read().split('\n')
for filename in sdoc:
	file=open(filename,'r')
	#nfile=open(r"gnm_"+sys.argv[1],"w")
	string=file.read().split('\n')
	nstring=""

	first = True

	for row in string:
		nrow=row[:23]+','+row[23:]
		if not first:
			nstring=nstring+"\n"+nrow
		else:
			nstring=nstring+nrow
			first=False
	file.close()
	file=open(filename,'w')
	file.write(nstring)		
	file.close()
doc.close()

		
