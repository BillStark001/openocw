# -*- coding: utf-8 -*-
"""
Created on Wed Feb 19 17:16:23 2020

@author: billstark
"""

from sys_utils import *
from utils import *

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

strip_useless_elem = lambda l: [x for x in l if str(x) != '\n' and str(x) != '\xa0'] 

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

def init_driver(path='chromedriver91.exe'):
    global driver
    options = webdriver.ChromeOptions()
    #options.add_argument('user-agent="Mozilla/5.0 (Linux; Android 4.0.4; Galaxy Nexus Build/IMM76B) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.133 Mobile Safari/535.19"')
    options.add_argument('user_agent="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.101 Safari/537.36"')
    driver = webdriver.Chrome(path, options=options)

def get_html(*args, **kwargs):
    global mem_reset_count
    if mem_reset_count >= mem_reset_limit:
        mem_reset_count = 0
        driver.quit()
        init_driver()
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

this_year = 2021
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
    bf_base = BeautifulSoup(html, 'lxml')
    lbs = [bf_base.find('div', id='left-body-1'), bf_base.find('div', id='left-body-2')] + \
             [bf_base.find('div', id='left-body-3'), bf_base.find('div', id='left-body-4')]
    
    cats = []
    cats_flat = {}
    excluded = []
    
    def sub1(x, prev):
        if x.a == None: 
            # print(x)
            return False
        name = str(fix_web_str(x.a.string if x.a.span == None else x.a.span.string))
        info = x.a
        if 'href' in info.attrs: 
            xah = deform_url(x.a['href'] if x.a['href'] != '#' else url)[1]
            non_exclude = ['GakubuCD', 'GakkaCD', 'KeiCD', 'KamokuCD', 'course', 'SubAction']
            for k in list(xah.keys()):
                if k not in non_exclude: 
                    if k not in excluded:
                        excluded.append(k)
                    del xah[k]
                    # if k in ['LeftTab']: print(x.a['href'])
            cats_flat[name] = xah
        else:
            xah = {}
            for y in x.ul: 
                if y.name == 'li': sub1(y, xah)
        prev[name] = xah
        return True
    
    for lb in lbs:
        li_classes = [x for x in lb.ul if x.name == 'li']
        sdmap = {}
        for sch in li_classes:
            sub1(sch, sdmap)
        cats.append(sdmap)
    
    print(excluded)
    return cats, cats_flat

def get_all_course_list(dept_id, lang='JA', retry_limit=5):
    args = dict(module='General', action='T0100' if dept_id != '00' else 'T0100L', GakubuCD=dept_id, lang=lang)
    url = form_url(__url__=addr_default, **args)
    
    for _ in range(retry_limit):
        html = get_html_after_loaded(url)
        bf_base = BeautifulSoup(html, 'lxml')
        tbls = [x.tbody for x in bf_base.find_all('table', class_='ranking-list') if x.tbody != None]
        tbls_ans = []
        if len(tbls) == 0:
            continue
        for tbl in tbls:
            trs = [x for x in tbl.contents if x.name == 'tr']
            strs = [[[[fix_web_str(z.string), deform_url(z['href'])[1]] for z in x.find_all('a') if 'href' in z.attrs] if x.a != None else \
                     (x.img['src'] if x.img != None else fix_web_str(x.string))for x in y.find_all('td') ] for y in trs]
            tbls_ans += strs
        if len(tbls_ans) > 0:
            break
    for datum in tbls_ans:
        datum[1] = datum[1][0]
        datum[1][1] = (datum[1][1]['KougiCD'], datum[1][1]['Nendo'], datum[1][1]['Gakki']) if 'Gakki' in \
            datum[1][1] else (datum[1][1]['KougiCD'], datum[1][1]['Nendo'])
        for d2 in datum[2]:
            d2[1] = d2[1]['id']
    return tbls_ans
        
        

