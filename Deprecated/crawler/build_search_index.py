from typing import List
from tqdm import tqdm
from fugashi import Tagger

from db_definitions import *
from db_utils import *
from db_oprs import *
TEXT_INDEX_TARGET = f'{KEY_META}.{KEY_SEARCH_REC}'
TEXT_DIRTY_TARGET = f'{KEY_META}.{KEY_DIRTY}'
TEXT_INDEX_DIRTY_SCHEME = {
  TEXT_DIRTY_TARGET: True
}

try:
  assert tagger
except:
  tagger = Tagger('-Owakati')
  
def build_stopwords():
  sw = {
  }
  for fn in ['./data/stopwords_{}.txt'.format(x) for x in ['en', 'ja', 'zh', 'py']]:
    with open(fn, 'r', encoding='utf-8') as f:
      while True:
        fl = f.readline()
        if not fl:
          break
        word = fl.replace('\n', '').replace('\r', '')
        sw[word] = True
  return sw

try:
  assert stopwords
except:
  stopwords = build_stopwords()
      
      
def create_index(db: MongoClient):
  ccrs, ccls, cfct = get_cols(db)
  scheme = [(TEXT_INDEX_TARGET, 'text')]
  ccrs.create_index(scheme)
  ccls.create_index(scheme)
  cfct.create_index(scheme)
  
def get_random_dirty_doc(
  col: Collection, 
  sess: Optional[ClientSession] = None
  ) -> Optional[Dict[str, Any]]:
  return col.find_one(TEXT_INDEX_DIRTY_SCHEME, session=sess)

def update_text_index(
  col: Collection, doc: Dict[str, Any], index: List[str],
  sess: Optional[ClientSession] = None, 
  ) -> bool:
  res = col.find_one_and_update({'_id': doc['_id'],}, {'$set': 
      {
        TEXT_INDEX_TARGET: index, #f'"{" ".join(index)}"', 
        TEXT_DIRTY_TARGET: False
      }
    }, session=sess)
  return res is not None
  
def iter_dirty_doc(
  col: Collection, sess: Optional[ClientSession] = None
  ) -> Optional[Dict[str, Any]]:
  doc = get_random_dirty_doc(col, sess)
  while doc is not None:
    yield doc
    doc = get_random_dirty_doc(col, sess)
  
  
if __name__ == '__main__':
  db = init_db()
  create_index(db)
  
  ccrs, ccls, cfct = get_cols(db)
  
  for doc in tqdm(iter_dirty_doc(ccls)):
    s = {}
    for word in tagger(str(doc)):
      wpos = str(word.pos).split(',')[0]
      if '記号' in wpos:
          continue
      wstr = str(word)
      wfeat = str(word.feature.lemma)
      if wstr in stopwords or wfeat in stopwords:
          continue
      s[wstr] = True
      s[wfeat] = True
      
    s = list(s.keys())
    dres = update_text_index(ccls, doc, s)
  