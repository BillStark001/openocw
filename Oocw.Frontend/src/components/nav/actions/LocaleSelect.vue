<template>
  <NavItem>
    <img src="@/assets/svg/lang.svg" />
    <template v-slot:dropdown>
      <div class="submenu align-right">
        <div class="item-list">
          <a
            href="javascript: void(0);"
            style="font-size: 12px; line-height: 20px"
            v-on:click="changeLocale()"
          >
            <p>{{ t('lang.detect') }}</p>
            <p>{{ t('lang.current') + t('lang.name') }}</p>
          </a>
          <a
            :class="{ active: locale === 'en' }"
            href="javascript: void(0);"
            v-on:click="changeLocale('en')"
            >{{ t('lang.en') }}</a
          >
          <a
            :class="{ active: locale === 'zh-CN' }"
            href="javascript: void(0);"
            v-on:click="changeLocale('zh-CN')"
            >{{ t('lang.zh-CN') }}</a
          >
          <a
            :class="{ active: locale === 'ja-JP' }"
            href="javascript: void(0);"
            v-on:click="changeLocale('ja-JP')"
            >{{ t('lang.ja-JP') }}</a
          >
        </div>
      </div>
    </template>
  </NavItem>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import { useI18n, changeLocale } from '@/i18n';
import NavItem from '@/components/base/NavItem.vue';
export default defineComponent({
  setup() {
    const { t, locale } = useI18n({ useScope: 'global' });
    return { t, locale };
  },
  components: {
    NavItem,
  },
  methods: {
    changeLocale(lang?: string) {
      changeLocale(lang);
    },
  },
});
</script>