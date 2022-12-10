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


  get isNoticeOn() {
    return localStorage.getItem('isNoticeOn') !== 'false';
  }

  set isNoticeOn(value: boolean) {
    localStorage.setItem('isNoticeOn', JSON.stringify(value));
  }

  get isLoggedIn() {
    if (localStorage.getItem('isLoggedIn') === 'true') return true;
    if (localStorage.getItem('isLoggedIn') === 'false') return false;
    return null;
  }

  set isLoggedIn(value: boolean | null) {
    localStorage.setItem('isLoggedIn', JSON.stringify(value));
  }

  constructor() {

  }

}

const Settings = new AppSettings();

export {
  Settings,
};