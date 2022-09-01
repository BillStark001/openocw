<template>
  <span class="route-div"></span>
  <div id="route-disp">
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

import { defineComponent } from 'vue';
import { useI18n } from 'vue-i18n';

interface RouteData {
  displayKey: string,
  routeTarget?: string,
}

interface RouteDisplayData {
  fullPath: string;
  separatedPaths: RouteData[];
}

export default defineComponent({
  name: "RouteDisplay",
  data(): RouteDisplayData {
    return {
      fullPath: '',
      separatedPaths: [],
    };
  },
  setup() {
    const { t } = useI18n();
    return { t };
  },
  methods: {
    updateRoute() {
      this.fullPath = this.$route.fullPath;
      this.separatedPaths = [];
      const paths = this.fullPath.split('/');
      for (let i = 0; i < paths.length; ++i) {
        if (i == 0 || (paths[i] == '' && i == paths.length - 1))
          continue;
        let path = paths[i];
        let key = `pos.${path}`;
        this.separatedPaths.push({
          displayKey: key,
          routeTarget: paths.slice(0, i + 1).join('/')
        });
      }
    }
  },
  mounted() {
    this.updateRoute();
  }
});

export {
  RouteData,
  RouteDisplayData,
}

</script>

<style scoped>

.route-div {
  display:block;
  background-color:var(--color-txt-trs3);
  height:1px;
  width:100%;
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