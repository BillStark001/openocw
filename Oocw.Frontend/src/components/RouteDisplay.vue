<template>
  <span class="route-div"></span>
  <div id="route-disp" v-if="updateFlag">
    <span class="rkey">{{ t('pos.current') }}</span>
    <router-link class="rkey h" to="/">{{ t('pos.root') }}</router-link>
    <template v-for="path in separatedPaths" :key="path">
      <span class="rkey rsign">></span>
      <router-link class="rkey h" v-if="path.routeTarget" v-bind:to="path.routeTarget">{{ t(path.displayKey) }}
      </router-link>
      <span class="rkey" v-if="!path.routeTarget">{{ t(path.displayKey) }}</span>
    </template>
  </div>
  <span class="route-div"></span>
</template>

<script lang="ts">

import { decodePath } from '@/utils/query';
import { defineComponent } from 'vue';
import { useI18n } from 'vue-i18n';

interface RouteData {
  displayKey: string,
  routeTarget?: string,
}

interface RouteDisplayData {
  fullPath?: string;
  updateFlag: boolean;
  separatedPaths: RouteData[];
}

export default defineComponent({
  name: "RouteDisplay",
  data(): RouteDisplayData {
    return {
      fullPath: undefined,
      updateFlag: true, 
      separatedPaths: [],
    };
  },
  setup() {
    const { t } = useI18n();
    return { t };
  },
  methods: {
    updateRoute(fullPath?: string) {
      this.fullPath = fullPath || this.$route.fullPath;
      var separatedPaths = [];
      const paths = this.fullPath.split('/');
      for (let i = 0; i < paths.length; ++i) {
        if (i == 0 || (paths[i] == '' && i == paths.length - 1))
          continue;
        let path = paths[i];
        let key = `pos.${path}`;
        // special judges
        if (paths[1] == 'db' && i == 2) {
          if (path == 'uncat')
            key = 'meta.uncat';
          else {
            let _key = decodePath(path);
            key = _key[_key.length - 1];
          }
        }

        separatedPaths.push({
          displayKey: key,
          routeTarget: paths.slice(0, i + 1).join('/')
        });
      }
      this.separatedPaths = separatedPaths;
    }
  },
  mounted() {
    this.updateRoute();
  },
  async beforeRouteUpdate(from, to) {
    this.updateFlag = false;
    this.updateRoute(to.fullPath);
    await this.$nextTick();
    console.log(this.separatedPaths);
    this.updateFlag = true;
  }, 
  /*
  async updated() {
    this.updateFlag = false;
    await this.$nextTick();
    this.updateFlag = true;
  }*/
});

export {
  RouteData,
  RouteDisplayData,
}

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