# -*- coding: utf-8 -*-
"""
Created on Wed Jun 16 15:20:34 2021

@author: zhaoj
"""

from db_definitions import *
from parse_utils import *
from sys_utils import *
from utils import *

from collections import defaultdict

from main import path
from form_utils import build_keys

info_parsers = {
    KEY_UNIT: lmb_identity,
    KEY_LECTURERS: lambda x: [y[0] for y in x],
    KEY_FORMAT: parse_form,
    KEY_MEDIA_USE: lmb_identity,  # * wtf???
    KEY_ADDRESS: parse_addr,
    KEY_CLASS_NAME: lambda x: '' if x.strip() == '-' else x.strip(),
    KEY_CODE: lmb_identity,
    KEY_CREDIT: lambda x: (0, 0, 0, int(x)),
    KEY_YEAR: lambda x: int(x.replace('年', '').replace('度', '').replace('AY', '').replace(' ', '')),
    KEY_QUARTER: parse_quarter,
    KEY_UPD_TIME_SYL: parse_date,
    KEY_UPD_TIME_NOTES: parse_date,
    KEY_LANGUAGE: parse_lang,
    KEY_ACCESS_RANK: parse_acrk,
}
# TODO parse lecturing unit!

def multilingual_key(k: str) -> bool:
  if k in [KEY_NAME, KEY_UNIT]:
    return True
  return False

def build_db_scheme(
  raw_record: dict[str, tuple[list, dict, dict, dict]], 
  keys: dict[str, dict[str, str]]
  ):
  '''
  raw_record: dict[lang: str, rec: raw_scheme]
  keys: dict[k: str, dict[lang: str, v: str]]
  '''
  info = form_basic_record_scheme()
  cls = form_class_record_scheme()
  
  for lang in raw_record:
    if raw_record == None:
      continue
    rname, rinfo, rsyl, rnts = raw_record[lang]
    
    # parse info
    
    pinfo = {}
    for k in info_parsers:
      kl = keys[k][lang] if k in keys and lang in keys[k] else None
      if kl == None or not kl in rinfo:
        continue
      v = rinfo[kl]
      pinfo[k] = info_parsers[k](v)
    
    rns: str = rname[1].strip()
    cnm = pinfo[KEY_CLASS_NAME] if KEY_CLASS_NAME in pinfo else ''
    pinfo[KEY_NAME] = rns[1][:-len(cnm)] if len(cnm) > 0 and rns.endswith(cnm) else rns
    
    for ik in info:
      if ik in pinfo:
        if multilingual_key(ik):
          info[ik][lang] = pinfo[ik]
        else:
          # sanity check needed?
          info[ik] = pinfo[ik]
          
    for ik in cls:
      if ik in pinfo:
        if multilingual_key(ik):
          cls[ik][lang] = pinfo[ik]
        else:
          # sanity check needed?
          cls[ik] = pinfo[ik]
          
    for ik in cls[KEY_META]:
      if ik in pinfo:
        cls[KEY_META][ik] = pinfo[ik]
          
    # TODO add syllabus etc.
  
  return info, cls
  


if __name__ == '__main__':

  if True:
    build_keys(path.savepath_details_raw, path.savepath_details_keys)

  dd, dt, du = gather_data(path.savepath_details_raw)

  details, _ = pload(path.savepath_details_raw)
  keys = pload(path.savepath_details_keys)
  keys_rev = ltod(list_concat([[(vv, k) for vv in keys[k]] for k in keys]))

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
