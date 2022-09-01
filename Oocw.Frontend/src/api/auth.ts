import 'cookie-store';

export type AuthResult = {
  code: number,
  info: string
}

export async function requestRegister(uname: string, pwd: string): Promise<AuthResult> {
  try {
    const req = new Request('/api/user/register/', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ uname: uname, pwd: pwd })
    });
    const res = await fetch(req);
    const json = await res.json();
    return json as AuthResult;
  }
  catch (e: any) {
    return { code: -1, info: (e as Error).message };
  }
}

export async function requestLogin(uname: string, pwd: string): Promise<AuthResult> {
  try {
    const req = new Request('/api/user/login/', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ uname: uname, pwd: pwd })
    });
    const res = await fetch(req);
    const json = await res.json();
    return json as AuthResult;
  }
  catch (e: any) {
    return { code: -1, info: (e as Error).message };
  }
}

const COOKIE_REFRESH_KEY = '.Oocw.Token.Refresh';
const COOKIE_ACCESS_PREFIX = '.Oocw.Token.Access';

export async function isLoggedIn(): Promise<boolean> {
  const cookie = await window.cookieStore.get(COOKIE_REFRESH_KEY);
  if (cookie) {
    const res = await fetch(new Request('/api/user/status'));
    const json = await res.json() as AuthResult;
    if (json.code == 0)
      return true;
  }
  return false;
}

export async function tryLogOut(): Promise<void> {
  // looks useless due to the policy?
  await window.cookieStore.delete(COOKIE_REFRESH_KEY);
  await window.cookieStore.delete(COOKIE_ACCESS_PREFIX);
  /*
  const res = await fetch(new Request('/api/user/logout'));
  const json = await res.json() as AuthResult;
  return json.code == 0;
  */
}