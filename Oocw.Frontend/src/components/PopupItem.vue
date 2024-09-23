<template>
  <div class="popup-item">
    <slot name="header"></slot>
    <div v-bind:class="r()">
      <slot></slot>
    </div>
  </div>
</template>

<script setup lang="ts">
import clsx from 'clsx';


const props = defineProps(['alignRight']);

function r() {
  return clsx('submenu', props.alignRight ? ' align-right' : "");
}

</script>


<style scoped>
.popup-item {
  display: inline-block;
}

.submenu {
  display: none;
  min-width: 100px;
  height: 0px;
  top: var(--navbar-height);
  transition: 400ms;
}

.submenu.align-right {
  right: 0;
  text-align: right;
}

.submenu:hover,
.popup-item:hover .submenu {
  display: block;
  position: absolute;
  height: auto;
}
</style>