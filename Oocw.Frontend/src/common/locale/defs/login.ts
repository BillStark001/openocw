import { LanguageResource } from "../model";

export default {
  reg: {
    title: ["Register or Login", "アカウントログイン", "注册或登录账号"]
  },
  login: {
    title: ["Login", "ログイン", "登录"]
  },
  f: {
    uname: ["UserName: ", "ユーザー名", "用户名"],
    pwd: ["Password: ", "パスワード", "密码"],
    vcode: ["Verification Code:", "認証コート", "验证码"],
    'uname.hint': ["username", "ユーザー名", "输入用户名"],
    'pwd.hint': ["password", "パスワード", "输入密码"],
    'vcode.hint': ["code", "認証コート", "输入验证码"],
  },
  hint: {
    restr: [
      "The account name and password must contain at least six letters, digits, or symbols.",
      "ユーザー名とパスワードは6桁以上のアルファベット、数字、または記号で構成されていなければなりません。",
      "用户名和密码必须由6位及以上英文字母、数字或符号组成。"
    ],
    code: ["Code: ", "コード：", "代码："],
    info: ["Info: ", "提示：", "信息："],
    requesting: ["Requesting......", "請求中……", "正在请求后端……"]
  },
  btn: {
    reg: ["Register", "登録", "注册"],
    login: ["Login", "ログイン", "登录"],
    reset: ["Reset", "リセット", "重置"]
  }
} as LanguageResource;