<template>
  <PageFrame>
    <img src="@/assets/img/banner.jpg" class="banner">
    <div>

      <p>{{ t('product.name') }}</p>


      <button class="round-button s-hollow h" style="display: block;" v-on:click="changeDarkMode">
        {{ t('btn.ch_style') }}
      </button>
      
    </div>
  </PageFrame>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { useI18n } from 'vue-i18n'
import { Settings } from '@/utils/settings';
import PageFrame from '../components/PageFrame.vue'

export default defineComponent({
    name: "InfoPage",
    setup() {
        const { t } = useI18n({
            inheritLocale: true,
            useScope: "local"
        });
        // Something todo ..
        return { t };
    },
    components: { PageFrame }, 
    methods: {
      changeDarkMode() {
        (this.$root as any).setDarkMode(!Settings.darkMode);
      }
    }
})
</script>
