# -*- coding: utf-8 -*-
"""
Created on Wed Feb 19 17:16:23 2020

@author: billstark
"""

from typing import Union
from sys_utils import pdump, pload, backup_file
from utils import *
import extractor
from driver import DriverWrapper

from bs4 import BeautifulSoup
from tqdm import tqdm

addr_default = 'http://www.ocw.titech.ac.jp/index.php'

try:
  assert driver
except:
  driver = DriverWrapper(
    driver_path='./chromedriver104.exe', 
    mem_reset_limit=300
  )

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
  html = driver.get_html_after_loaded(url)
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
    html = driver.get_html_after_loaded(url)
    tbls_ans = extractor.get_course_list(html)
    if len(tbls_ans) > 0 and tbls_ans[0][3].startswith(str(year)):
      break
  
  return tbls_ans
  
def fetch_lecture_info_2(id_seed, year, en=False):
  '''
  Get the html return according to lecture id (and other information).
  lecture_info: id or (id, year) or (id, year, term)
  '''
  # request
  url0 = 'http://www.ocw.titech.ac.jp/index.php?module=General&action=T0300&JWC={year}{id}&lang={lang}'
  lang = 'JA' if not en else 'EN'
  url1 = url0.format(year=year, id=str(id_seed)[4:], lang=lang)
  url2 = url1 + '&vid=05'

  html = driver.get_html_after_loaded(url1)
  if html == '404':
    return None
  bf_base_page = BeautifulSoup(html, 'lxml')
  cnt_lname = bf_base_page.find_all(class_='page-title-area clearfix')[0]
  cnt_summ = bf_base_page.find_all(class_='gaiyo-data')[0]
  cnt_inner = bf_base_page.find_all(class_='right-inner')[0]
  
  html = driver.get_html_after_loaded(url2)
  bf_base_page = BeautifulSoup(html, 'lxml')
  try:
    cnt_note = bf_base_page.find_all(class_='right-inner')[0]
  except IndexError as e:
    cnt_note = bf_base_page.find_all(id='right-inner')[0]
  
  ans = (cnt_lname, cnt_summ, cnt_inner, cnt_note)
  return ans

# 開講元のリスト（つまり左側のあの赤いまたは青いもの）を取得する
def task1(outpath: str):
    d = get_department_list()
    e = get_department_list(lang='EN')
    pdump((d, e), outpath)

def task2(outpath: str):
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
    cl1, cl2, nr1, nr2 = pload(outpath)
  except:
    cl1, cl2, nr1, nr2 = [], [], 0, 0
    pdump((cl1, cl2, nr1, nr2), outpath)
      
  for (i, target) in enumerate(d):
    if i < nr1:
      continue
    url = form_url(addr_default, 
                   **(d[target]), 
                   **(args_la if target == '教養科目群' else args_default), 
                   **args_lang())
    print(target, url)
    html = driver.get_html_after_loaded(url)
    lists = extractor.get_course_list(html)
    
    nr1 = i
    cl1 += lists
    
    backup_file(outpath)
    pdump((cl1, cl2, nr1, nr2), outpath)
  
  
def task3(inpath: str, outpath: str, start_year=2016, end_year=2022):
  clist, _, _, _ = pload(inpath)
  targets = [x[1][1][0] for x in clist]
  
  # initialize storage
  try:
    details, gshxd_code = pload(outpath)
  except:
    details = {} # these codes are successful in scraping
    gshxd_code = [] # these codes are yabai
    pdump((details, gshxd_code), outpath)
  
  def task3_sub1(detail: dict, dcode: Union[int, str], year: int):
    if year in detail:
      return
    detail_jp = extractor.parse_lecture_info(fetch_lecture_info_2(dcode, year))
    detail_en = extractor.parse_lecture_info(fetch_lecture_info_2(dcode, year, en=True))
    detail[year] = (detail_jp, detail_en)
  
  # error correction
  while gshxd_code:
    dcode = gshxd_code.pop()
    print(f'Retrying errored record {dcode} ({len(gshxd_code) + 1} remains)')
    
    detail = details[dcode] if dcode in details else {}
    for year in range(start_year, end_year + 1):
      try:
        task3_sub1(detail, dcode, year)
      except Exception as e:
        print(f'{dcode}, {year}')
        raise e
      
    details[dcode] = detail
    
    backup_file(outpath)
    pdump((details, gshxd_code), outpath)
    
  # normal process
  exit_tag = False
  for dcode in tqdm(targets):
    detail = details[dcode] if dcode in details else {}
    err_rec = False
    for year in range(start_year, end_year + 1):
      try:
        task3_sub1(detail, dcode, year)
      except KeyboardInterrupt:
        exit_tag = True
        break
      except Exception as e:
        if not err_rec:
          gshxd_code.append(dcode)
          err_rec = True
        print(f'ERROR {dcode} in year {year}: {e.message}')
        
    # to avoid incomplete records
    if exit_tag:
      print('Interrupted by user.')
      break
        
    details[dcode] = detail
    
    backup_file(outpath)
    pdump((details, gshxd_code), outpath)
    
    
  
if not driver.is_initialized():
  driver.init_driver()
  
if __name__ == '__main__':
  pass
  # a = fetch_lecture_info_2('202002964', 2017, en=True)
  #step1(start_year=2021, start_year_old=2021, omit_old_courses=True)
  #a = get_all_course_list(1)