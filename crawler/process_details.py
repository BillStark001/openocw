# -*- coding: utf-8 -*-
"""
Created on Wed Jun 16 15:20:34 2021

@author: zhaoj
"""

from sys_utils import *
from utils import *

from collections import defaultdict

details = pload(path.savepath_details_raw)
keys = pload(path.savepath_details_keys)
keys_rev = ltod(list_concat([[(vv, k) for vv in keys[k]] for k in keys]))

basic_info_parse = keys_general = {
 'lect_unit': lmb_identity,
 'lect_insts': lmb_identity,
 'components': lmb_identity, # TODO
 'time_place': lmb_identity,
 'class': lmb_identity,
 'code': lmb_identity,
 'credit': lambda x: int(x) if int(x) % 1 == 0 else float(x),
 'ac_year': parse_ay,
 'quarter': parse_q,
 'upd_syllabus': parse_date,
 'upd_lect_note': parse_date,
 'lang': parse_lang,
 'access_ranking': parse_ar
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