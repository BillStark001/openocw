import re
import datetime
from collections import defaultdict
import collections
from db_definitions import *

from sys_utils import *

def gather_data(inpath: str):
  details, _ = pload(inpath)
  ans = defaultdict(lambda: defaultdict(int))
  anst = defaultdict(dict)
  ansu = {}
  for code in details:
    for year in details[code]:
      subu = [None, None]
      for item in details[code][year]:
        if not item:
          continue
        for k in item[1]:
          if isinstance(item[1][k], collections.Hashable):
            ans[k][item[1][k]] += 1
        for k in item[2]:
          if isinstance(item[2][k], collections.Hashable):
            ans[k][item[2][k]] += 1
        
        if '担当教員名' in item[1]  and type(item[1]['担当教員名']) == list:
          for id, tn in item[1]['担当教員名']:
            anst[id]['ja'] = tn  
        if 'Instructor(s)' in item[1] and type(item[1]['Instructor(s)']) == list:
          for id, tn in item[1]['Instructor(s)']:
            anst[id]['en'] = tn  
        
        if '開講元' in item[1]:
          subu[0] = item[1]['開講元']
          
        if 'Academic unit or major' in item[1]:
          subu[1] = item[1]['Academic unit or major']
      if subu[0] != None:
        ansu[subu[0]] = subu[1]
        
  return ans, anst, ansu

def normalize_brackets(dstr: str) -> str:
  return dstr \
    .replace('（', '(') \
    .replace('）', ')') \
    .replace('【', '[') \
    .replace('】', ']')
        
def detect_kw(dstr: str, kws: list[str]) -> bool:
  for kw in kws:
    if dstr.find(kw) >= 0:
      return True
  return False

def bool2bin(arr: list[bool]) -> int:
  ans = 0
  for i, b in enumerate(arr):
    ans += (1 << i) * b
  return ans

def bin2bool(bin: int, digit: int) -> list[bool]:
  ans = []
  for i in range(digit):
    ans.append((bin & (1 << i)) > 0)
  return ans
        
def find_brackets(
  passage: str, 
  start: int=0, 
  brl: str='(', 
  brr: str=')', 
  strict: bool=False, 
  exclude: tuple[str, str]=('"', '\\"')
  ) -> tuple[int, int]:

  pointer: int = start
  endpoint: int = len(passage)
  lbrl = len(brl)
  lbrr = len(brr)
  lex0 = len(exclude[0]) if exclude else -1
  lex1 = len(exclude[1]) if exclude else -1

  layer = 0
  outer_brl = -1
  while pointer < endpoint:
    next_brl = passage.find(brl, pointer)
    next_brr = passage.find(brr, pointer)
    next_exclude = passage.find(exclude[0], pointer) if exclude else -1
    # print(pointer, next_brl, next_brr, next_exclude, layer)

    if next_exclude >= 0 and \
      (next_brl < 0 or next_exclude < next_brl) and \
      (next_brr < 0 or next_exclude < next_brr):
      pointer = next_exclude + lex0
      while True:
        next_exclude_2 = passage.find(exclude[0], pointer)
        next_ignore_exclude_2 = passage.find(exclude[1], pointer)
        if next_ignore_exclude_2 >= 0 \
          and next_exclude_2 > next_ignore_exclude_2 \
          and next_exclude_2 <= next_ignore_exclude_2 + lex1:
          # this exclude mark is ignored
          pointer = next_ignore_exclude_2 + lex1
        elif next_exclude_2 >= 0:
          # this mark is real
          pointer = next_exclude_2 + lex0
          break
        else:
          if strict:
            raise ValueError('Excluding section matching failed')
          else:
            return (outer_brl, -1)

    elif next_brl >= 0 and (next_brl < next_brr or next_brr < 0):
      pointer = next_brl + lbrl
      if layer == 0:
        outer_brl = next_brl
      layer += 1
    elif next_brr >= 0 and (next_brr < next_brl or next_brl < 0):
      if layer == 0:
        if strict:
          raise ValueError('Brackets matching failed (no left bracket)')
        else:
          pointer = next_brr + lbrr
          continue
      else:
        pointer = next_brr + lbrr
        layer -= 1
        if layer == 0:
          return (outer_brl, next_brr)
    elif next_brl < 0 and next_brr < 0:
      return (-1, -1)
    else:
      raise ValueError('what the hell?')
  if strict:
    raise ValueError('Brackets matching failed (no right bracket)')
  return (outer_brl, -1)

