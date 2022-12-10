export function encodePath(path: string[]): string {
  return path.join('|');
}

export function decodePath(path: string): string[] {
  return path.split('|');
}


export type ParamsRecord = Record<
  string,
  string | number | boolean | null | undefined | number[] | string[]
>;

export function buildParams(params: ParamsRecord): string {
  const searchParams = new URLSearchParams();
  for (const key in params) {
    const value = params[key];
    if (value === undefined || value === null) {
      continue;
    } else {
      if (Array.isArray(value)) {
        value.map((v) => {
          searchParams.append(key, String(v));
        });
      } else {
        searchParams.append(key, String(params[key]));
      }
    }
  }
  return searchParams.toString();
}

export interface StandardResult {
  code: number;
  info: string;
}

export interface TimedResult extends StandardResult {
  time: string | Date | undefined;
}

export interface QueryResult<T> {
  status: number;
  result?: T;
  info: string;
}

function _buildUrl(
  path: string,
  params?: ParamsRecord) {

  path = path.replace(/^\//, '');

  let url = path;
  /*
  if (import.meta.env.VITE_SERVER_URL) {
    url = `${import.meta.env.VITE_SERVER_URL}/${path}`;
  } else {
    url = path;
  }
  */
  if (params) {
    const queryString = buildParams(params);
    if (queryString) {
      url = url + '?' + queryString;
    }
  }

  return url;
}

export async function getInfo<T>(
  path: string,
  params?: ParamsRecord
): Promise<QueryResult<T>> {
  return fetchServer(new Request(_buildUrl(path, params), {
    credentials: 'include',
  }));
}

export async function postInfo<T>(
  path: string,
  params?: ParamsRecord
): Promise<QueryResult<T>> {
  return fetchServer(
    new Request(_buildUrl(path, params), {
      credentials: 'include',
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(params || {}),
    })
  );
}

export async function fetchServer<T>(scheme: Request): Promise<QueryResult<T>> {
  const resRaw = await fetch(scheme);
  let json: T | undefined = undefined;
  const contentType = resRaw.headers.get('content-type');
  let info = resRaw.statusText;
  if (!contentType || !contentType.includes('application/json')) {
    json = undefined;
    info = await resRaw.text();
  } else if (contentType.includes('application/problems+json')) {
    const _probs = await resRaw.json();
    if (_probs.title != undefined) info = _probs.title;
  } else {
    json = (await resRaw.json()) as T;
  }
  const ret = {
    status: resRaw.status,
    result: json,
    info: info,
  };
  // TODO what about other return format?
  return ret;
}