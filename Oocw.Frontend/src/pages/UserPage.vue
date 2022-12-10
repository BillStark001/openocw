<template>
  <PageFrame>
    <p>User Page</p>
    <button class="round-button" v-on:click="logout">Log Out</button>
  </PageFrame>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { useI18n } from 'vue-i18n'
import { tryLogOut } from '@/api/auth';
import PageFrame from '../components/PageFrame.vue'

export default defineComponent({
  name: "UserPage",
  setup() {
    const { t } = useI18n({
      inheritLocale: true,
      useScope: "local"
    });
    // Something todo ..
    return { t };
  },
  components: { PageFrame },
  methods: {
    async logout() {
      await tryLogOut();
      this.$router.push('/auth');
    }
  }
})
</script>
