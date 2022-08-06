# -*- coding: utf-8 -*-
"""
Created on Thu Aug  5 12:38:13 2021

@author: zhaoj
"""

# 他の.pyファイルの注釈に参照させたい

import crawler
import form_utils

class path:
    root_path = './operation/'
    cache_suffix = '.cache'
    
    savepath_unit_tree_raw = root_path + 'unit_tree_raw.pkl'
    savepath_unit_tree = root_path + 'unit_tree.pkl'
    savepath_course_list_raw = root_path + 'course_list_raw.pkl'
    savepath_course_list = root_path + 'course_list.pkl'
    savepath_details_raw = root_path + 'details_raw.pkl'
    savepath_details_keys = root_path + 'details_keys.pkl'
    data_path = './'
    school_dep_map_path = data_path + 'school2dep.json'

print('\nSTEP 0\n')
'''
crawler.task1(path.savepath_unit_tree_raw)

form_utils.build_tree(
  path.savepath_unit_tree_raw, 
  path.savepath_unit_tree, 
  path.school_dep_map_path)'''

print('\nSTEP 1\n')

crawler.task2(path.savepath_course_list_raw)

print('\nSTEP 2\n')

crawler.task3(
  path.savepath_course_list_raw, 
  path.savepath_details_raw, 
  start_year=2020, 
  end_year=2022)