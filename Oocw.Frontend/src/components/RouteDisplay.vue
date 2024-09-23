<template>
  <span class="route-div"></span>
  <div id="route-disp" v-if="updateFlag">
    <span class="rkey">{{ t('pos.current') }}</span>
    <router-link class="rkey h" to="/">{{ t('pos.root') }}</router-link>
    <template v-for="path in separatedPaths" :key="path">
      <span class="rkey rsign">></span>
      <router-link class="rkey h" v-if="path.routeTarget" :to="path.routeTarget">
        {{ t(path.displayKey) }}
      </router-link>
      <span class="rkey" v-if="!path.routeTarget">{{ t(path.displayKey) }}</span>
    </template>
  </div>
  <span class="route-div"></span>
</template>

<script setup lang="ts">
import { ref, onMounted, nextTick } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute, onBeforeRouteUpdate } from 'vue-router';
import { decodePath } from '@/utils/query';

interface RouteData {
  displayKey: string;
  routeTarget?: string;
}

const { t } = useI18n();
const route = useRoute();

const fullPath = ref<string | undefined>(undefined);
const updateFlag = ref(true);
const separatedPaths = ref<RouteData[]>([]);

const updateRoute = (path?: string) => {
  fullPath.value = path || route.fullPath;
  const paths = fullPath.value.split('/');
  separatedPaths.value = [];

  for (let i = 0; i < paths.length; ++i) {
    if (i == 0 || (paths[i] == '' && i == paths.length - 1)) continue;
    
    let path = paths[i];
    let key = `pos.${path}`;
    
    // special judges
    if (paths[1] == 'db' && i == 2) {
      if (path == 'uncat') {
        key = 'meta.uncat';
      } else {
        let _key = decodePath(path);
        key = _key[_key.length - 1];
      }
    }

    separatedPaths.value.push({
      displayKey: key,
      routeTarget: paths.slice(0, i + 1).join('/')
    });
  }
};

onMounted(() => {
  updateRoute();
});

onBeforeRouteUpdate(async (to) => {
  updateFlag.value = false;
  updateRoute(to.fullPath);
  await nextTick();
  console.log(separatedPaths.value);
  updateFlag.value = true;
});
</script>

<style scoped>
.route-div {
  display: block;
  background-color: var(--color-txt-trs3);
  height: 1px;
  width: 100%;
}

#route-disp {
  box-sizing: border-box;
  width: 100%;
  padding: 10px;
}

.rkey {
  background-color: transparent;
  border-radius: 3px;
  padding: 4px;
  transition: 300ms;
}

.rsign {
  font-family: 'Consolas', 'Courier New', Courier, monospace;
  padding: 2px 6px;
}

.rkey.h:hover {
  background-color: var(--color-txt-trs3);
}
</style>