def get_course_list(info, year=this_year, retry_limit=5):
    args_course = args_general_list if year >= this_year else args_archive_list
    if year >= this_year:
        if 'SubAction' in info:
            info = dict(info)
            info['action'] = info['SubAction']
            del info['SubAction']
    url = form_url(__url__=addr_default, **args_course, **args_year(year), **info)
    #print(url)
    for _ in range(retry_limit):
        html = get_html_after_loaded(url)
        bf_base = BeautifulSoup(html, 'lxml')
        tbls = [x.tbody for x in bf_base.find_all('table', class_='ranking-list') if x.tbody != None]
        tbls_ans = []
        if len(tbls) == 0:
            continue
        for tbl in tbls:
            trs = [x for x in tbl.contents if x.name == 'tr']
            strs = [[[[fix_web_str(z.string), deform_url(z['href'])[1]] for z in x.find_all('a') if 'href' in z.attrs] if x.a != None else \
                     (x.img['src'] if x.img != None else fix_web_str(x.string))for x in y.find_all('td') ] for y in trs]
            tbls_ans += strs
        if len(tbls_ans) > 0 and tbls_ans[0][3].startswith(str(year)):
            break
    for datum in tbls_ans:
        datum[1] = datum[1][0]
        datum[1][1] = (datum[1][1]['KougiCD'], datum[1][1]['Nendo'], datum[1][1]['Gakki']) if 'Gakki' in \
            datum[1][1] else (datum[1][1]['KougiCD'], datum[1][1]['Nendo'])
        for d2 in datum[2]:
            d2[1] = d2[1]['id']
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
    
def parse_lecture_info(info_caches):
    
    cnt_lname, cnt_summ, cnt_inner, cnt_note = info_caches
    
    # lname
    lname_ = cnt_lname.h3.contents[0]
    try:
        lname = lname_.split('\u3000')
        lname[1] = lname[1].split('\xa0\xa0\xa0')
        lname = [lname[0]] + lname[1]
    except:
        lname = [None] + lname_.split('\xa0\xa0\xa0')
    
    # summary
    summ = [[x.dt.contents[0].strip('\n'), x.dd.contents] for x in cnt_summ.find_all('dl') if x.dt != None and x.dd != None]
    for summ_key in summ:
        summ_key[1] = strip_useless_elem(summ_key[1])
        if len(summ_key[1]) == 1 and summ_key[0] != '担当教員名': 
            summ_key[1] = summ_key[1][0]
            if isinstance(summ_key[1], str): 
                summ_key[1] = fix_web_str(summ_key[1])
                sk_tmp = None
                while sk_tmp != summ_key[1]:
                    sk_tmp = summ_key[1]
                    summ_key[1] = summ_key[1].replace('  ', ' ')
            else: #rankings
                summ_key[1] = summ_key[1]['src']
        else: # lecturers
            sk1 = summ_key[1]
            sk = []
            for lect in sk1:
                if isinstance(lect, str):
                    lect_tmp = fix_web_str(lect)
                    if len(lect_tmp) > 0:
                        print(len(lect_tmp), lect_tmp)
                        sk.append((-1, lect_tmp))
                else:
                    lect_addr = deform_url(lect['href'])[1]['id']
                    lect_name = fix_web_str(lect.string)
                    sk.append((lect_addr, lect_name))
            summ_key[1] = sk
    summ = ltod(summ)
    
    # contents
    conts_orig = [[x for x in y if str(x) != '\n' and str(x) != '\xa0']  for y in cnt_inner.find_all(class_='cont-sec')]
    conts = {}
    for cont in conts_orig:
        k = str(fix_web_str(cont[0].contents[0]))
        v = cont[1]
        if v.name == 'table':
            if k in ['Competencies that will be developed', '学生が身につける力(ディグリー・ポリシー)']: # skills
                vtmp = [[y.contents[0] if not isinstance(y, str) else y for y in x.contents] for x in v.tbody.find_all('td')]
                skills_dict = {
                    '専門力': 0, '教養力': 1, 'コミュニケーション力': 2, '展開力(探究力又は設定力)': 3, '展開力(実践力又は解決力)': 4, 
                    'Specialist skills': 0, 'Intercultural skills': 1, 'Communication skills': 2, 'Critical thinking skills': 3, 'Practical and/or problem-solving skills': 4
                    }
                v = [False] * 5
                for skl in vtmp: 
                    if len(skl) == 2: v[skills_dict[skl[1]]] = True
            elif k in ['Course schedule/Required learning', '授業計画・課題']:
                vtmp = [[str(y.string) for y in x.contents if not (str(y) == '\n')] for x in v.tbody.find_all('tr')]
                v = vtmp
            elif k in ['Course taught by instructors with work experience', '実務経験のある教員等による授業科目等']:
                v = v.tbody.find_all('tr')[-1].td.string
                v = fix_web_str(v)
            else:
                print(v.name)
                raise Exception()
        elif v.name == 'p':
            vc = v.contents
            vcs = ''
            for i in vc: vcs += str(i)
            v = vcs.replace('<br/>', '\n')
        elif v.name == 'ul':
            v = [[y.strip(' ') for y in fix_web_str(x.contents[0]).split('：')] for x in v.find_all('li')]
        elif v.name == 'div':
            # TODO what about the god damn video format contents?
            v = str(v)
            '''
            vs = []
            for dd in [x for x in v.contents if x.name == 'div']:
                video_src = dd.find('video')['src']
                video_title = dd.find('div', class_='movie_th').string
                video_title = fix_web_str(video_title)
                vs.append((video_src, video_title))
            v = vs
            '''
        else:
            print(v.name)
            raise Exception()
        conts[k] = v
    
    # docs
    try:
        docs_supp = cnt_note.dl.dd.find_all('li')
    except AttributeError as e:
        docs_supp = []
    docs_supp = [strip_useless_elem(x.contents)[-2:] for x in docs_supp]
    docs_supp = [(str(x[0]['href']), str(x[0].contents[0]), str(x[1]['src'])) for x in docs_supp]
    docs_supp_meta = (None, None)
    
    docs_orig = cnt_note.find_all('div', class_='cont-sec')
    docs = {'supp': (docs_supp_meta, docs_supp)}
    for doc_orig in docs_orig:
        nhead = doc_orig.find(class_='note-head')
        nlower = doc_orig.find(class_='clearfix note-lower')
        if nhead is None or nlower is None: # adobe pdf related 
            continue
        k = str(nhead.h3.string)
        lecture_type = fix_web_str(str(nhead.p.string))
        start_date = fix_web_str(str(nlower.p.string))
        notes = [[str(x.a['href'])] + strip_useless_elem(x.a.contents)[1:] + ([x.img] if x.img is not None else []) for x in nlower.find_all('div', class_='pdf-link')]
        notes = [(x[0], fix_web_str(str(x[1])), str(x[2]['src'])) for x in notes]
        meta = (lecture_type, start_date)
        docs[k] = (meta, notes)
    
    #print(lname)    
    return lname, summ, conts, docs

