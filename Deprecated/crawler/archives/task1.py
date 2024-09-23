# -*- coding: utf-8 -*-
"""
Created on Sat Aug 14 14:25:29 2021

@author: billstark
"""

#
from sys_utils import *
from utils import *
import shutil

# ますは開講元ごとのコースリストを取得する
# もし既にデータを取得したら、そのデータを使い、再び取得しない
try:
  tree, _, _ = pload(path.savepath_unit_tree)
  list_by_depts, _, _, _ = pload(path.savepath_course_list_raw)
except:
  import spider
  spider.step0()
  import build_tree
  spider.step1(start_year=2021, start_year_old=2021, omit_old_courses=True)
  import clean_data

# 必要でないものを捨て、開講元の「アドレス」とOCW IDだけ残す
courses_by_dept_raw = dict([[tree[i][0], [x[1][1][0] for x in list_by_depts[i]]] for i in range(len(tree))])

# 院と開講元のIDに関するデータを導入する
dept_ids = [1, 2, 3, 4, 5, 6, 7, 10, '00']
dept_names = ['理学院', '工学院', '物質理工学院', '情報理工学院', '生命理工学院', '環境・社会理工学院', '教養科目群', '初年次専門科目', '学位プログラムとして特別に設けた教育課程']
sdm_raw = jload(path.school_dep_map_path)
school2dep = dict([list(y.values())[::-1][1:] for y in list_concat([x['department'] for x in sdm_raw])])

# 開講元にIDをつけ、つけられないものを-1とする
dept_id_map = defaultdict(lambda: -1)
for dept in courses_by_dept_raw:
  least_significant_addr = dept.split('.')[-1]
  second_least_significant_addr = dept.split('.')[-2] if dept.find('.') >= 0 else ''
  if least_significant_addr in school2dep:
    dept_id_map[dept] = school2dep[least_significant_addr]
  elif second_least_significant_addr + least_significant_addr in school2dep:
    dept_id_map[dept] = school2dep[second_least_significant_addr + least_significant_addr]
  elif least_significant_addr + '共通専門科目' in school2dep:
    dept_id_map[dept] = school2dep[least_significant_addr + '共通専門科目']
  
# IDごとに科目リストを作る
courses_by_dept = {-1: []}
for i in school2dep.values(): courses_by_dept[i] = []
for d in courses_by_dept_raw: courses_by_dept[dept_id_map[d]] += courses_by_dept_raw[d]
for d in courses_by_dept:
  courses_by_dept[d] = list_clean_repeats(sorted(courses_by_dept[d]), lmb_identity)
# 未分類の科目リストにおき、他のリストと重複の科目を消す
for d in courses_by_dept:
  if d == -1: continue
  for c in courses_by_dept[d]:
    try:
      cc = courses_by_dept[-1].index(c)
      del courses_by_dept[-1][cc]
    except ValueError:
      pass

# スクレーパーを呼び出し、全ての科目のデータを集計する
# もし既にデータを取得したら、そのデータを使い、再び取得しない
try:
  assert courses
  assert courses_en
except:
  import spider
  courses = [spider.get_all_course_list(x) for x in dept_ids]
  courses_en = [spider.get_all_course_list(x, lang='EN') for x in dept_ids]

  addr_default = spider.addr_default

def reform(datum, default_dept='None'):
  ayq = parse_ay_and_q(datum[4])
  dept_name = datum[3][0][0] if datum[3][0][0] else default_dept
  dept = school2dep[dept_name] if dept_name in school2dep else -1
  t_en = en_course_data[datum[1][1][0]]
  try:
    sq = ayq[1].index(True) + 1
    eq = 4 - ayq[1][::-1].index(True)
  except:
    sq = 0
    eq = 0
  teacher_list = [x[1] for x in datum[2]]
  ans = {
    "startQuarter" : sq,
    "endQuarter" : eq,
    "ocwId" : datum[1][1][0],
    "courseNumber" : datum[0],
    "year" : ayq[0],
    "titleJa" : datum[1][0],
    "titleEn" : t_en,
    "department" : dept,
    "teachers" : teacher_list
  }
  return ans

