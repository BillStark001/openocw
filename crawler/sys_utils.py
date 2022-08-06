# -*- coding: utf-8 -*-
"""
Created on Sat Jun 12 03:15:49 2021

@author: billstark
"""

import json
import pickle
import os
import time
import shutil

from utils import *

illegal_char = '%\/:*?"<>|'
legal_char = [illegal_char[0] + hex(ord(x)).split('x')[-1] for x in illegal_char]

def ensure_dir(path: str) -> bool:
  dir: str = path
  if not os.path.isdir(path):
    dir = os.path.dirname(path)
  try:
    os.makedirs(dir)
  except FileExistsError:
    return False
    pass
  return True
  

def legalize_filename(ustr: str) -> str:
  for i in range(len(illegal_char)):
    ustr = ustr.replace(illegal_char[i], legal_char[i])
  return ustr

def restore_filename(ustr: str) -> str:
  ustr.replace(legal_char[-1], illegal_char[-1])
  for i in range(len(illegal_char[:-1])):
    ustr = ustr.replace(legal_char[i], illegal_char[i])
  return ustr
  
def jdump(obj, path: str, indent=2, ensure_ascii=False):
  ensure_dir(path)
  with open(path, 'w', encoding='utf-8') as f:
    json.dump(obj, f, indent=indent, ensure_ascii=ensure_ascii)
    
def jload(path: str):
  with open(path, 'r', encoding='utf-8') as f:
    ans = json.load(f)
  return ans

def pdump(obj, path: str):
  ensure_dir(path)
  with open(path, 'wb') as f:
    pickle.dump(obj, f)
    
def pload(path: str):
  with open(path, 'rb') as f:
    ans = pickle.load(f)
  return ans

backup_suffix = '.bak'
backup_file = lambda f: shutil.copyfile(f, f + backup_suffix)
clear_backup = lambda f: os.remove(f + backup_suffix)

if __name__ == '__main__':
  pass