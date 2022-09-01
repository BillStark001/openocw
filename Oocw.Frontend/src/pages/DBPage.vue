<template>
  <PageFrame>
    <PageBanner/>
    <RouteDisplay></RouteDisplay>
    <p>Stats Page</p>
    <NavList source="@/assets/orgtree.json"></NavList>
  </PageFrame>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { useI18n } from 'vue-i18n'
import PageFrame from '../components/PageFrame.vue'
import RouteDisplay from '@/components/RouteDisplay.vue';
import NavList from '@/components/NavList.vue';
import PageBanner from '@/components/lesser/PageBanner.vue';

export default defineComponent({
  name: "DBPage",
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
    RouteDisplay,
    NavList,
    PageBanner
},
  methods: {

  }
})
</script>
