<template>
  <div class="navitem">
    <RouterLink v-if="to" :to="to" :active-class="preventActive ? '' : 'active'">
      <slot />
    </RouterLink>
    <a v-else href="javascript:void(0)" @click.stop="$emit('click')">
      <slot />
    </a>
    <slot name="dropdown" />
  </div>
</template>

<script setup lang="ts">

defineProps<{
  to?: string,
  preventActive?: boolean,
}>();

</script>

<style scoped>
.navitem {
  float: left;
  position: relative;
  height: var(--navbar-height);
  line-height: var(--navbar-height);
  font-weight: 300;
  transition: all 0.2s;
  -webkit-tap-highlight-color: transparent;
}

.navitem a {
  display: flex;
  align-items: center;
  padding: 0 14px;
  height: 100%;
}

.navitem a :deep(img) {
  width: 24px;
  height: 24px;
}

.navitem a:hover,
.navitem a:active {
  background-color: var(--color-button-hover);
}

.navitem a.active::after {
  display: block;
  content: '';
  width: 100%;
  height: 5px;
  background: var(--navbar-hint-color);
  position: absolute;
  bottom: 0;
  left: 0;
}

.navitem :deep(.submenu .item-list) {
  position: relative;
  width: var(--navbar-item-list-width);
  background-color: var(--color-bg2);
  color: var(--color-txt1);
  overflow: hidden;
  border-radius: 2px;
  box-shadow: var(--popup-shadow);
}

.navitem :deep(.submenu .item-list > a) {
  display: block;
  padding: 8px 16px;
  position: relative;
  box-sizing: border-box;
  line-height: 36px;
  margin: 4px 0;
}

.navitem :deep(.submenu .item-list > a:hover) {
  background-color: var(--color-button-hover);
}

.navitem :deep(.submenu .item-list > a.active::after),
.navitem :deep(.submenu .item-list > a.router-link-active::after) {
  display: block;
  content: '';
  width: 6px;
  height: 100%;
  background: var(--navbar-hint-color);
  position: absolute;
  top: 0;
  left: 0;
}


.navitem :deep(.submenu.align-right .item-list > a.active::after),
.navitem :deep(.submenu.align-right .item-list > a.router-link-active::after) {
  left: auto;
  right: 0;
}


.navitem :deep(.submenu) {
  display: none;
  min-width: 100px;
  height: 0px;
  top: var(--navbar-height);
  transition: 400ms;
}

.navitem :deep(.submenu.align-right) {
  right: 0;
  text-align: right;
}

.navitem :deep(.submenu:hover),
.navitem:hover :deep(.submenu) {
  display: block;
  position: absolute;
  height: auto;
}

@media only screen and (max-width: 768px) {
  .navitem {
    height: auto;
  }

  .navitem>a {
    height: var(--navbar-height);
  }

  .navitem :deep(.submenu:hover),
  .navitem:hover :deep(.submenu) {
    position: static;
  }

  .navitem :deep(.submenu .item-list) {
    width: 100%;
  }
}
</style>