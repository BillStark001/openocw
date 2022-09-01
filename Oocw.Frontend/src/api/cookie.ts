// in case https is disabled
// reference: https://www.runoob.com/js/js-cookies.html

function setCookieRaw(key: string, value: string, expiryDays: number) {
  const d = new Date();
  d.setTime(d.getTime() + (expiryDays * 24 * 60 * 60 * 1000));
  const expires: string = "expires=" + d.toUTCString();
  document.cookie = key + "=" + value + "; " + expires;
}

function getCookieRaw(key: string) {
  const name = key + "=";
  const ca = document.cookie.split(';');
  for (let i = 0; i < ca.length; i++) {
    const c = ca[i].trim();
    if (c.indexOf(name) == 0) 
      return c.substring(name.length, c.length);
  }
  return '';
}

function isNullOrWhiteSpace(s: string | null | undefined) {
  return s === null || s === undefined || s.trim() === '';
}
