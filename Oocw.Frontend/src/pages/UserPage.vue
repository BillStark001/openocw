<template>
  <PageFrame>
    <p>{{ t('User Page') }}</p>
    <button class="round-button" v-on:click="logout">Log Out</button>
  </PageFrame>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';
import { tryLogOut } from '@/api/auth';
import PageFrame from '../components/PageFrame.vue'
import { useRouter } from 'vue-router';
const { t } = useI18n({
  inheritLocale: true,
  useScope: "local"
});
const router = useRouter();
async function logout() {
  await tryLogOut();
  router.push('/auth');
}

</script>
