<template>
  <NavItem :to="isSuperGreen()? '/main': '/'">
    {{ t('navbar.homepage') }}
  </NavItem>
  <NavItem to="/db">
    {{ t('navbar.database') }}
    <template v-slot:dropdown>
      <div class="submenu">
        <div class="item-list">
          <RouterLink 
            v-for="target in targets" 
            :to="`/db/${target}`"
            :class="{ active: shouldBeActive($route.fullPath, `/db/${target}`) }"
            >
            {{ t(target) }}
          </RouterLink>
        </div>
      </div>
    </template>
  </NavItem>
  <NavItem to="/discussion">
    {{ t('navbar.discussion') }}
  </NavItem>
  
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';
import NavItem from '@/components/base/NavItem.vue';
import Orgs from '@/assets/meta/orgs.json';
import { shouldBeActive } from '@/router';
const { t } = useI18n();
const targets: Array<string> = Orgs.map(x => x.key);
function isSuperGreen() {
  return false; 
}
</script>