# -*- coding: utf-8 -*-
"""
Created on Sun Jun 13 00:02:27 2021

@author: billstark
"""

from sys_utils import *
from utils import *
import re
from collections import defaultdict

def build_tree(inpath: str, outpath: str, dep_path: str):

  # 開講元のID
  sd_num_map = jload(dep_path)
  sd_num_map_ja = dict([[y['nameJa'], y['id']] for y in list_concat([x['department'] for x in sd_num_map])])
  sd_num_map_en = dict([[y['nameEn'], y['id']] for y in list_concat([x['department'] for x in sd_num_map])])
  # オリジナルなリスト
  d, e = pload(inpath)
  jt, jl = d
  et, el = e

  # 異なるurlの共通的なquery parametersを得る
  def max_common_dict(l):
    if len(l) == 0: return {}
    ans = dict(l[0])
    for d in l[1:]:
      for k in list(ans.keys()):
        if not k in d or ans[k] != d[k]:
          del ans[k]
    return ans

  # リストはツリー状なものである
  # 辞書型のリストの要素をノードに変える
  def reform(n):
    if len(n) == 0: return node()
    if not isinstance(first_elem(n), dict):
      return node(info=dict(n))
    ans_k = []
    ans_d = []
    for k in n:
      r = reform(n[k])
      ans_k.append((k, r))
      ans_d.append(r.info)
    return node(info=max_common_dict(ans_d), children=ltod(ans_k))

  # OCWには正くないパラメータを含むリンクがある
  # これを直す
  def fix_subaction(n):
    if (len(n.info) == 1 and 'GakubuCD' in n.info) or \
      (len(n.info) == 2 and 'GakubuCD' in n.info and 'SubAction' in n.info):
        #print(n.info)
        if n.info['GakubuCD'] in [1, 2, 3, 4, 5, 6]:
          n.info['SubAction'] = 'T0210'
        elif n.info['GakubuCD'] in [7]:
          n.info['SubAction'] = 'T0200'
        #print(n.info)
    
    for nn in n.children.values():
      fix_subaction(nn)
      
  jtt = [reform(jt[i]) for i in range(len(jt))]
  fix_subaction(jtt[0])
  ett = [reform(et[i]) for i in range(len(et))]
  fix_subaction(ett[0])
    

  # この前作ったノードツリーを再びリストに変える

  # あるノートに対してそのアドレスを表示する関数
  c = lambda x: [[x[0] + '.' + y[0], y[1]] for y in x[1].children.items()]
  # ソートのための比較関数
  d = lambda x: (str(x[1]), x[0])
  d2 = lambda x: str(x[1])

  # 新課程
  used_dicts = [(x[0], x[1].info) for x in iter_tree([['学部', jtt[0]], ['大学院', jtt[1]], ['Undergraduate', ett[0]], ['Graduate', ett[1]]], children=c, pass_queue=True)]
  used_dicts.sort(key=d, reverse=True)
  used_dicts = list_clean_repeats(used_dicts, d2)
  used_dicts.sort(key=lambda x: (sd_num_map_ja[x[0]] if x[0] in sd_num_map_ja else 114514, x[0], str(x[1])))

  # 旧課程
  used_dicts_old = [(x[0], x[1].info) for x in iter_tree([['学部', jtt[2]], ['大学院', jtt[3]], ['Undergraduate', ett[2]], ['Graduate', ett[3]]], children=c, pass_queue=True)]
  used_dicts_old.sort(key=d, reverse=True)
  used_dicts_old = list_clean_repeats(used_dicts_old, d2)
  used_dicts.sort(key=lambda x: (x[0], str(x[1])))

  '''
  jl = ltod(list_concat([[[x[0], x[1].info] for x in iter_tree(['ルート', y], children=c)] for y in jtt]), ignore_repeat=False)
  el = ltod(list_concat([[[x[0], x[1].info] for x in iter_tree(['root', y], children=c)] for y in ett]), ignore_repeat=False)

  sdmap = ltod([[str(x.info), x.info] for x in iter_tree(jtt[0])])
  djmap = ltod(list_concat([[[str(x[1].info), x[0]] for x in iter_tree(['ルート', y], children=lambda x: x[1].children.items())] for y in jtt]))
  demap = ltod(list_concat([[[str(x[1].info), x[0]] for x in iter_tree(['root', y], children=lambda x: x[1].children.items())] for y in ett]))
  jemap = [[djmap[k], (demap[k] if k in demap else '')] for k in djmap] + [[(djmap[k] if k in djmap else ''), demap[k]] for k in demap]
  ejmap = [[x[1], x[0]] for x in jemap]
  jemap = ltod(jemap, ignore_repeat=False)
  ejmap = ltod(ejmap, ignore_repeat=False)

  jk = [djmap[str(x[1])] for x in used_dicts]
  ek = [demap[str(x[1])] for x in used_dicts]
  '''

  # 出力のために辞書型に変える
  jtt = [x.__dict__() for x in jtt]
  ett = [x.__dict__() for x in ett]
  # 出力
  additional = (jtt, ett) #, jl, el, jk, ek, djmap, demap, jemap, ejmap)
  pdump((used_dicts, used_dicts_old, additional), outpath)
  

