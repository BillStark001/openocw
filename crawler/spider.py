# -*- coding: utf-8 -*-
"""
Created on Wed Feb 19 17:16:23 2020

@author: billstark
"""

from sys_utils import *
from utils import *
import extractor

from urllib.parse import quote, unquote
from urllib.request import urlopen
import requests
from selenium import webdriver
from bs4 import BeautifulSoup

import os
import time
import numpy as np
from collections import defaultdict
from tqdm import tqdm
from heapq import heapify, heappush, heappop


addr_default = 'http://www.ocw.titech.ac.jp/index.php'

# Network related
'''
このスクレーパーはSeleniumを使う。
原則としてはrequestsやurllibなども使えるが、
OCWの一部のページではcookiesとjsを使ってユーザーを特定し、ページを更新するのである。
そのため、動的にページを得なければならない。故にseleniumを使う。
Seleniumを使うためにはブラウザのドライバーが必要である。
具体的にはhttps://www.selenium.dev/selenium/docs/api/py/api.htmlに参照して欲しい。
'''
mem_reset_limit = 300
try:
  assert driver
except:
  driver = None
  mem_reset_count = 0
  
driver_path = './chromedriver104.exe'

def init_driver(path='chromedriver.exe'):
  global driver
  options = webdriver.ChromeOptions()
  #options.add_argument('user-agent="Mozilla/5.0 (Linux; Android 4.0.4; Galaxy Nexus Build/IMM76B) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.133 Mobile Safari/535.19"')
  options.add_argument('user_agent="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.101 Safari/537.36"')
  options.add_argument('-Referer="http://www.ocw.titech.ac.jp/"')
  driver = webdriver.Chrome(path, options=options)

def get_html(*args, **kwargs):
  global mem_reset_count
  if mem_reset_count >= mem_reset_limit:
    mem_reset_count = 0
    driver.quit()
    init_driver(driver_path)
  driver.get(*args, **kwargs)
  html = driver.page_source
  mem_reset_count += 1
  return html

def get_html_after_loaded(*args, **kwargs):
  timeout = 60
  global cookie
  html = get_html(*args, **kwargs)
  full_loaded = -1
  time_start = time.time()
  dtime = time.time() - time_start
  while full_loaded < 0:
    html = driver.page_source
    full_loaded = min(html.find('left-menu'), html.find('right-contents'))
    dtime = time.time() - time_start
    if timeout < dtime:
      print('timeout')
      return 'timeout'
    if html.find('HTTP 404') >= 0 or html.find('404 NOT FOUND') >= 0 or html.find('お探しのコンテンツが見つかりませんでした') >= 0:
      return '404'
    elif html.find('top-mein-navi') >= 0:
      return 'toppage'
  return html

# args

this_year = 2022
innovation_year = 2016

args_lang = lambda lang='JA': {'lang': lang}
args_year = lambda year=this_year: {'Nendo': year}
args_term = lambda zenki=True: {'Gakki': (1 if zenki else 2) if isinstance(zenki, bool) else zenki}
args_lnote = lambda lnote=True: {'vid': '05' if lnote else '03'}

args_general_list = {'module': 'General', 'tab': 2}
args_general_course = {'module': 'General', 'action': 'T0300'}

args_archive_list = {'module': 'Archive', 'action': 'ArchiveIndex', 'tab': 2, 'vid': 4}
args_archive_course = {'module': 'Archive', 'action': 'ArchiveIndex', 'SubAction': 'T0300'}
args_archive_course_old = {'module': 'Archive', 'action': 'KougiInfo', 'GakubuCD': 100, 'GakkaCD': 12}

# funcs
'''
インターネットと交互し、htmlを操り、データを導き出す部分である。
それぞれの関数の意味は名前の通りである。
'''
def get_department_list(year=2020, lang='JA'):
  url = 'http://www.ocw.titech.ac.jp/index.php?module=Archive&action=ArchiveIndex&GakubuCD=1&GakkaCD=311100&KeiCD=11&tab=2&focus=200&lang={}&Nendo={}&SubAction=T0200'.format(lang, year)
  html = get_html_after_loaded(url)
  cats, cats_flat, excluded = extractor.get_department_list(html)
  print(excluded)
  return cats, cats_flat

def get_course_list(info, year=this_year, retry_limit=5):
  args_course = args_general_list if year >= this_year else args_archive_list
  if year >= this_year:
    if 'SubAction' in info:
      info = dict(info)
      info['action'] = info['SubAction']
      del info['SubAction']
  url = form_url(__url__=addr_default, **args_course, **args_year(year), **info)
  #print(url)
  
  tbls_ans = []
  for _ in range(retry_limit):
    html = get_html_after_loaded(url)
    tbls_ans = extractor.get_course_list(html)
    if len(tbls_ans) > 0 and tbls_ans[0][3].startswith(str(year)):
      break
  
  return tbls_ans
  
