import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import LanguageDetector from 'i18next-browser-languagedetector';

import zh from '../locales/zh-CN.json'
import ja from '../locales/ja-JP.json'
import en from '../locales/en.json'
const resources = {
  zh: {
    translation: zh
  },
  ja: {
    translation: ja
  },
  en: {
    translation: en
  },
};

i18n
.use(LanguageDetector) 
.use(initReactI18next) 
  .init({ 
    resources, 
    fallbackLng: "ja", 
    detection: {
      caches: ['localStorage', 'sessionStorage', 'cookie'],
    }
  })

export default i18n