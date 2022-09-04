export function encodePath(path: string[]): string {
  return path.join('|');
}

export function decodePath(path: string): string[] {
  return path.split('|');
}

export function buildParams(params: Record<string, string | number | boolean | null | undefined>): string {
  const paramsFiltered: Record<string, string> = {};
  for (const k in params) {
    if (params[k] === undefined || params[k] === null)
      continue;
    else
      paramsFiltered[k] = String(params[k]);
  }
  return new URLSearchParams(paramsFiltered).toString();
}


export interface QueryResult<T> {
  status: number,
  result?: T,
  info: string, 
}


export async function getInfo<T>(scheme: string): Promise<QueryResult<T>> {
  const resRaw = await fetch(new Request(scheme));
  let json: T | undefined = undefined;
  const contentType = resRaw.headers.get('content-type');
  let info = resRaw.statusText;
  if (!contentType || !contentType.includes('application/json')) {
    json = undefined;
  } else if (contentType.includes('application/problems+json')) {
    const _probs = await resRaw.json();
    if (_probs.title != undefined)
      info = _probs.title;
  } else {
    json = await resRaw.json() as T;
  }
  const ret = {
    status: resRaw.status,
    result: json,
    info: info, 
  };
  // TODO what about other return format?
  return ret;
}