# -*- coding: utf-8 -*-
"""
Created on Wed Jun 16 15:20:34 2021

@author: zhaoj
"""

from parse_utils import *
from sys_utils import *
from utils import *

from collections import defaultdict

from main import path
from form_utils import build_keys

if __name__ == '__main__':

  if True:
    build_keys(path.savepath_details_raw, path.savepath_details_keys)

  # dd = gather_data(path.savepath_details_raw)

  details, _ = pload(path.savepath_details_raw)
  keys = pload(path.savepath_details_keys)
  keys_rev = ltod(list_concat([[(vv, k) for vv in keys[k]] for k in keys]))

  info_parsers = {
      'unit': lmb_identity,
      'lects': lambda x: [y[0] for y in x],
      'form': parse_form,
      'media_use': lmb_identity, #* wtf???
      'addr': parse_addr, 
      'class': lmb_identity,
      'code': lmb_identity,
      'credit': lambda x: int(x),
      'year': lambda x: int(x.replace('年', '').replace('度', '').replace('AY', '').replace(' ', '')),
      'quarter': parse_quarter,
      'upd_syl': parse_date,
      'upd_lect': parse_date,
      'lang': parse_lang,
      'acrk': parse_acrk, 
  }

  def parse_details(dict_details, lang='JP'):
    ans = {}
    for k in dict_details:
      dk = dict_details[k]
      if not k in keys_rev:
        rk = 'other'
      else:
        rk = keys_rev[k]
      if rk == 'competencies':
        ansk = btoi(dk)
      elif rk == 'relcrs':
        ansk = [x[0] for x in dk]
      elif rk in ['textbooks', 'refmats']:
        ansk = [parse_book(x) for x in dk.split('\n')]
      elif rk == 'grading':
        # TODO
        ansk = dk
      elif rk == 'contact':
        # TODO
        ansk = parse_contacts(dk)
      elif rk == 'other':
        ansk = [dk]
      else:
        ansk = dk
        
      if rk == 'other' and rk in ans:
        ans[rk] += ansk
      else:
        ans[rk] = ansk
    return ans