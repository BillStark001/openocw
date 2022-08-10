from typing import Any

# technical

KEY_META = '__meta__'
KEY_COMPLETE = 'compl'
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
KEY_NOTES = 'notes'

def form_basic_record_scheme() -> dict[str, Any]:
  
  ans = {KEY_META: {}}
  ans_meta = ans[KEY_META]
  ans_meta[KEY_ACCESS_RANK] = 0.  # access_ranking
  ans_meta[KEY_UPD_TIME] = 0

  ans[KEY_CODE] = ''
  ans[KEY_YEAR] = 1970
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
  cls_meta[KEY_UPD_TIME] = 0
  cls_meta[KEY_UPD_TIME_NOTES] = 0
  cls_meta[KEY_UPD_TIME_SYL] = 0

  cls[KEY_CLASS_NAME] = ''
  cls[KEY_LECTURERS] = []
  cls[KEY_FORMAT] = 0
  cls[KEY_QUARTER] = 0
  cls[KEY_ADDRESS] = []
  cls[KEY_LANGUAGE] = []

  cls[KEY_SYLLABUS] = {}
  cls[KEY_NOTES] = {}
  
  return cls

if __name__ == '__main__':
  pass
