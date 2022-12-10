<template>
  <ShowInfoPage></ShowInfoPage>
  <ToggleDark />
  <LocaleSelect />
</template>

<script setup lang="ts">
import LocaleSelect from './actions/LocaleSelect.vue';
import ShowInfoPage from './actions/ShowInfoPage.vue';
import ToggleDark from './actions/ToggleDark.vue';
</script>