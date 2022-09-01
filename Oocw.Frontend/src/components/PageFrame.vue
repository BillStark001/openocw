<template>

  <div class="page-frame-wrapper">
    <div class="page-frame" id="pf">
      <slot></slot>
    </div>


  </div>


</template>

<script lang="tsx">
import { defineComponent } from 'vue';

export interface PageFrameData {
  width: number;
  height: number;
}

export default defineComponent({
  data(): PageFrameData {
    return {
      width: 1440, 
      height: 600
    };
  }, 
  methods: {
    updateData() {
      const el = this.$el.querySelector('#pf');
      if (el) {
        this.width = el.clientWidth;
        this.height = el.clientHeight;
      }
    }
  }, 
  mounted() {
    window.addEventListener('resize', this.updateData);
    this.updateData();
  }, 
  beforeUnmount() {
    window.removeEventListener('resize', this.updateData);
  }
});


</script>

<style scoped>
.page-frame-wrapper {
  width: 100%;
  position: relative;
}

.page-frame {
  max-width: var(--page-content-max-width);
  min-width: var(--page-content-min-width);
  min-height: var(--page-content-min-height);
  background-color: var(--color-bg-trs);
  margin: auto;
  color: var(--color-txt1);
}
</style>