def fetch_lecture_info(lecture_info, **kwargs):
  '''
  Get the html return according to lecture id (and other information).
  lecture_info: id or (id, year) or (id, year, term)
  '''
  if isinstance(lecture_info, int) or isinstance(lecture_info, str):
    lecture_id = lecture_info
    year = int(str(lecture_id)[:4])
  elif isinstance(lecture_info, tuple) or isinstance(lecture_info, list):
    lecture_id = lecture_info[0]
    if len(lecture_info) > 1: year = lecture_info[1]
    if len(lecture_info) > 2: term = lecture_info[2]
  elif isinstance(lecture_info, dict):
    lecture_id = lecture_info['KougiCD']
    year = lecture_info['Nendo'] if 'Nendo' in lecture_info else \
      (int(str(lecture_id)[:4]) if int(lecture_id) > 10000000 else innovation_year - 1)
    term = lecture_info['Gakki'] if 'Gakki' in lecture_info else False
    
  if year >= innovation_year:
    main_args = args_general_course
    info_args = {'KougiCD': lecture_id, 'Nendo': year}
  else:
    main_args = args_archive_course_old
    info_args = {'KougiCD': lecture_id, 'Nendo': year, 'Gakki': args_term(term)['Gakki']}
  
  # request
  
  url1 = form_url(__url__=addr_default, **main_args, **info_args, **kwargs, **args_lnote(False))
  url2 = form_url(__url__=addr_default, **main_args, **info_args, **kwargs, **args_lnote(True))
  
  #print(url1)
  #print(url2)

  html = get_html_after_loaded(url1)
  bf_base_page = BeautifulSoup(html, 'lxml')
  cnt_lname = bf_base_page.find_all(class_='page-title-area clearfix')[0]
  cnt_summ = bf_base_page.find_all(class_='gaiyo-data')[0]
  cnt_inner = bf_base_page.find_all(class_='right-inner')[0]
  
  html = get_html_after_loaded(url2)
  bf_base_page = BeautifulSoup(html, 'lxml')
  try:
    cnt_note = bf_base_page.find_all(class_='right-inner')[0]
  except IndexError as e:
    cnt_note = bf_base_page.find_all(id='right-inner')[0]
  
  ans = (cnt_lname, cnt_summ, cnt_inner, cnt_note)
  return ans
  
def fetch_lecture_info_2(id_seed, year, en=False):
  '''
  Get the html return according to lecture id (and other information).
  lecture_info: id or (id, year) or (id, year, term)
  '''
  # request
  url0 = 'http://www.ocw.titech.ac.jp/index.php?module=General&action=T0300&JWC={year}{id}&lang={lang}'
  lang = 'JA' if not en else 'EN'
  url1 = url0.format(year=year, id=id_seed[4:], lang=lang)
  url2 = url1 + '&vid=05'

  html = get_html_after_loaded(url1)
  if html == '404':
    return None
  bf_base_page = BeautifulSoup(html, 'lxml')
  cnt_lname = bf_base_page.find_all(class_='page-title-area clearfix')[0]
  cnt_summ = bf_base_page.find_all(class_='gaiyo-data')[0]
  cnt_inner = bf_base_page.find_all(class_='right-inner')[0]
  
  html = get_html_after_loaded(url2)
  bf_base_page = BeautifulSoup(html, 'lxml')
  try:
    cnt_note = bf_base_page.find_all(class_='right-inner')[0]
  except IndexError as e:
    cnt_note = bf_base_page.find_all(id='right-inner')[0]
  
  ans = (cnt_lname, cnt_summ, cnt_inner, cnt_note)
  return ans

# 開講元のリスト（つまり左側のあの赤いまたは青いもの）を取得する
def step0():
    d = get_department_list()
    e = get_department_list(lang='EN')
    pdump((d, e), path.savepath_unit_tree_raw)

