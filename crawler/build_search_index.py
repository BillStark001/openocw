from db_definitions import *
from db_utils import *
from db_oprs import *

TEXT_INDEX_TARGET = f'{KEY_META}.{KEY_SEARCH_REC}'
TEXT_INDEX_DIRTY_SCHEME = {
  f'{KEY_META}.{KEY_DIRTY}': True
}

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


  
if __name__ == '__main__':
  db = init_db()
  create_index(db)
  
  ccrs, ccls, cfct = get_cols(db)