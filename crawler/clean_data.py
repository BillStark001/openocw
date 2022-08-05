# -*- coding: utf-8 -*-
"""
Created on Sat Jun 12 03:23:31 2021

@author: billstark
"""

from sys_utils import *
from utils import *
import re

from collections import defaultdict

today = (2021, 6, 14)
default_date = (1970, 1, 1)

# unit list

# course list

cl1, _, _, _ = pload(path.savepath_course_list_raw)
dep1, dep2, adddddddddddddata = pload(path.savepath_unit_tree)

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
    
    
def form_record(recs_raw, code, year=2022, lang='ja'):
    # scan common words
    names = [x[0][0] for x in recs_raw]
    common_name = find_common_word(names)
    
    # build basic record
    ans = {'__meta__': {}}
    ans_meta = ans['__meta__']
    ans_meta['complete'] = False
    ans['code'] = code
    ans['year'] = year
    ans['isLink'] = False
    ans['name'] = {lang: common_name}
    ans['credit'] = (0, 0, 0, 0)
    ans['classes'] = []
    
    # build full record
    for rec in recs_raw:
        cls = {'__meta__': {}}
        cls_meta = cls['__meta__']
        # determine class name
        cname = rec[0][0]
        cname = cname[len(find_common_prefix([cname, common_name])):].strip()        
        cls['name'] = cname
        cls_meta['ocwId'] = rec[0][1][0]
        cls_meta['complete'] = False
        cls['lects'] = [x[1] for x in rec[1]]
        
        ans['classes'].append(cls)
    
    return ans
    

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
    
pdump((built_list, lecturers), path.savepath_course_list)