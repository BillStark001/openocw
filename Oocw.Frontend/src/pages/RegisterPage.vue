<template>
  <div id="locator">
    <RegisterPanel />
  </div>
  <div id="placeholder">

  </div>
</template>

<script lang="ts">
import RegisterPanel from '@/components/RegisterPanel.vue';
import { defineComponent } from 'vue';

export default defineComponent({
  data() {
    return {};
  },
  created() {

  },
  components: {
    RegisterPanel,
  }
})
</script>

<style>
#locator {
  position: fixed;
  left: 50%;
  top: 50%;
  transform: translate(-50%, -50%);
}
#placeholder {
  height: calc(100vh - var(--navbar-height) - var(--footer-height));
  display: block;
}
</style>