# 英語の講義名だけ使う
en_course_data = [[d[1][1][0], d[1][0]] for d in list_concat(courses_en)]
en_course_data = dict(list_clean_repeats(sorted(en_course_data, key=lambda x: str(x[0])), lambda x: x[0]))

# コースをDBが使える辞書型に変える
course_sort_func_str = lambda x: x['ocwId'] * 114514 \
  + int(x['department'] in ['学位プログラムとして特別に設けた教育課程共通専門科目', '教養科目群共通専門科目']) * 10 \
    + int(x['department'].endswith('共通専門科目'))
course_sort_func_int = lambda x: x['ocwId'] * 114514 + (x['department'] if x['department'] > 0 else 100)

courses_reformed = list_concat([[reform(x, default_dept=dept_names[i] + '共通専門科目') for x in courses[i]] for i in range(len(dept_ids))])
courses_reformed = [x for x in courses_reformed if x['ocwId'] != '']
courses_reformed.sort(key=course_sort_func_int)
courses_reformed = list_clean_repeats(courses_reformed, f=lambda x: x['ocwId'])

# courses_by_dept[-1]一部の科目は実に他の開講元に所属するが、OCWのウェブページではその開講元からアクセスできない
# 全ての科目の開講元を得たうえて、この問題を解決できるはず
# 
uncategorized_courses = []
for c in courses_by_dept[-1]:
  try_to_get_dept = [x['department'] for x in courses_reformed if x['ocwId'] == c]
  if try_to_get_dept:
    dept = try_to_get_dept[0]
    courses_by_dept[dept].append(c)
  else:
    uncategorized_courses.append(c)
courses_by_dept[-1] = uncategorized_courses
for d in courses_by_dept:
  courses_by_dept[d] = list_clean_repeats(sorted(courses_by_dept[d]), lmb_identity)

# 全ての講義の教師名を取り出し、教師IDの辞書を作る
teacher_list_ja = dict(list_clean_repeats(sorted(list_concat(list_concat([[[w[::-1] for w in x[2]] for x in y] for y in courses]))), lmb_identity))
teacher_list_en = dict(list_clean_repeats(sorted(list_concat(list_concat([[[w[::-1] for w in x[2]] for x in y] for y in courses_en]))), lmb_identity))
teacher_list = [dict(id=x, nameJa=teacher_list_ja[x], nameEn=teacher_list_en[x]) for x in teacher_list_ja]

# 開講元のUrlの辞書を作る
department_list = list_clean_repeats(sorted(list_concat(list_concat([[[[w[0], form_url(__url__=addr_default, **w[1])] for w in x[3]] for x in y] for y in courses]))), lmb_identity)
department_dict = [dict(name=x[0], url=x[1]) for x in department_list if x[0]]

# IDごとに科目リストを作る
# strictの意味は、一つの科目は一つのリスト（開講元のリスト）だけ所属するである
courses_by_dept_strict = defaultdict(list)
for c in courses_reformed:
  courses_by_dept_strict[c['department']].append(c['ocwId'])
for dept in courses_by_dept_strict:
  courses_by_dept_strict[dept] = list_clean_repeats(sorted(courses_by_dept_strict[dept]), lmb_identity)
  
  
# データ出力
try:
  os.makedirs('./task1/by_department/')
except:
  pass
try:
  os.makedirs('./task1/by_department_strict/')
except:
  pass

jdump(courses_reformed, './task1/courses.json')
jdump(teacher_list, './task1/teachers.json')
jdump(department_dict, './task1/departments.json')
jdump(courses_by_dept_strict, './task1/by_department_strict.json')
jdump(courses_by_dept, './task1/by_department.json')
for d in courses_by_dept_strict.items():
  jdump(d[1], f'./task1/by_department_strict/{d[0]}.json')
for d in courses_by_dept.items():
  jdump(d[1], f'./task1/by_department/{d[0]}.json')