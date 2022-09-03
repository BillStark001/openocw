import { buildParams, QueryResult, getInfo } from "@/utils/query";

export interface LecturerInfo {
  id: number,
  name: string
}

export interface CourseBrief {
  id: string,
  name: string,
  className: string,
  tags: string[],
  lecturers: LecturerInfo[],
  description: string,
  imageLink?: string,
}

export interface FacultyBrief {
  name: string,
  names: Record<string, string>,
}

export function defaultCourseBrief(): CourseBrief {
  return {
    id: "AAA.B123",
    name: "Test Course",
    className: "what the hell is this?",
    tags: ["tag1", "标签2", "タグ3"],
    lecturers: [
      { id: 114514, name: "Tadokoro Koji" },
      { id: 1919810, name: "Yajuu Senpai" },
    ],
    description: "Test English\n日本語文字テスト\n中文文本测试",
  }
}

// info

export async function getCourseInfo(id: string, year?: number, className?: string, lang?: string): Promise<QueryResult<string>> {
  const scheme = `/api/info/course/${encodeURIComponent(id)}?` + buildParams({
    year: year, className: className, lang: lang,
  });
  return await getInfo<string>(scheme);
}

export async function getCourseBrief(id: string, year?: number, className?: string, lang?: string): Promise<QueryResult<CourseBrief>> {
  const scheme = `/api/info/coursebrief/${encodeURIComponent(id)}?` + buildParams({
    year: year, className: className, lang: lang,
  });
  return await getInfo<CourseBrief>(scheme);
}

export async function getFacultyBrief(id: number, lang?: string): Promise<QueryResult<FacultyBrief>> {
  const scheme = `/api/info/faculty/${encodeURIComponent(id)}?` + buildParams({
    lang: lang,
  });
  return await getInfo<FacultyBrief>(scheme);
}

// list

export async function getCourseListByDepartment(
  id: string,
  year?: number,
  byclass?: boolean,
  lang?: string,
  sort?: string,
  filter?: string
): Promise<QueryResult<CourseBrief[]>> {
  const scheme = `/api/list/dept/${encodeURIComponent(id)}?`
    + buildParams({
      year: year,
      byclass: byclass,
      lang: lang,
      sort: sort,
      filter: filter,
    });
  return await getInfo<CourseBrief[]>(scheme);
}

export async function getCourseListByFaculty(
  id: number,
  year?: number,
  byclass?: boolean,
  lang?: string,
  sort?: string,
  filter?: string
): Promise<QueryResult<CourseBrief[]>> {
  const scheme = `/api/list/faculty/${encodeURIComponent(id)}?`
    + buildParams({
      year: year,
      byclass: byclass,
      lang: lang,
      sort: sort,
      filter: filter,
    });
  return await getInfo<CourseBrief[]>(scheme);
}

// search

export interface SearchScheme extends Record<string, string | number | undefined> {
  queryStr: string, 

  restrictions?: string, 
  sort?: string, 
  filter?: string, 

  dispCount?: number, 

  page?: number, 
}

export async function getSearchResult(
  mode: 'faculty', 
  scheme: SearchScheme, 
  lang?: string, 
): Promise<QueryResult<FacultyBrief[]>>;

export async function getSearchResult(
  mode: 'class' | 'course', 
  scheme: SearchScheme, 
  lang?: string, 
): Promise<QueryResult<CourseBrief[]>>;

export async function getSearchResult(
  mode: unknown, 
  scheme: SearchScheme, 
  lang?: string, 
): Promise<unknown> {
  const url = `/api/search/${mode}?` + buildParams(Object.assign(scheme, {lang: lang}));
  return await getInfo<undefined>(url);
}
