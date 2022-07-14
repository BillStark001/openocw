# -*- coding: utf-8 -*-
"""
Created on Sat Jun 12 03:15:49 2021

@author: billstark
"""

import json
import pickle
import os
import time
import shutil

from utils import *

class path:
    root_path = './operation/'
    cache_suffix = '.cache'
    backup_suffix = '.bak'
    savepath_unit_tree_raw = root_path + 'unit_tree_raw.pkl'
    savepath_unit_tree = root_path + 'unit_tree.pkl'
    savepath_course_list_raw = root_path + 'course_list_raw.pkl'
    savepath_course_list = root_path + 'course_list.pkl'
    savepath_details_raw = root_path + 'details_raw.pkl'
    savepath_details_keys = root_path + 'details_keys.pkl'
    data_path = './'
    school_dep_map_path = data_path + 'school2dep.json'

illegal_char = '%\/:*?"<>|'
legal_char = [illegal_char[0] + hex(ord(x)).split('x')[-1] for x in illegal_char]

def legalize_filename(ustr):
    for i in range(len(illegal_char)):
        ustr = ustr.replace(illegal_char[i], legal_char[i])
    return ustr

def restore_filename(ustr):
    ustr.replace(legal_char[-1], illegal_char[-1])
    for i in range(len(illegal_char[:-1])):
        ustr = ustr.replace(legal_char[i], illegal_char[i])
    return ustr
    
def jdump(obj, path, indent=2, ensure_ascii=False):
    with open(path, 'w', encoding='utf-8') as f:
        json.dump(obj, f, indent=indent, ensure_ascii=ensure_ascii)
        
def jload(path):
    with open(path, 'r', encoding='utf-8') as f:
        ans = json.load(f)
    return ans

def pdump(obj, path):
    with open(path, 'wb') as f:
        pickle.dump(obj, f)
        
def pload(path):
    with open(path, 'rb') as f:
        ans = pickle.load(f)
    return ans

backup_file = lambda f: shutil.copyfile(f, f+path.backup_suffix)
clear_backup = lambda f: os.remove(f+path.backup_suffix)

if __name__ == '__main__':
    pass