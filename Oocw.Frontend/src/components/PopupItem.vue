<template>
  <div class="popup-item">
    <slot name="header"></slot>
    <div v-bind:class="r()">
      <slot></slot>
    </div>
  </div>
</template>

<script>

import { defineComponent } from 'vue';

export default defineComponent({
  name: 'PopupItem',
  props: ['alignRight'],
  methods: {
    r() {
      return 'submenu' + (this.alignRight ? ' align-right' : "");
    }
  }
})

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