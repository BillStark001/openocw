export class AppSettings {

  get locale(): string {
    return localStorage.locale ? String(localStorage.locale) : navigator.language;
  }
  set locale(value: string) {
    localStorage.locale = value;
  }

  get darkMode(): boolean {
    if (String(localStorage.darkMode).toLowerCase() == 'false')
      return false;
    return !!localStorage.darkMode;
  }
  set darkMode(value: boolean) {
    localStorage.darkMode = value;
  }
  
  constructor() {

  }

}

const Settings = new AppSettings();

export {
  Settings, 
};