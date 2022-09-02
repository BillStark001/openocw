export function encodePath(path: string[]): string {
  return path.join('|');
}

export function decodePath(path: string): string[] {
  return path.split('|');
}