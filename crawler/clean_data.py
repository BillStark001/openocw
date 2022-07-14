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

cl1, cl2, _, _ = pload(path.savepath_course_list_raw)
dep1, dep2, adddddddddddddata = pload(path.savepath_unit_tree)

clist = defaultdict(list) # course code: details

# merge code
for dept in cl1:
    for course in dept:
        ccode = course[0]
        clist[ccode].append(course[1:])
        
# clean repeats
for code in clist:
    info = clist[code]
    key1 = lambda x: x[0][1]
    info.sort(key=key1)
    info = list_clean_repeats(info, key1)
    clist[code] = info
    
crs = clist['LAE.E213']
dcrs = {'LAE.E213': crs}

k_compl = 'completed'
k_compl_d = 'details_completed'
k_name = 'name_jp'
k_name_en = 'name_en'
k_code = 'code'
k_dept = 'lecturer_unit'
k_years = 'years'
k_creds = 'nr_credits'
k_classes = 'classes'

kc_id = 'class_id'
kc_sys = 'sys_info'
kc_lang = 'lang'
kc_ltime = 'term_and_time'
kc_tchr = 'lecturers'
kc_upd_syl = 'upd_syllabus'
kc_upd_note = 'upd_lect_note'
kc_upd = 'upd_local'

built_list = {}
ars = defaultdict(int)
for code in clist:
    
    # build structure
    cur_course = clist[code]
    classes = {}
    lyears = []
    names = [x[0][0] for x in clist[code]]
    common_name = find_common_word(names).strip(' ')
    
    
    # Clean repeated classes in the same year of a same course
    classes_raw = defaultdict(list)
    for instance in cur_course:
        cname = instance[0][0][len(find_common_prefix([common_name, instance[0][0]])):].strip(' ')
        instance[0] = instance[0][1]
        try:
            instance[2] = parse_ay_and_q(instance[2])
            instance[3] = parse_date(instance[3])
            instance[4] = parse_date(instance[4])
            instance[5] = parse_ar(instance[5])
        except Exception as e:
            print(instance)
            raise(e)
        classes_raw[cname].append(instance)
    cr_tmp = {}
    for cr in list(classes_raw.keys()):
        crcr = classes_raw[cr]
        cnts = defaultdict(list)
        cnts2 = defaultdict(int)
        for instance in crcr:
            cnts[instance[0][1]].append(instance)
            cnts2[instance[0][0] % 100000] += 1
        lyears = list(cnts.keys())
        if not any(len(x) > 1 for x in cnts.values()): 
            continue
        # multiple classes with the same name, division required
        del classes_raw[cr]
        for v in cnts.values(): v.sort(key=lambda x: cnts2[x[0][0] % 100000] * 1000000 + x[0][0] % 1000000)
        while any(len(x) != 0 for x in cnts.values()):
            new_instance = [cnts[y].pop() for y in cnts if cnts[y]]
            frn_cr = first_unrepeated_name(cr, classes_raw)
            #print(frn_cr)
            classes_raw[frn_cr] = new_instance
            
    # structurize
    for class_id in classes_raw:
        class_detail = classes_raw[class_id]
        sys_info = ltod([x[0][::-1] for x in class_detail])
        ltime = ltod([x[2] for x in class_detail])
        tchrs = ltod([[x[0][-1], x[1] if x[1] else []] for x in class_detail])
        # TODO is it okay to not differentiating the years?
        upd_syl = class_detail[-1][3]
        upd_note = class_detail[-1][4]
        class_detail_formed = {kc_id: class_id, kc_sys: sys_info, kc_lang: 'JP', kc_ltime: ltime, \
                        kc_tchr: tchrs, kc_upd_syl: upd_syl, kc_upd_note: upd_note, kc_upd: today}
        ars[code] = max(ars[code], sorted([x[5] for x in class_detail])[-1])
        classes[class_id] = class_detail_formed

    details = {k_compl: False, k_compl_d: False, k_code: code, \
               k_name: common_name, k_name_en: None, k_dept: None, k_years: lyears, k_creds: 0, k_classes: classes}

    built_list[code] = details
    
# TODO what about cl2?

ars = sorted(list(ars.items()), key = lambda x: -x[1])
pdump((built_list, ars), path.savepath_course_list)