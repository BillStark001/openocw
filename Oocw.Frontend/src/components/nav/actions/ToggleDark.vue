<template>
  <NavItem @click="toggleDarkMode">
    <svg
      ref="svgRef"
      xmlns="http://www.w3.org/2000/svg"
      width="24"
      height="24"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      stroke-width="2"
      stroke-linecap="round"
      stroke-linejoin="round"
    >
      <mask :id="maskId">
        <rect x="0" y="0" width="100%" height="100%" fill="white" />
        <circle ref="maskedCircleRef" r="9" fill="black" />
      </mask>
      <circle
        ref="centerCircleRef"
        fill="currentColor"
        cx="12"
        cy="12"
        :mask="`url(#${maskId})`"
      />
      <g ref="linesRef" stroke="currentColor">
        <line x1="12" y1="1" x2="12" y2="3" />
        <line x1="12" y1="21" x2="12" y2="23" />
        <line x1="4.22" y1="4.22" x2="5.64" y2="5.64" />
        <line x1="18.36" y1="18.36" x2="19.78" y2="19.78" />
        <line x1="1" y1="12" x2="3" y2="12" />
        <line x1="21" y1="12" x2="23" y2="12" />
        <line x1="4.22" y1="19.78" x2="5.64" y2="18.36" />
        <line x1="18.36" y1="5.64" x2="19.78" y2="4.22" />
      </g>
    </svg>
  </NavItem>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import anime from 'animejs';
import { Settings } from '@/utils/settings';
import NavItem from '@/components/base/NavItem.vue';
import { useUIStore } from '@/stores/UIStore';

const properties = {
  light: {
    svg: { rotate: '90deg' },
    maskedCircle: { cx: 30, cy: 0 },
    centerCircle: { r: 5 },
    lines: { opacity: 1 },
  },
  dark: {
    svg: { rotate: '40deg' },
    maskedCircle: { cx: 12, cy: 4 },
    centerCircle: { r: 9 },
    lines: { opacity: 0 },
  },
  common: {
    easing: 'easeInOutQuart',
    duration: 300,
  },
};

const getProperty = (isDarkMode: boolean) => 
  isDarkMode ? properties.dark : properties.light;

const maskId = ref(`mask_${Date.now()}`);
const uiStore = useUIStore();

const svgRef = ref<SVGElement | null>(null);
const maskedCircleRef = ref<SVGCircleElement | null>(null);
const centerCircleRef = ref<SVGCircleElement | null>(null);
const linesRef = ref<SVGGElement | null>(null);

onMounted(() => {
  const isDarkMode = Settings.darkMode;
  const property = getProperty(isDarkMode);

  if (svgRef.value) {
    svgRef.value.setAttribute('style', `transform: rotate(${property.svg.rotate})`);
  }
  if (maskedCircleRef.value) {
    maskedCircleRef.value.setAttribute('cx', String(property.maskedCircle.cx));
    maskedCircleRef.value.setAttribute('cy', String(property.maskedCircle.cy));
  }
  if (centerCircleRef.value) {
    centerCircleRef.value.setAttribute('r', String(property.centerCircle.r));
  }
  if (linesRef.value) {
    linesRef.value.setAttribute('opacity', String(property.lines.opacity));
  }
});

const toggleDarkMode = () => {
  const endProperty = getProperty(!Settings.darkMode);

  anime({
    targets: svgRef.value,
    rotate: endProperty.svg.rotate,
    ...properties.common,
  });
  anime({
    targets: maskedCircleRef.value,
    cx: endProperty.maskedCircle.cx,
    cy: endProperty.maskedCircle.cy,
    ...properties.common,
  });
  anime({
    targets: centerCircleRef.value,
    r: endProperty.centerCircle.r,
    ...properties.common,
  });
  anime({
    targets: linesRef.value,
    opacity: endProperty.lines.opacity,
    ...properties.common,
  });

  uiStore.toggleDarkMode();
};
</script>