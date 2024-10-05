import { LanguageResource } from "../model";

export default {
  product: {
    name: "OpenOCW",
    version: "v0.1.0",
    developer: "Bill Stark"
  },
  navbar: {
    homepage: ["Home", "ホーム", "主页"],
    database: ["Database", "データベース", "数据库"],
    discussion: ["Discussion", "ディスカッション", "讨论"],
    info: ["Info.", "お問い合わせ", "信息"],
    user: ["User", "ユーザー", "用户"],
    search: ["Search", "検索", "搜索"]
  },
  searchbar: {
    search: {
      _default: ["Search", "検索", "搜索"],
      hint:["Type to search", "科目名・講師名・シラバス内容検索", "根据科目、讲师、关键词等搜索"]
    },
  },
  hp: {
    search: {
      "hint": ["Search by: ", " ", "按："],
      "kw": ["Keywords", "キーワード", "关键词"],
      "faculty": ["Faculty", "教員", "讲师"],
      "code": ["Course Code", "科目コード", "科目编号"],
      "name": ["Course Name", "科目名", "科目名称"],
      "hint.rear": [" ", "で探す", "搜索"]
    },
  },
  flt: {
    name: ["Filterer", "条件で探す", "筛选工具"],
    cond: {
      inc: ["Includes", "", "包含"],
      exc: ["Excludes", "", "不包含"],
      and: ["And", "かつ", "与"],
      or: ["Or", "または", "或"],
      not: ["Not", "", "非"],
      xor: ["Xor", "と", "异或"],
      group: ["Group", "グループ", "组"],
      set: ["Set", "集合", "集合"],
      eq: ["Equals", "は", "等于"],
      "inc.rear": ["", "を含む", ""],
      "exc.rear": ["", "を含まない", ""],
      "and.rear": ["", "", ""],
      "or.rear": ["", "", ""],
      "not.rear": ["", "のではない", ""],
      "xor.rear": ["", "のいずれか", ""],
      "eq.rear": ["", "に等しい", ""]
    },
    sel: {
      major: ["Major", "開講元", "授课单位"],
      quarter: ["Quarter", "クォーター", "学季"],
      day: ["Day", "曜日", "日历日"],
      time: ["Period", "時限", "时限"],
      lecturer: ["Lecturer", "講師", "讲师"],
      lvl: ["Level", "番台", "课程等级（番台）"],
      lang: ["Language", "言語", "语言"],
      "kw.name": ["Keywords(Course Name)", "キーワード（科目名）", "关键词（科目）"],
      "kw.lect": ["Keywords(Lecturer Name)", "キーワード（講師名）", "关键词（讲师）"]
    }
  },
  pos: {
    current: ["Currently at: ", "現在位置：", "当前位于："],
    root: ["Home", "ホーム", "主页"],
    db: ["Database", "データベース", "数据库"]
  },
  crsl: {
    "noitem.db": ["There are no courses under this unit.", "この開講元には授業を開設していません。", "该单位未开设任何课程。"],
    "noitem.faculty": ["The faculty you selected have no lectures.", "この教員は担当されていた授業がありません。", "该教职工未担任任何课程的授课教师。"],
    "noitem.search": ["There are no courses match your query.", "あなたの検索結果に一致する授業はありません。", "没有与您的搜索一致的项目。"],
    "noitem.error": ["Your query is responded with an errorr: ", "エラーが発生しました：", "您的请求返回了一个异常："]
  }
} as LanguageResource;