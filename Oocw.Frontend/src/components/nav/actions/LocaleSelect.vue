<template>
  <NavItem>
    <img src="@/assets/svg/lang.svg" />
    <template v-slot:dropdown>
      <div class="submenu align-right">
        <div class="item-list">
          <a href="javascript: void(0);" style="font-size: 12px; line-height: 20px" v-on:click="changeLocale()">
            <p>{{ t('lang.detect') }}</p>
            <p>{{ t('lang.current') + t('lang.name') }}</p>
          </a>
          <a :class="{ active: locale === 'en' }" href="javascript: void(0);" v-on:click="changeLocale('en')">{{
            t('lang.en') }}</a>
          <a :class="{ active: locale === 'zh-CN' }" href="javascript: void(0);" v-on:click="changeLocale('zh-CN')">{{
            t('lang.zh-CN') }}</a>
          <a :class="{ active: locale === 'ja-JP' }" href="javascript: void(0);" v-on:click="changeLocale('ja-JP')">{{
            t('lang.ja-JP') }}</a>
        </div>
      </div>
    </template>
  </NavItem>
</template>

<script setup lang="ts">

import NavItem from '@/components/base/NavItem.vue';
import { useI18n } from 'vue-i18n';

const { t, locale } = useI18n();

function changeLocale(lang?: string) {
  if (!lang) {
    locale.value = navigator.language;
  } else {
    locale.value = lang;
  }
  // Settings.locale = locale.value;
}

</script>