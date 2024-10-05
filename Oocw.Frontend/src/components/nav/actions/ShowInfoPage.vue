<template>
  <NavItem to="/info">
    {{ t("product.version") }}
  </NavItem>
</template>

<script setup type="ts">
import { useI18n } from 'vue-i18n';
import NavItem from '@/components/base/NavItem.vue';
const { t } = useI18n();
</script>