def parse_form(dstr: str):
  ans = 0
  
  dstr = normalize_brackets(dstr).lower()
  fl_lect = detect_kw(dstr, ['講義', 'lecture'])
  fl_exer = detect_kw(dstr, ['演習', '練習', 'exercise', 'recitation'])
  fl_expr = detect_kw(dstr, ['実験', 'experiment'])
  
  ans += bool2bin([fl_lect, fl_exer, fl_expr])
  
  brl, brr = find_brackets(dstr)
  if brl >= 0:
    dstr_br = dstr[brl+1:] if brr < 0 else dstr[brl+1: brr]
    fl_hybrid = detect_kw(dstr_br, ['ハイフレックス', 'ブレンド', 'blend', 'hyflex', 'hybrid'])
    fl_online = detect_kw(dstr_br, ['zoom', 'livestream', 'online', 'ライブ', 'オンライン'])
    fl_offline = detect_kw(dstr_br, ['対面', 'face-to-face', 'offline', 'オンライン'])
    ans += bool2bin([fl_online or fl_hybrid, fl_offline or fl_hybrid]) * 16
    
  return ans

def parse_day(dstr: str) -> int:
  dstr = dstr.lower()
  for i, groups in enumerate([
    ['月', 'mon'], 
    ['火', 'tue'], 
    ['水', 'wed'], 
    ['木', 'thu'], 
    ['金', 'fri'], 
    ['土', 'sat'], 
    ['日', 'sun'],
  ]):
    if detect_kw(dstr, groups):
      return i + 1
  return 0

def parse_addr(dstr: str):
  dstrp = normalize_brackets(dstr.lower())
  ans = []
  pat_time = r' *(月|火|水|木|金|土|日|mon|tue|wed|thur?|fri|sat|sun) *(\d{1,2})[-~]?(\d{1,2})? *'
  pat_spec = r' *(集中講義等?|オン・?デマンド|講究等?|ゼミ|セミナー|intensive|on\-?demand|seminar) *(\d{1,2})?[-~]?(\d{1,2})? *'
  
  def parse_loc(pos: int) -> str:
    brl, brr = find_brackets(dstrp, pos)
    if brl < 0:
      return ''
    elif brr < 0:
      return dstr[brl+1:]
    return dstr[brl+1: brr]
  
  res_time = list(re.finditer(pat_time, dstrp))
  res_spec = list(re.finditer(pat_spec, dstrp))
  for res in res_time:
    day = parse_day(res.group(1))
    start = int(res.group(2))
    end = int(res.group(3)) if res.group(3) else start
    loc_pos = res.span()[1]
    loc = parse_loc(loc_pos) if loc_pos < len(dstrp) and dstrp[loc_pos] == '(' else ''
    ansd = form_address_scheme()
    ansd[KEY_ADDR_TYPE] = VAL_TYPE_NORMAL
    ansd[KEY_ADDR_TIME] = {
      KEY_ADDR_DAY: day, 
      KEY_ADDR_START: start, 
      KEY_ADDR_END: end, 
    }
    ansd[KEY_ADDR_LOCATION] = loc
    ans.append(ansd)
  for res in res_spec:
    ins, ine = res.span(1)
    loc_pos = res.span()[1]
    start = int(res.group(2)) if res.group(2) else 0
    end = int(res.group(3)) if res.group(3) else start
    loc = parse_loc(loc_pos) if loc_pos < len(dstrp) and dstrp[loc_pos] == '(' else ''
    ansd = form_address_scheme()
    ansd[KEY_ADDR_TYPE] = VAL_TYPE_SPECIAL
    ansd[KEY_ADDR_TIME] = {
      KEY_ADDR_DESC: dstr[ins: ine], 
      KEY_ADDR_START: start, 
      KEY_ADDR_END: end, 
    }
    ansd[KEY_ADDR_LOCATION] = loc
    ans.append(ansd)
  
  if not ans:
    ansd = form_address_scheme()
    ansd[KEY_ADDR_TYPE] = VAL_TYPE_UNKNOWN
    ansd[KEY_ADDR_TIME][KEY_ADDR_DESC] = dstr
    ans.append(ansd)
  
  return ans
  

