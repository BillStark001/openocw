<template>
  <PageFrame>
    <PageBanner/>
    <div>

      <p>{{ t('product.name') }}</p>


      <button class="round-button s-hollow h" style="display: block;" v-on:click="changeDarkMode">
        {{ t('btn.ch_style') }}
      </button>
      
    </div>
  </PageFrame>
  <PageFooter/>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { useI18n } from 'vue-i18n'
import { Settings } from '@/utils/settings';
import PageFrame from '../components/PageFrame.vue'
import PageBanner from '@/components/lesser/PageBanner.vue';
import PageFooter from '@/components/lesser/PageFooter.vue';

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
    components: { 
      PageFrame, 
      PageBanner, 
      PageFooter 
    }, 
    methods: {
      changeDarkMode() {
        (this.$root as any).setDarkMode(!Settings.darkMode);
      }
    }
})
</script>
