import { createI18n, LocaleMessages, VueMessageType, useI18n, UseI18nOptions, ComposerTranslation } from 'vue-i18n'
import type { RemoveIndexSignature } from '@intlify/core-base';
import { Settings } from './utils/settings';

/**
 * Load locale messages
 *
 * The loaded `JSON` locale messages is pre-compiled by `@intlify/vue-i18n-loader`, which is integrated into `vue-cli-plugin-i18n`.
 * See: https://github.com/intlify/vue-i18n-loader#rocket-i18n-resource-pre-compilation
 */
function loadLocaleMessages(): LocaleMessages<VueMessageType> {
  const locales = require.context('./locales', true, /[A-Za-z0-9-_,\s]+\.json$/i);
  const messages: LocaleMessages<VueMessageType> = {

  };
  locales.keys().forEach(key => {
    const matched = key.match(/([A-Za-z0-9-_]+)\./i);
    if (matched && matched.length > 1) {
      const locale = matched[1];
      messages[locale] = locales(key).default;
    }
  });
  return messages;
}

const msgs = loadLocaleMessages();
const LANG_FALLBACK = 'en';

export const i18n = createI18n({
  sync: true, 
  legacy: false,
  locale: Settings.locale || process.env.VUE_APP_I18N_LOCALE || LANG_FALLBACK,
  fallbackLocale: process.env.VUE_APP_I18N_FALLBACK_LOCALE || LANG_FALLBACK,
  messages: msgs as {} // this is a HACK that prevents type error
});

export interface WithTranslation {
  t: ComposerTranslation<LocaleMessages<VueMessageType>, string, RemoveIndexSignature<LocaleMessages<VueMessageType>>>;
}

export function changeLocale(lang?: string) {
  if (!lang) {
    i18n.global.locale.value = navigator.language;
  } else {
    i18n.global.locale.value = lang;
  }
  Settings.locale = i18n.global.locale.value;
}

export {
  useI18n
};