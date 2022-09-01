<template>
  <div id="locator">
    <RegisterPanel />
  </div>
  <PageFooter static="true"></PageFooter>
</template>

<script lang="ts">
import RegisterPanel from '@/components/RegisterPanel.vue';
import { defineComponent } from 'vue';
import PageFooter from '@/components/lesser/PageFooter.vue';

export default defineComponent({
  data() {
    return {};
  },
  created() {

  },
  components: {
    RegisterPanel,
    PageFooter
}
})
</script>

<style scoped>
#locator {
  position: fixed;
  left: 50%;
  top: 50%;
  transform: translate(-50%, -50%);
}
</style>