# -*- coding: utf-8 -*-
"""
Created on Sat Jun 12 03:40:23 2021

@author: billstark
"""

import itertools
import re

from collections import defaultdict

rev_range = lambda x: range(x-1, -1, -1)

lmb_identity = lambda x: x
lmb_yield_none = lambda x: None

def btoi(bs):
    lbs = len(bs)
    ans = 0
    for b in bs:
        ans *= 2
        ans += int(b)
    return ans, lbs

def itob(i, lbs):
    ans = []
    for _ in range(lbs):
        ans = [i % 2 == 1] + ans
        i = i // 2
    return ans

def find_common_prefix(strs):
    if not strs: return ''
    
    l = len(strs[0])
    n = len(strs)
    for i in range(l):
        c = strs[0][i]
        if any(i == len(strs[j]) or strs[j][i] != c for j in range(1, n)):
            return strs[0][:i]
    
    return strs[0]

def find_common_suffix(strs):
    if not strs: return ''
    
    l = len(strs[0])
    n = len(strs)
    for i in range(l):
        c = strs[0][-i-1]
        if any(i == len(strs[j]) or strs[j][-i-1] != c for j in range(1, n)):
            return strs[0][(-i if i != 0 else l):]
    
    return strs[0]

# あるリスト型の要素だけ含むリストに対し、そのリスク全ての要素に対して足し算をする
# 例えば、[[1, 2, 3], [4, 5], [6, 7, 8, 9]] -> [1, 2, 3, 4, 5, 6, 7, 8, 9]
list_concat = lambda lists: list(itertools.chain(*lists))

# あるソート済みのリストに対し、そのリスクの繰り返しの要素を一つ（最も先の要素）だけ残す
# fはある要素に対して「同じである」ものであるかを判断するものを生成する関数である
# [(1, 0), (1, 1), (1, 2), (2, 0), (3, 0), (3, 1)], lambda x: x[0] -> [(1, 0), (2, 0), (3, 0)]
list_clean_repeats = lambda x, f: [x[0]] + [x[i] for i in range(1, len(x)) if f(x[i]) != f(x[i-1])] if len(x) > 1 else x

# 前の関数に似ている
# 但し、繰り返す要素を削除するではなく、他の要素で入れ替える
# fillは要素から入れ替えのものを生成する関数である
# [1, 1, 2, 3, 3, 3, 4], lmb_identity, lambda x: -x -> [1, -1, 2, 3, -3, -3, 4]
list_fill_repeats = lambda x, f, fill: [x[0]] + [x[i] if f(x[i]) != f(x[i-1]) else fill(x[i]) for i in range(1, len(x))] if len(x) > 1 else x

def first_unrepeated_name(s, it, i_init = 1, func=lambda s, i: '{}({})'.format(s, i)):
    i = i_init
    cur_name = s
    while cur_name in it:
        i += 1
        cur_name = func(s, i)
    return cur_name

def ltod(l, ignore_repeat=True):
    d = {}
    if ignore_repeat:
        return dict(l)
        # for k, v in l:
        #     d[k] = v
    else:
        c = defaultdict(int)
        for k, v in l:
            kk = k if not c[k] else (k, c[k])
            d[kk] = v
            c[k] += 1
    return d

def form_url(__url__, **kwargs):
    ans = str(__url__)
    first_mark = True
    for key in kwargs:
        ans += ('?' if first_mark else '&') + key + '=' + str(kwargs[key])
        if first_mark: first_mark = False
    return ans

def deform_url(addr_orig):
    try:
        addr, args = addr_orig.split('?')
    except:
        return addr_orig, {}
    args = [x.split('=') for x in args.split('&')]
    for i in range(len(args)):
        try:
            if not args[i][1].startswith('0'):
                args[i][1] = float(args[i][1])
                if float(args[i][1]) == int(args[i][1]): args[i][1] = int(args[i][1])
        except:
            pass
    return addr, ltod(args)

fix_web_str = lambda s: str(s).replace('\t', ' ').replace('\n', ' ').replace('\xa0', ' ').replace('\u3000', ' ').strip(' ')
first_elem = lambda d: d[list(d.keys())[0]] if len(d) > 0 else None

class node:
    def __init__(self, info={}, children={}):
        self.info = info
        self.children = children
        
    def __dict__(self):
        ans_children = {}
        for c in self.children:
            ans_children[c] = self.children[c].__dict__()
        return dict(info=self.info, children=ans_children)

def iter_tree(n, children=lambda n: n.children.values(), mode='dfs', pass_queue=False):
    if not pass_queue:
        queue = [n]
    else:
        queue = n
    if mode == 'dfs' or mode == 'DFS':
        while queue:
            ncur = queue.pop()
            l = list(children(ncur))
            l.reverse()
            queue += l
            yield ncur
    else:
        while queue:
            ncur = queue[0]
            queue = queue[1:]
            queue += list(children(ncur))
            yield ncur

class trietree:
    
    class trienode:
        def __init__(self, word=None, cend=0, cpref=0):
            self.chrc = word
            self.cnt_end = cend
            self.cnt_pref = cpref
            self.children = {}
            
        
    
    def __init__(self):
        self.root = self.trienode()
        
    def add_word(self, word):
        cur_node = self.root
        for i in range(len(word)):
            chrc = word[i]
            cadd = int(i == len(word) - 1)
            if not chrc in cur_node.children:
                new_node = self.trienode(chrc, cadd, 1)
                cur_node.children[chrc] = new_node
                cur_node = new_node
            else:
                cur_node = cur_node.children[chrc]
                cur_node.cnt_end += cadd
                cur_node.cnt_pref += 1
                
    def add_words(self, *words):
        for w in words: 
            self.add_word(w)
                
    def scan_common_word(self, ignore_conflict=False):
        ans = ''
        cur_node = self.root
        while True:
            chld = sorted(list(cur_node.children.values()), key=lambda x: (-x.cnt_pref, x.chrc))
            if not chld or (len(chld) > 1 and chld[0].cnt_pref == chld[1].cnt_pref and not ignore_conflict):
                break
            #print([x.chrc for x in chld])
            ans += chld[0].chrc
            cur_node = chld[0]
        return ans
        
def find_common_word(strs):
    T = trietree()
    T.add_words(*strs)
    return T.scan_common_word()

scw = find_common_word(['abccc', 'abaadd', 'abaaaab', 'abzz'])

split_with_sign = lambda s, sgn: list_concat([[x, sgn] for x in s.split(sgn)])[:-1]

def apart_str(s, signs, mono_brackets=['"'], dual_brackets=['()', '[]']):
    if isinstance(s, str): s = [s]
    # mono_brackets
    for mb in mono_brackets:
        s = list_concat([split_with_sign(x) for x in s])
     
strip_useless_elem = lambda l: [x for x in l if str(x) != '\n' and str(x) != '\xa0']    

if __name__ == '__main__':
    pass