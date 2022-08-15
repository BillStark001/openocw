from tqdm import tqdm

from sys_utils import *
from db_oprs import *
from db_utils import *



if __name__ == '__main__':
  # dd, dt, du = gather_data(path.savepath_details_raw)

  details, _ = pload(path.savepath_details_raw)
  keys = pload(path.savepath_details_keys)
  
  db = init_db()
  
  for oid in tqdm(details):
    if not isinstance(oid, int):
      continue
    scrs, scls = build_db_schemes(oid, details[oid], keys)
    
    with db.start_session() as sess:
      sess.with_transaction(
        lambda sess: put_course(sess.client, sess, scrs)
      )
      for cls in scls:
        sess.with_transaction(
          lambda sess: put_class(sess.client, sess, cls)
        )
  