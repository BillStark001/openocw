# -*- coding: utf-8 -*-
"""
Created on Wed Jun 16 15:20:34 2021

@author: billstark001
"""

from sys_utils import *
from utils import *

from collections import defaultdict

details = pload(path.savepath_details_raw)

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
    
#pdump(kkk, path.savepath_details_keys)