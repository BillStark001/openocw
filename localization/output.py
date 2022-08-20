import json
import pandas as pd
import numpy as np
import os
import sys
from collections import defaultdict

if __name__ == '__main__':
  if len(sys.argv) < 2:
    sys.stderr.write("Output directory not assigned.")
    sys.exit(1)
  
  try:
    file = sys.argv[1]
    try:
      os.makedirs(file)
    except: pass
    dir = os.path.dirname(sys.argv[0])
    d = defaultdict(dict)
    for cat in [x for x in os.listdir(dir) if x.endswith('.csv')]:
      f = pd.read_csv(dir + '/' + cat, dtype=str)
      f.fillna('')
      cols = [c for c in f if c != 'key']
      for i, r in f.iterrows():
        if isinstance(r['key'], float) and np.isnan(r['key']):
          continue
        k = str(r['key']).replace(' ', '')
        if k == '':
          continue
        for col in cols:
          if not isinstance(r[col], float) or not np.isnan(r[col]):
            d[col][k] = str(r[col])
          
          
    for lang in d:
      json.dump(d[lang], open(file + '/' + lang + '.json', 'w'))
    
    sys.exit(0)
  except Exception as e:
    sys.stderr.write(str(e))
    sys.exit(2)
