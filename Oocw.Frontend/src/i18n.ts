import {
  createI18n,
} from 'vue-i18n';
import { Settings } from '@/utils/settings';
import locale from '@/common/locale';

const LANG_FALLBACK = 'en';

export const i18n = createI18n({
  legacy: false,
  locale: Settings.locale
    || LANG_FALLBACK
    || process.env.VUE_APP_I18N_LOCALE,
  fallbackLocale: LANG_FALLBACK
    || process.env.VUE_APP_I18N_FALLBACK_LOCALE,
  messages: locale as {},
  escapeParameter: true,
  // fallbackWarn: false,
  // keySeparator: false,
  flatJson: true,
  defaultKeyMsg: '_default',
});