# 開講元のリスト（つまり左側のあの赤いまたは青いもの）を取得する
def step0():
        d = get_department_list()
        e = get_department_list(lang='EN')
        pdump((d, e), path.savepath_unit_tree_raw)

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
            print(form_url(None, **unit_args))
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
            print(form_url(None, **unit_args))
            ll = []
            for year in years_old:
                clist = get_course_list(unit_args, year=year)
                ll += clist
            cl2.append(ll)
            nr2 += 1
            
            backup_file(path.savepath_course_list_raw)
            pdump((cl1, cl2, nr1, nr2), path.savepath_course_list_raw)

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
                detail_jp[dcode] = parse_lecture_info(fetch_lecture_info(dcode, **args_lang('JP')))
                detail_en[dcode] = parse_lecture_info(fetch_lecture_info(dcode, **args_lang('EN')))
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
                    detail_jp[dcode] = parse_lecture_info(fetch_lecture_info(dcode, **args_lang('JP')))
                    detail_en[dcode] = parse_lecture_info(fetch_lecture_info(dcode, **args_lang('EN')))
                except:
                    gshxd_code.append((code, dcode))
            details[code] = (detail_jp, detail_en)
            
            backup_file(path.savepath_details_raw)
            pdump((details, gshxd_code), path.savepath_details_raw)

if driver is None: init_driver()
    
if __name__ == '__main__':
    pass
    #step1(start_year=2021, start_year_old=2021, omit_old_courses=True)
    #a = get_all_course_list(1)