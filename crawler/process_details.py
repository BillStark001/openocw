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

syl_parsers = {
  KEY_SYL_DESC: lmb_identity, 
  KEY_SYL_OUTCOMES: lmb_identity,
  KEY_SYL_KEYWORDS: lmb_identity,
  KEY_SYL_COMPETENCIES: bool2bin,
  KEY_SYL_FLOW: lmb_identity,
  KEY_SYL_SCHEDULE: lmb_identity,
  KEY_SYL_TEXTBOOKS: lmb_identity,
  KEY_SYL_REF_MATS: lmb_identity,
  KEY_SYL_GRADING: lmb_identity,
  KEY_SYL_REL_COURSES: lmb_identity,
  KEY_SYL_PREREQUESITES: lmb_identity,
  KEY_SYL_OOC_TIME: lmb_identity,
  KEY_SYL_OTHER: lmb_identity,
  KEY_SYL_CONTACT: lmb_identity,
  KEY_SYL_EXP_INST: lmb_identity,
  KEY_SYL_OFF_HRS: lmb_identity, 
}
# TODO parse lecturing unit!

def multilingual_key(k: str) -> bool:
  if k in [KEY_NAME, KEY_UNIT]:
    return True
  return False

def ml_key_syl(k: str) -> bool:
  if k in [KEY_SYL_COMPETENCIES]:
    return False
  return True

def build_db_scheme(
  raw_record: dict[str, tuple[list, dict, dict, dict]], 
  keys: dict[str, dict[str, str]], 
  ocw_id: int = 197000000
  ):
  '''
  raw_record: dict[lang: str, rec: raw_scheme]
  keys: dict[k: str, dict[lang: str, v: str]]
  '''
  info = form_basic_record_scheme()
  cls = form_class_record_scheme()
  syl = {
    KEY_SYL_OTHER: {
      
    }
  }
  notes = {}
  cls[KEY_SYLLABUS][KEY_VERSION] = VAL_VER_RAW
  cls[KEY_SYLLABUS][VAL_VER_RAW] = syl
  cls[KEY_NOTES][KEY_VERSION] = VAL_VER_RAW
  cls[KEY_NOTES][VAL_VER_RAW] = notes
  cls[KEY_META][KEY_OCW_ID] = ocw_id
  
  syl_parsed = False
  
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
          
    # add notes
    notes[lang] = rnts['supp']
    
    # add syllabus
    if not rsyl:
      continue
    syl_parsed = True
    for k in syl_parsers:
      kl = keys[k][lang] if k in keys and lang in keys[k] else None
      if kl == None or k == KEY_SYL_OTHER:
        syl[KEY_SYL_OTHER][f'{kl}_{lang}'] = k
        continue
      if not k in syl:
        if ml_key_syl(k):
          syl[k] = {}
      v = syl_parsers[k](rsyl[kl]) if kl in rsyl else None
      if ml_key_syl(k):
        syl[k][lang] = v
      else:
        syl[k] = v
        
    if not syl_parsed:
      cls[KEY_SYLLABUS][KEY_VERSION] = VAL_VER_NONE
      del cls[KEY_SYLLABUS][VAL_VER_RAW]
    
  
  return info, cls
  
def build_db_schemes(id_seed: int, data: dict, keys: dict):
  info = {KEY_CLASSES: []}
  cls = []
  nyear = 1970
  
  for year in data:
    oid = id_seed % 100000 + year * 100000
    ydata = {}
    if data[year][0] is not None:
      ydata['ja'] = data[year][0]
    if data[year][1] is not None:
      ydata['en'] = data[year][1]
    if not ydata:
      continue
    yinfo, ycls = build_db_scheme(ydata, keys, ocw_id=oid)
    if year > nyear:
      nyear = year
      for k in yinfo:
        if k != KEY_CLASSES:
          info[k] = yinfo[k]
    cls.append(ycls)
    info[KEY_CLASSES].append(oid)
    
  return info, cls


if __name__ == '__main__':

  if False:
    build_keys(path.savepath_details_raw, path.savepath_details_keys)

  dd, dt, du = gather_data(path.savepath_details_raw)

  details, _ = pload(path.savepath_details_raw)
  keys = pload(path.savepath_details_keys)
  keys_rev = ltod(list_concat([[(vv, k) for vv in keys[k]] for k in keys]))

  
