<template>
  <div>
    <RegisterPanel />
  </div>
  <PageFooter :static="true"></PageFooter>
</template>

<script setup lang="ts">
import RegisterPanel from '@/components/RegisterPanel.vue';
import PageFooter from '@/components/lesser/PageFooter.vue';
</script>

<style scoped>
:root {
  position: fixed;
  left: 50%;
  top: 50%;
  transform: translate(-50%, -50%);
}
</style>