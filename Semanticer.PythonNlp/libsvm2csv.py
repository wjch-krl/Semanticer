"""
convert libsvm file to csv'
libsvm2csv.py <input file> <output file> <X dimensionality>
"""

import sys
import csv

input_file = sys.argv[1]
output_file = sys.argv[2]

d = int( sys.argv[3] )
assert ( d > 0 )

reader = csv.reader( open( input_file ), delimiter = " " )
writer = csv.writer( open( output_file, 'wb' ))

writer.writerow(['label'] + (range(1 , d+1)))

for line in reader:
	label = line.pop( 0 )
	if line[-1].strip() == '':
		line.pop( -1 )
	line = map( lambda x: tuple( x.split( ":" )), line )
	new_line = [ label ] + [ 0 ] * d
	for i, v in line:
		i = int( i )
		if i <= d:
			new_line[i] = v
		
	writer.writerow( new_line )