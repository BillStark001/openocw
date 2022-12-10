<template>
  <div class="aside-wrapper">
    <div ref="backdrop" class="backdrop" @click.self="$emit('hide')" />
    <aside ref="aside">
      <slot />
    </aside>
  </div>
</template>
  
<script lang="ts">
import { defineComponent } from 'vue';
import anime from 'animejs';
export default defineComponent({
  props: {
    show: Boolean,
  },
  watch: {
    show: {
      handler(value = false, oldValue: boolean) {
        if (value === oldValue) return;
        value ? this.showAside() : this.hideAside();
      },
    },
  },
  methods: {
    showAside() {
      this.$el.style.display = 'block';
      this.$el.style.pointerEvents = 'auto';
      anime({
        targets: this.$refs.backdrop as HTMLDivElement,
        opacity: [0, 1],
        easing: 'easeOutQuart',
        duration: 300,
      });
      anime({
        targets: this.$refs.aside as HTMLElement,
        opacity: [0.7, 1],
        translateX: ['-100%', '0%'],
        easing: 'easeOutQuart',
        duration: 300,
      });
    },
    hideAside(duration: number = 300, duration2: number = 300) {
      this.$el.style.pointerEvents = 'none';
      anime({
        targets: this.$refs.backdrop as HTMLDivElement,
        opacity: [1, 0],
        easing: 'easeOutQuart',
        duration,
        complete: () => {
          this.$el.style.display = 'block';
        },
      });
      anime({
        targets: this.$refs.aside as HTMLElement,
        opacity: [1, 0.7],
        translateX: ['0%', '-100%'],
        easing: 'easeOutQuart',
        duration: duration2,
      });
    },
  },
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