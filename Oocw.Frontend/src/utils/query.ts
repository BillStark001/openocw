export function encodePath(path: string[]): string {
  return path.join('|');
}

export function decodePath(path: string): string[] {
  return path.split('|');
}

export function buildParams(params: Record<string, string | number | boolean | null | undefined>): string {
  const paramsFiltered: Record<string, string> = {};
  for (const k in params) {
    if (k === undefined || k === null)
      continue;
    else
      paramsFiltered[k] = String(params[k]);
  }
  return new URLSearchParams(paramsFiltered).toString();
}


export interface QueryResult<T> {
  status: number, 
  result?: T, 
}


export async function getInfo<T>(scheme: string): Promise<QueryResult<T>> {
  const resRaw = await fetch(new Request(scheme));
  const ret = {
    status: resRaw.status,
    result: (await resRaw.json()) as T, 
  };
  // TODO what about other return format?
  return ret;
}