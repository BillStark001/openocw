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