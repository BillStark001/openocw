<template>
  <PageFrame>
    <PageBanner />
    <div>

      <p>{{ t('product.name') }}</p>


      <button class="round-button s-hollow h" style="display: block;" v-on:click="changeDarkMode">
        {{ t('btn.ch_style') }}
      </button>

    </div>
  </PageFrame>
  <PageFooter />
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';
import PageFrame from '../components/PageFrame.vue'
import PageBanner from '@/components/lesser/PageBanner.vue';
import PageFooter from '@/components/lesser/PageFooter.vue';
import { useUIStore } from '@/stores/UIStore';
const { t } = useI18n({
  inheritLocale: true,
  useScope: "local"
});
function changeDarkMode() {
  (useUIStore().toggleDarkMode());
}
</script>
