export interface ISet<T> {
  contains(val: T): boolean;
}

export interface IEquatable<T> {
  equals(t: T): boolean;
}

declare global {
  interface String extends IEquatable<String> {
    equals(t: String): boolean;
  }
  interface Number extends IEquatable<String> {
    equals(t: Number): boolean;
  }
}
String.prototype.equals = function(t) { return t === String(this); }
Number.prototype.equals = function(t) { return t === Number(this); }

export interface ICondition<T> {
  eval(t: T): boolean;
}

export class IncludeCondition<T> implements ICondition<T> {
  _set: ISet<T>;
  get set() { return this._set; }
  constructor(set: ISet<T>) {
    this._set = set;
  }
  eval(t: T) {
    return this._set.contains(t);
  }
}

export class EqualCondition<T extends IEquatable<T>> implements ICondition<T> {
  _val: T;
  get val() { return this._val; }
  constructor(val: T) {
    this._val = val;
  }
  eval(t: T) {
    return this._val.equals(t);
  }
}

export type BinaryLogicalExpression = (a: boolean, b: boolean) => boolean;

export abstract class LogicalCondition<T> implements ICondition<T> {
  _c1: ICondition<T>;
  _c2: ICondition<T>;
  get cond1() { return this._c1; }
  get cond2() { return this._c2; }
  constructor(c1: ICondition<T>, c2: ICondition<T>) {
    this._c1 = c1;
    this._c2 = c2;
  }
  abstract evalInner(c1: boolean, c2: boolean): boolean;
  eval(t: T) {
    return this.evalInner(this._c1.eval(t), this._c2.eval(t));
  }
}
  
export class AndCondition<T> extends LogicalCondition<T> {
  override evalInner(c1: boolean, c2: boolean): boolean {
      return c1 && c2;
  }
}

export class OrCondition<T> extends LogicalCondition<T> {
  override evalInner(c1: boolean, c2: boolean): boolean {
      return c1 || c2;
  }
}

export class XorCondition<T> extends LogicalCondition<T> {
  override evalInner(c1: boolean, c2: boolean): boolean {
      return (c1 && !c2) || (!c1 && c2);
  }
}

export class NotCondition<T> implements ICondition<T> {
  _c: ICondition<T>;
  get cond() {return this._c;}
  constructor(c: ICondition<T>) {
    this._c = c;
  }
  eval(t: T): boolean {
    return !this._c.eval(t);
  }
}