def step1_new():
  d = {
    '理学院': {'GakubuCD': 1},
    '工学院': {'GakubuCD': 2},
    '物質理工学院': {'GakubuCD': 3},
    '情報理工学院': {'GakubuCD': 4},
    '生命理工学院': {'GakubuCD': 5},
    '環境・社会理工学院': {'GakubuCD': 6},
    '教養科目群': {'GakubuCD': 7, 'GakkaCD': 370000},
    '初年次専門科目': {'GakubuCD': 10},
  }
  args_default = {'module': 'General', 'action': 'T0100'}
  args_la = {'module': 'General', 'action': 'T0200', 'tab': 2, 'focus': 100}
  
  # load cache
  try:
    cl1, cl2, nr1, nr2 = pload(path.savepath_course_list_raw)
  except:
    cl1, cl2, nr1, nr2 = [], [], 0, 0
    pdump((cl1, cl2, nr1, nr2), path.savepath_course_list_raw)
      
  for (i, target) in enumerate(d):
    if i < nr1:
      continue
    url = form_url(addr_default, 
                   **(d[target]), 
                   **(args_la if target == '教養科目群' else args_default), 
                   **args_lang())
    print(target, url)
    html = get_html_after_loaded(url)
    lists = extractor.get_course_list(html)
    
    nr1 = i
    cl1 += lists
    
    backup_file(path.savepath_course_list_raw)
    pdump((cl1, cl2, nr1, nr2), path.savepath_course_list_raw)
  

# リストのあらゆる開講元におき科目リストを得る
def step1(start_year = 2016, start_year_old = 2009, omit_old_courses=False):
    unit_list, unit_list_old, ul_adds = pload(path.savepath_unit_tree)
    years = range(start_year, this_year + 1, 1)
    years_old = range(start_year_old, this_year + 1, 1)
  
    try:
      cl1, cl2, nr1, nr2 = pload(path.savepath_course_list_raw)
    except:
      cl1, cl2, nr1, nr2 = [], [], 0, 0
      pdump((cl1, cl2, nr1, nr2), path.savepath_course_list_raw)
      
    # New courses (depatrment-based)
    for i in tqdm(range(nr1, len(unit_list))):
      unit_args = unit_list[i][1]
      print(form_url(addr_default, **unit_args))
      ll = []
      for year in years:
        clist = get_course_list(unit_args, year=year)
        ll += clist
      cl1.append(ll)
      nr1 += 1
      
      backup_file(path.savepath_course_list_raw)
      pdump((cl1, cl2, nr1, nr2), path.savepath_course_list_raw)
      
    # old courses
    if omit_old_courses: return
    for i in tqdm(range(nr2, len(unit_list_old))):
      unit_args = unit_list_old[i][1]
      print(form_url(addr_default, **unit_args))
      ll = []
      for year in years_old:
        clist = get_course_list(unit_args, year=year)
        ll += clist
      cl2.append(ll)
      nr2 += 1
      
      backup_file(path.savepath_course_list_raw)
      pdump((cl1, cl2, nr1, nr2), path.savepath_course_list_raw)

def step2_new(start_year=2016):
  clist, _, _, _ = pload(path.savepath_course_list_raw)
  print(1)
  print(clist)
  

# 全ての科目（のID）におきシラバスを得る
def step2():
    clist, ars = pload(path.savepath_course_list)
    try:
      details, gshxd_code = pload(path.savepath_details_raw)
    except:
      details = {} # these codes are successful in scraping
      gshxd_code = [] # these codes are yabai
      pdump((details, gshxd_code), path.savepath_details_raw)
    
    # error correction
    while gshxd_code:
      code, dcode = gshxd_code.pop()
      print(code, dcode)
      detail_jp, detail_en = details[code]
      try:
        detail_jp[dcode] = extractor.parse_lecture_info(fetch_lecture_info(dcode, **args_lang('JP')))
        detail_en[dcode] = extractor.parse_lecture_info(fetch_lecture_info(dcode, **args_lang('EN')))
      except Exception as e:
        raise e
      details[code] = (detail_jp, detail_en)
      
    for code, _ in tqdm(ars):
      print(code)
      if code in details: continue
      detail_jp = {}
      detail_en = {}
      cc = clist[code]['classes']
      detail_to_craw = list_concat([list(cc[x]['sys_info'].values()) for x in cc])
      for dcode in detail_to_craw:
        try:
          detail_jp[dcode] = extractor.parse_lecture_info(fetch_lecture_info(dcode, **args_lang('JP')))
          detail_en[dcode] = extractor.parse_lecture_info(fetch_lecture_info(dcode, **args_lang('EN')))
        except:
          gshxd_code.append((code, dcode))
      details[code] = (detail_jp, detail_en)
      
      backup_file(path.savepath_details_raw)
      pdump((details, gshxd_code), path.savepath_details_raw)

if driver is None: init_driver(driver_path)
  
if __name__ == '__main__':
  # a = fetch_lecture_info_2('202002964', 2017, en=True)
  step2_new(2018)
  print(2)
  

  #step1(start_year=2021, start_year_old=2021, omit_old_courses=True)
  #a = get_all_course_list(1)