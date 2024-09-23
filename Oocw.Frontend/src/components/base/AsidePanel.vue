<template>
  <div class="aside-wrapper" ref="root">
    <div ref="backdrop" class="backdrop" @click.self="$emit('hide')" />
    <aside ref="aside">
      <slot />
    </aside>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import anime from 'animejs';

const props = defineProps<{
  show?: boolean
}>();

const root = ref<HTMLDivElement | null>(null);
const backdrop = ref<HTMLDivElement | null>(null);
const aside = ref<HTMLElement | null>(null);

function showAside() {
  if (!root.value) {
    return;
  }
  root.value.style.display = 'block';
  root.value.style.pointerEvents = 'auto';
  anime({
    targets: backdrop.value,
    opacity: [0, 1],
    easing: 'easeOutQuart',
    duration: 300,
  });
  anime({
    targets: aside.value,
    opacity: [0.7, 1],
    translateX: ['-100%', '0%'],
    easing: 'easeOutQuart',
    duration: 300,
  });
}
function hideAside(duration: number = 300, duration2: number = 300) {
  if (!root.value) {
    return;
  }
  root.value.style.pointerEvents = 'none';
  anime({
    targets: backdrop.value,
    opacity: [1, 0],
    easing: 'easeOutQuart',
    duration,
    complete: () => {
      root.value!.style.display = 'block';
    },
  });
  anime({
    targets: aside.value,
    opacity: [1, 0.7],
    translateX: ['0%', '-100%'],
    easing: 'easeOutQuart',
    duration: duration2,
  });
}


watch(() => props.show, (value = false, oldValue: boolean) => {
  if (value === oldValue) return;
  value ? showAside() : hideAside();
});


</script>

<style scoped>
.aside-wrapper {
  position: fixed;
  width: 100%;
  height: 100%;
  z-index: 10;
  display: none;
}

.backdrop {
  position: absolute;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.7);
  backdrop-filter: blur(3px);
}

aside {
  position: fixed;
  top: 0;
  left: 0;
  width: 280px;
  height: 100%;
  background-color: var(--color-bg2);
  color: var(--color-txt2);
  z-index: 100;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.3);
  overflow: auto;
}
</style>