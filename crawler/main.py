# -*- coding: utf-8 -*-
"""
Created on Thu Aug  5 12:38:13 2021

@author: zhaoj
"""

# 他の.pyファイルの注釈に参照させたい

import spider

print('\nSTEP 0\n')

spider.step0()
import build_tree

print('\nSTEP 1\n')

# spider.step1(start_year=2018, start_year_old=2018)
spider.step1_new()
import clean_data

print('\nSTEP 2\n')

spider.step2()