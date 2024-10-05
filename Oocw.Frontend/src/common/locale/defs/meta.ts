import { LanguageResource } from "../model";


export default {
  lang: {
    key: ['Language', '言語', '语言'],
    name: ["English", "日本語", "中文（大陆简体）"],
    en: ["English", "英語", "英文"],
    "ja-JP": ["Japanese", "日本語", "日文"],
    "zh-CN": ["Chinese(Simplified)", "中国語（簡体字）", "中文（大陆简体）"],
    detect: ["Follow System", "システムの設定と一致", "与系统一致"],
    current: ["Current: ", "現在：", "正在使用："]
  },
  id: {
    bachelor: "Bachelor",
    master: "Master",
    doctor: "Doctor",
    "master.prof": ["Professional master", "", "专业硕士"],
    student: "Student",
    faculty: ["Faculty", "教員", "教职员工"]
  },
  notice: {
    intest: {
      shorter: [
        "Note: At present, this service is still in the development stage, do not guarantee the security of information, the stability of the service!",
        "注意:本サービスはまだ開発段階にあり、情報の安全性、サービスの安定性を保証するものではありません。",
        "注意：目前本服务仍处于开发阶段，不保证信息的安全性、服务的稳定性！"
      ]
    }
  },
  meta: {
    uncat: ["Uncategorized", "未分類", "未分类"]
  },
  ay: ['AY {{ay}}', '{{ay}} 年度', '{{ay}} 年度'],
  date: {
    md: ['MMM D', 'M月D日', 'M月D日'],
  },
  day: {
    1: ["Monday", "月曜日", "周一"],
    2: ["Tuesday", "火曜日", "周二"],
    3: ["Wednesday", "水曜日", "周三"],
    4: ["Thursday", "木曜日", "周四"],
    5: ["Friday", "金曜日", "周五"],
    6: ["Saturday", "土曜日", "周六"],
    7: ["Sunday", "日曜日", "周日"],
    "1b": ["Mon.", "月", "一"],
    "2b": ["Tue.", "火", "二"],
    "3b": ["Wed.", "水", "三"],
    "4b": ["Thur.", "木", "四"],
    "5b": ["Fri.", "金", "五"],
    "6b": ["Sat.", "土", "六"],
    "7b": ["Sun.", "日", "日"]
  },
  time: {
    period: {
      unary: ["Period {0}", "{0}限", "时段{0}"],
      binary: ["Period {0}-{1}", "{0}-{1}限", "时段{0}-{1}"]
    }
  }
} as LanguageResource;