def parse_date(dstr):
  '''
  return: (year, month, date) or None
  '''
  patterns = [r'( *\d+ *)/( *\d+ *)/( *\d+ *)', r'( *\d+ *)-( *\d+ *)-( *\d+ *)', r'( *\d+ *)年( *\d+ *)月( *\d+ *)日']
  ans = 0
  for p in patterns:
    rs = re.search(p, dstr)
    try:
      anst = [int(rs.group(i).strip(' ')) for i in range(1, 4)]
      ans = to_timestamp(*anst)
    except Exception as e:
      # print(e)
      pass
  # return ans
  return datetime.datetime.fromtimestamp(ans / 1000, tz=datetime.timezone.utc)

def parse_ay_and_q(dstr):
  pattern = r' *(H?R?\d{1,4}) *年?度? *(.+) *Q'
  rs = re.search(pattern, dstr)
  ans = None
  try:
    anst = [rs.group(i).strip(' ') for i in range(1, 3)]
    anst[0] = int(anst[0])
    anst[1] = parse_quarter(anst[1])
    ans = tuple(anst)
  except:
    pass
  return ans

def parse_quarter(dstr) -> int:
  ans: int = 0
  try:
    anst = dstr.replace('Q', '').strip()
    anst1 = [False, False, False, False]
    last_d = 0
    bar = False
    for d in anst: 
      try: 
        cur_d = int(d)-1
        anst1[cur_d] = True
        if bar:
          for dd in range(last_d, cur_d):
            anst1[dd] = True
        last_d = cur_d
      except:
        if d in ['-', '~']:
          bar = True
    
    ans = bool2bin(anst1)
  except Exception as e:
    raise e
  return ans
  

def parse_contacts(dstr):
  bad_signs = ['：', '；', '，', '、', '[at]', '\u3000', '（', '）', '【', '】']
  good_signs = [':', ';', ',', ',', '@', ' ', '(', ')', '[', ']']
  for i in range(len(bad_signs)):
    dstr = dstr.replace(bad_signs[i], good_signs[i])
  dstr = [x for x in dstr.split('\n') if x]
  dstr = [x.strip(' ') for x in list_concat([x.split(';') for x in dstr])]
  return dstr

def parse_book(bstr):
  none_mark = len(bstr) <= 10 and (bstr.find('なし') >= 0 or bstr.find('無し') >= 0 \
                       or bstr.find('指定しない') >= 0 or bstr.find('定めない') >= 0 \
                         or bstr.find('ません') >= 0)
  lect_word_list = ['講義', '演習', '資料', '配信', '配布', 'アップロード', 'OCW', 'PDF', 'テキスト']
  lect_note_prob = len([x for x in [bstr.find(w) >= 0 for w in lect_word_list] if x])
  lect_mark = lect_note_prob >= 2

  if none_mark: return 'None'
  if lect_mark: return 'Lecture Note'
  return bstr

def parse_acrk(ar) -> float:
  return float(re.search('acbar(\d+).gif', ar).group(1))

lang_dict = {
  '日本語': 'ja', 
  '英語': 'en', 
  'Japanese': 'ja', 
  'English': 'en'
}
parse_lang = lambda x: lang_dict[x] if x in lang_dict else x


if __name__ == '__main__':
  print(parse_addr('集中講義等 5-8(Zoom)'))
  print(parse_addr('月5-8(物理学生実験室（石川台6・南５）) 木5-8(物理学生実験室（石川台6・南５）)'))