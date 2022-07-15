import util from 'util';
import i18next from 'i18next';
import {initReactI18next, useTranslation} from 'react-i18next';
import XHR from 'i18next-xhr-backend';
import LanguageDetector from 'i18next-browser-languagedetector';

i18next.use(XHR).use(LanguageDetector).use(initReactI18next).init({
    backend: {
        loadPath:'./locales/{{lng}}.json'
    },
    react: {
        useSuspense: true
    },
    fallbackLng: 'en',
    preload: [
        'en', 'zh-cn', 'ja'
    ],
    keySeparator: false,
    interpolation: {
        escapeValue: false
    }
});

const t = (x: any) => x;//useTranslation();

function L(key: string, ...params: any[]) {
    var loc = t(key); // TODO
    return util.format(loc, ...params);
}

export default L;
