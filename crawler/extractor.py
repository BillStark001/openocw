from bs4 import BeautifulSoup
from utils import fix_web_str, deform_url, form_url, strip_useless_elem, ltod


def get_department_list(html, url_in=''):

  bf_base = BeautifulSoup(html, 'lxml')
  lbs = [bf_base.find('div', id='left-body-1'), 
         bf_base.find('div', id='left-body-2')] + \
      [bf_base.find('div', id='left-body-3'),
       bf_base.find('div', id='left-body-4')]

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
      xah = deform_url(x.a['href'] if x.a['href'] != '#' else url_in)[1]
      non_exclude = ['GakubuCD', 'GakkaCD',
                     'KeiCD', 'KamokuCD', 'course', 'SubAction']
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
        if y.name == 'li':
          sub1(y, xah)
    prev[name] = xah
    return True

  for lb in lbs:
    sdmap = {}
    li_classes = [x for x in lb.ul if x.name == 'li'] if lb != None else []
    for sch in li_classes:
      sub1(sch, sdmap)
    cats.append(sdmap)

  return cats, cats_flat, excluded


def get_course_list(html):

  bf_base = BeautifulSoup(html, 'lxml')
  tbls = [
      x.tbody
      for x in bf_base.find_all('table', class_='ranking-list')
      if x.tbody != None
  ]

  tbls_ans = []
  if len(tbls) == 0:  # no available data
    return tbls_ans

  for tbl in tbls:
    trs = [x for x in tbl.contents if x.name == 'tr']
    strs = \
        [
            [
                [
                    [fix_web_str(z.string), deform_url(z['href'])[1]]
                    for z in x.find_all('a')
                    if 'href' in z.attrs
                ]
                if x.a != None else
                (x.img['src'] if x.img != None else fix_web_str(x.string))
                for x in y.find_all('td')
            ]
            for y in trs
        ]
    tbls_ans += strs

  for datum in tbls_ans:
    datum[1] = datum[1][0]
    datum[1][1] = (datum[1][1]['KougiCD'], datum[1][1]['Nendo'], datum[1][1]['Gakki']) if 'Gakki' in \
        datum[1][1] else (datum[1][1]['KougiCD'], datum[1][1]['Nendo'])
    for d2 in datum[2]:
      d2[1] = d2[1]['id']
  return tbls_ans


def parse_lecture_info(info_caches):
  if info_caches == None:
    return None
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

if __name__ == '__main__':
  with open('./test_data/course_list_test.html', 'r', encoding='utf8') as f:
    html = f.read()
  depts = get_department_list(html)