def build_keys(inpath: str, outpath: str):
  
  details = pload(inpath) # details_raw

  keys_jp = defaultdict(list)
  keys_en = defaultdict(list)

  for code in details:
    for det in details[code][0]:
      for k in det[2]:
        keys_jp[k].append(det[2][k])
    for det in details[code][1]:
      for k in det[2]:
        keys_en[k].append(det[2][k])
        
        
  kj = keys_jp.keys()
  ke = keys_en.keys()
  kj = list(kj)
  ke = list(ke)
  del ke[16]
  keys_general = ['desc', 'outcomes', 'keywords', 'competencies', 'flow', 'schedule', 'ooctime', 'textbooks', 'refmats', 'grading', 'relcrs', 'prereqs', 'other', 'contact', 'offhrs', 'exp_inst_courses', 'intro_avs']
  kkk = {}

  for i in range(len(keys_general)):
    kkk[keys_general[i]] = [kj[i], ke[i]]
    
  keys_jp = defaultdict(list)
  keys_en = defaultdict(list)

  for code in details:
    for det in details[code][0]:
      for k in det[1]:
        keys_jp[k].append(det[1][k])
    for det in details[code][1]:
      for k in det[1]:
        keys_en[k].append(det[1][k])
        
        
  kj = keys_jp.keys()
  ke = keys_en.keys()
  kj = list(kj)
  ke = list(ke)
  #del ke[16]
  keys_general = [
  'lect_unit',
  'lect_insts',
  'components',
  'time_place',
  'class',
  'code',
  'credit',
  'ac_year',
  'quarter',
  'upd_syllabus',
  'upd_lect_note',
  'lang',
  'access_ranking'
  ]

  for i in range(len(keys_general)):
    kkk[keys_general[i]] = [kj[i], ke[i]]
    
  pdump(kkk, outpath) # details_keys
  
  
def form_record(recs_raw, code, year=2022, lang='ja'):
  # scan common words
  names = [x[0][0] for x in recs_raw]
  common_name = find_common_word(names)
  
  # build basic record
  ans = {'__meta__': {}}
  ans_meta = ans['__meta__']
  ans_meta['complete'] = False
  ans_meta['acrk'] = 0. # access_ranking
  ans_meta['upd'] = 0
  
  ans['code'] = code
  ans['year'] = year
  ans['isLink'] = False
  ans['name'] = {lang: common_name}
  ans['credit'] = (0, 0, 0, 0)
  ans['unit'] = -1
  ans['classes'] = []
  
  # build full record
  for rec in recs_raw:
    
    cls = {'__meta__': {}}
    cls_meta = cls['__meta__']
    # determine class name
    cname = rec[0][0]
    cname = cname[len(find_common_prefix([cname, common_name])):].strip()    
    cls_meta['ocwId'] = rec[0][1][0]
    cls_meta['complete'] = False
    cls_meta['upd'] = 0
    cls_meta['upd_lect'] = 0
    cls_meta['upd_syl'] = 0
    
    cls['name'] = cname
    cls['lects'] = [x[1] for x in rec[1]]
    cls['form'] = 0
    cls['quarter'] = 0
    cls['addr'] = []
    cls['lang'] = []
    
    cls['syllabus'] = ''
    cls['notes'] = {}
    
  ans['classes'].append(cls)
  
  return ans
  
  
def form_records(inpath: str, outpath: str):

  cl1, _, _, _ = pload(inpath)

  clist = defaultdict(list) # course code: details

  # merge code
  for course in cl1:
    ccode = course[0]
    clist[ccode].append(course[1:])
      
  # clean repeats
  for code in clist:
    info = clist[code]
    key1 = lambda x: x[0][1]
    info.sort(key=key1)
    info = list_clean_repeats(info, key1)
    clist[code] = info
    

  built_list = {}
  lecturers = {}

  for code in clist:
    cur_course = clist[code]
    
    # add lecturer info
    for rec in cur_course:
      lects = rec[1]
      for lname, lid in lects:
        if lid not in lecturers:
          lecturers[lid] = {'ja': lname}
    
    # form record
    course_rec = form_record(cur_course, code, 2022, 'ja')
    built_list[code] = course_rec
    
  pdump((built_list, lecturers), outpath)
  

if __name__ == '__main__':
  pass