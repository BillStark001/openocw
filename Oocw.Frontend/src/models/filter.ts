
interface FilterScheme {
  quarter: string;
  address: string;
  code: string;
  lang: string;
  unit: string;
}

export abstract class BaseFilter {
  
  constructor() {

  }

  abstract compile(): string;
}

export function concat(strs: Array<string>): string {
  return strs
    .map(x => x.replace(/,/g, '%2c'))
    .join(',');
}

export class SingleElementFilter extends BaseFilter {

  element: string;

  constructor(elem?: string) {
    super();
    this.element = elem || '';
  }

  compile(): string {
    return this.element;
  }

}

export class CourseCodeFilter extends BaseFilter {
  unitCode: string;
  divisionCode: string;
  level: string;
  rest: string;

  constructor() {
    super();
    this.unitCode = "";
    this.divisionCode = "";
    this.level = "";
    this.rest = "";
  }

  compile(): string {
    return `${this.unitCode}|${this.divisionCode}|${this.level}|${this.rest}`;
  }

}



export class FilterSet {

  quarter: Array<BaseFilter>;
  address: Array<BaseFilter>;
  code: Array<CourseCodeFilter>;
  lang: SingleElementFilter;
  unit: Array<SingleElementFilter>;

  constructor() {
    this.quarter = [];
    this.address = [];
    this.code = [];
    this.lang = new SingleElementFilter();
    this.unit = [];
  }

  compile(): FilterScheme {
    const ret = {
      quarter: concat(this.quarter.map(x => x.compile())),
      address: concat(this.address.map(x => x.compile())),
      code: concat(this.code.map(x => x.compile())),
      lang: this.lang.compile(), 
      unit: concat(this.unit.map(x => x.compile())),
    };
    return ret;
  }
}