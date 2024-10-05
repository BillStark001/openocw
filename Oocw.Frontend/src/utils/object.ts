export const isPureObject = (item: any): item is object => {
  return item && typeof item === 'object' && !Array.isArray(item);
};

export const deepAssign = <T extends object = object>(object: T, ...objects: Partial<T>[]): T => {
  return objects.reduce((result, object) => {
    Object.keys(object).forEach(key => {
      const resultValue = result[key as keyof T];
      const objectValue = object[key as keyof T];

      if (isPureObject(resultValue) && isPureObject(objectValue)) {
        result[key as keyof T] = deepAssign(resultValue, objectValue) as any;
      } else {
        result[key as keyof T] = objectValue as T[keyof T];
      }
    });

    return result;
  }, object) as T;
}