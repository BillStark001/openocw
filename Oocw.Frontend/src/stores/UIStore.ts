import { Settings } from "@/utils/settings";
import { inject, InjectionKey, onMounted, provide, reactive } from "vue";

class UIStore {
  isSideBarOn = false;
  darkMode = Settings.darkMode;

  showSideBar() {
    this.isSideBarOn = true;
  }

  hideSideBar() {
    this.isSideBarOn = false;
  }

  toggleDarkMode() {
    Settings.darkMode = !Settings.darkMode;
    this.darkMode = Settings.darkMode;
    this.updateDarkModeClass();
  }

  updateDarkModeClass() {
    if (this.darkMode) {
      document.documentElement.classList.add('dark-mode');
    } else {
      document.documentElement.classList.remove('dark-mode');
    }
  }
}


const key = Symbol('UIStore') as InjectionKey<UIStore>;

export function provideUIStore() {
  const store = new UIStore();
  provide(key, reactive(store));
  onMounted(() => {
    store.updateDarkModeClass();
  });
  return store;
}

export function useUIStore() {
  const store = inject(key);
  if (!store) throw new Error('Failed to UIStore.');
  return store;
}


export { UIStore }