<template>
  <div class="page-frame-wrapper">
    <div class="page-frame" ref="pageFrameRef">
      <slot></slot>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue';

interface PageFrameData {
  width: number;
  height: number;
}

const pageFrameRef = ref<HTMLElement | null>(null);
const frameData = ref<PageFrameData>({
  width: 1440,
  height: 600
});

const updateData = () => {
  if (pageFrameRef.value) {
    frameData.value.width = pageFrameRef.value.clientWidth;
    frameData.value.height = pageFrameRef.value.clientHeight;
  }
};

onMounted(() => {
  window.addEventListener('resize', updateData);
  updateData();
});

onUnmounted(() => {
  window.removeEventListener('resize', updateData);
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
  backdrop-filter: blur(10px);
}
</style>