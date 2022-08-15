from typing import Any
from sys_utils import time_id

# technical

KEY_META = '__meta__'
KEY_COMPLETE = 'compl'
KEY_DIRTY = 'dirty'
KEY_OCW_ID = 'ocwId'

KEY_UPD_TIME = 'upd'
KEY_UPD_TIME_SYL = 'updSyl'
KEY_UPD_TIME_NOTES = 'updNts'

KEY_SEARCH_REC = 'srec'
KEY_ACCESS_RANK = 'acrk'

# universal
KEY_CODE = 'code'
KEY_YEAR = 'year'

KEY_NAME = 'name'
KEY_CREDIT = 'credit'
KEY_ISLINK = 'isLink'
KEY_UNIT = 'unit'

KEY_CLASSES = 'classes'

KEY_MEDIA_USE = 'mediaUse'

# class-wise

KEY_CLASS_NAME = 'cname'
KEY_LECTURERS = 'lects'
KEY_FORMAT = 'form'
KEY_QUARTER = 'qurt'
KEY_ADDRESS = 'addr'
KEY_LANGUAGE = 'lang'

KEY_SYLLABUS = 'syl'
KEY_VERSION = 'ver'
VAL_VER_NONE = 'none'
VAL_VER_RAW = 'raw'
VAL_VER_PARSED = 'parsed'
KEY_NOTES = 'notes'

# syllabus related

KEY_SYL_DESC = 'desc'
KEY_SYL_OUTCOMES = 'outcomes'
KEY_SYL_KEYWORDS = 'keywords'
KEY_SYL_COMPETENCIES = 'competencies'
KEY_SYL_FLOW = 'flow'
KEY_SYL_SCHEDULE = 'schedule'
KEY_SYL_TEXTBOOKS = 'textbooks'
KEY_SYL_REF_MATS = 'refmats'
KEY_SYL_GRADING = 'grading'
KEY_SYL_REL_COURSES = 'rels'
KEY_SYL_PREREQUESITES = 'prereqs'
KEY_SYL_OOC_TIME = 'ooctime'
KEY_SYL_OTHER = 'other'
KEY_SYL_CONTACT = 'contact'
KEY_SYL_EXP_INST = 'exp_inst_courses'
KEY_SYL_OFF_HRS = 'offhrs'

def form_basic_record_scheme() -> dict[str, Any]:
  
  ans = {KEY_META: {}}
  ans_meta = ans[KEY_META]
  ans_meta[KEY_ACCESS_RANK] = 0.  # access_ranking
  ans_meta[KEY_SEARCH_REC] = []
  ans_meta[KEY_UPD_TIME] = time_id()
  ans_meta[KEY_DIRTY] = True
  

  ans[KEY_CODE] = ''
  ans[KEY_ISLINK] = False
  ans[KEY_NAME] = {}
  ans[KEY_CREDIT] = (0, 0, 0, 0)
  ans[KEY_UNIT] = {}
  ans[KEY_CLASSES] = []
  
  return ans

def form_class_record_scheme() -> dict[str, Any]:
  
  cls = {KEY_META: {}}
  cls_meta = cls[KEY_META]
  
  cls_meta[KEY_OCW_ID] = 197000000
  cls_meta[KEY_COMPLETE] = False
  cls_meta[KEY_DIRTY] = True
  cls_meta[KEY_UPD_TIME] = time_id()
  cls_meta[KEY_UPD_TIME_NOTES] = time_id()
  cls_meta[KEY_UPD_TIME_SYL] = time_id()

  cls[KEY_YEAR] = 1970
  cls[KEY_CODE] = ''
  cls[KEY_CLASS_NAME] = ''
  cls[KEY_LECTURERS] = []
  cls[KEY_FORMAT] = 0
  cls[KEY_QUARTER] = 0
  cls[KEY_ADDRESS] = []
  cls[KEY_LANGUAGE] = []

  cls[KEY_SYLLABUS] = {
    KEY_VERSION: VAL_VER_NONE
  }
  cls[KEY_NOTES] = {
    KEY_VERSION: VAL_VER_NONE
  }
  
  return cls

if __name__ == '__main__':
  pass
