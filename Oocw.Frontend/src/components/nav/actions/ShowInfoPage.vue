<template>
  <NavItem to="/info">
    {{ t("product.version") }}
  </NavItem>
</template>

<script setup type="ts">
import { useI18n } from '@/i18n';
import NavItem from '@/components/base/NavItem.vue';
const { t } = useI18n({ useScope: 'global' });
</script>