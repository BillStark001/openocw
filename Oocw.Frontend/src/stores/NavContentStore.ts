
import { FilterSet } from "@/models/filter";
import { inject, InjectionKey, onMounted, provide, reactive } from "vue";

class NavContentStore {

  searchContent: string;
  pageNumber: number;
  filter: FilterSet;
  
  constructor() {
    this.searchContent = "";
    this.pageNumber = 0;
    this.filter = new FilterSet();
  }

  fetchInfo() {

  }
}


// provider

const defaultKey = Symbol('NavContentStore') as InjectionKey<NavContentStore>;

export function provideNavContentStore(key?: InjectionKey<NavContentStore>) {
  const store = new NavContentStore();
  provide(key || defaultKey, reactive(store));
  onMounted(() => store.fetchInfo());
  return store;
}

export function useNavContentStore(key?: InjectionKey<NavContentStore>) {
  const store = inject(key || defaultKey);
  if (!store) throw new Error('Failed to inject user store.');
  onMounted(() => store.fetchInfo());
  return store;
}