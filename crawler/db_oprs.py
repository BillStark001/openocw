from typing import Any, Dict, Optional, Tuple
from pymongo import MongoClient, WriteConcern, ReadPreference
from pymongo.client_session import ClientSession
from pymongo.read_concern import ReadConcern
from pymongo.collection import Collection

from utils import *
from sys_utils import *
from db_definitions import *

def init_db() -> MongoClient:
  uri = 'mongodb://localhost:27017/'
  cl = MongoClient(host=uri)
  return cl

def get_cols(cl: MongoClient) -> Tuple[Collection, Collection, Collection]:
  db = cl['openocw']
  col_crs = db['courses']
  col_cls = db['classes']
  col_fct = db['faculties']
  return col_crs, col_cls, col_fct

def put_course(db: MongoClient, sess: Optional[ClientSession], crs: Dict[str, Any]):
  ccrs, _, _ = get_cols(db)
  cid = crs[KEY_CODE]
  crs_old = ccrs.find_one({KEY_CODE: cid}, session=sess)
  
  crs_new: Dict[str, Any] = {}
  if crs_old is not None:
    # TODO timestamp concern
    crs_new = dict(crs_old)
    for k in crs:
      if k == KEY_CLASSES or k == KEY_META:
        continue
      crs_new[k] = crs[k]
    crs_new[KEY_CLASSES] += crs[KEY_CLASSES]
    list.sort(crs_new[KEY_CLASSES])
    crs_new[KEY_CLASSES] = list_clean_repeats(crs_new[KEY_CLASSES], lmb_identity)
  else:
    crs_new = dict(crs)
    
  crs_new[KEY_META][KEY_UPD_TIME] = datetime.datetime.utcnow()
  crs_new[KEY_META][KEY_DIRTY] = True
  if crs_old is not None:
    ccrs.update_one({KEY_CODE: cid}, {'$set': crs_new}, session=sess)
  else:
    ccrs.insert_one(crs_new, session=sess)
    
def put_class(db: MongoClient, sess: Optional[ClientSession], cls: Dict[str, Any]):
  ccrs, ccls, _ = get_cols(db)
  cond_scheme = {
    KEY_CODE: cls[KEY_CODE], 
    KEY_YEAR: cls[KEY_YEAR], 
    KEY_CLASS_NAME: cls[KEY_CLASS_NAME]
  }
  cls_old = ccls.find_one(cond_scheme, session=sess)
  cls_new: Dict[str, Any] = {}
  if cls_old is None:
    cls_new = dict(cls)
  else:
    cls_new = dict(cls_old)
    # TODO compare timestamp
    for k in cls:
      if k in [KEY_META, KEY_SYLLABUS, KEY_NOTES]:
        continue
      cls_new[k] = cls[k]
    oid = cls_old[KEY_META][KEY_OCW_ID]
    if oid != cls[KEY_META][KEY_OCW_ID]:
      # TODO may cause bug, try to put first
      ccrs.update_one(
        cond_scheme, 
        {'$set': {KEY_CLASS_NAME: cls_old[KEY_CLASS_NAME] + str(oid)}}, 
        session=sess)
    # maintain key_meta
    # TODO
    # maintain syllabus and note
    for k in cls[KEY_SYLLABUS]:
      cls_new[KEY_SYLLABUS][k] = cls[KEY_SYLLABUS][k]
    for k in cls[KEY_NOTES]:
      cls_new[KEY_NOTES][k] = cls[KEY_NOTES][k]
  
  cls_new[KEY_META][KEY_UPD_TIME] = datetime.datetime.utcnow()
  cls_new[KEY_META][KEY_DIRTY] = True
  
  cls_found = ccls.find_one_and_update(cond_scheme, {'$set': cls_new}, session=sess)
  if not cls_found:
    ccls.insert_one(cls_new, session=sess)
  
    
def put_faculty(db: MongoClient, sess: ClientSession, scheme: Dict[str, Any]):
  _, _, cfct = get_cols(db)
  fid = scheme[KEY_ID]
  cond_scheme = {KEY_ID: fid}
  fct_old = cfct.find_one(cond_scheme, session=sess)
  if fct_old is None:
    cfct.insert_one(scheme, session=sess)
  else:
    cfct.update_one(cond_scheme, {'$set': scheme})