import {
  createI18n,
  LocaleMessages,
  VueMessageType,
  useI18n,
  ComposerTranslation,
} from 'vue-i18n';
import type { RemoveIndexSignature } from '@intlify/core-base';
import { Settings } from '@/utils/settings';
import en from './locales/en.json';
import jaJP from './locales/ja-JP.json';
import zhCN from './locales/zh-CN.json';

const LANG_FALLBACK = 'en';

export const i18n = createI18n({
  sync: true,
  legacy: false,
  locale: Settings.locale || LANG_FALLBACK || process.env.VUE_APP_I18N_LOCALE,
  fallbackLocale: LANG_FALLBACK || process.env.VUE_APP_I18N_FALLBACK_LOCALE,
  fallbackWarn: false,
  allowComposition: true,
  messages: {
    en,
    'ja-JP': jaJP,
    'zh-CN': zhCN,
  } as {}, // this is a HACK that prevents type error
});

export interface WithTranslation {
  t: ComposerTranslation<
    LocaleMessages<VueMessageType>,
    string,
    RemoveIndexSignature<LocaleMessages<VueMessageType>>
  >;
}

export function changeLocale(lang?: string) {
  if (!lang) {
    i18n.global.locale.value = navigator.language;
  } else {
    i18n.global.locale.value = lang;
  }
  Settings.locale = i18n.global.locale.value;
}

export { useI18n };