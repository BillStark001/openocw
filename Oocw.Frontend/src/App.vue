<template>
  <div id="dm">
    <NavBar>
      <p style="height: 36px; line-height: 36px; display: block;">{{ t('notice.intest.shorter') }}</p>
    </NavBar>
    <div id="bg"></div>
    <router-view></router-view>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import NavBar from './components/NavBar.vue';
import { useI18n } from './i18n';
import { provideCurrentUserStore } from './stores/CurrentUserStore';
import { provideUIStore } from './stores/UIStore';
import { Settings } from './utils/settings';


export interface RootData {
  darkMode: boolean,
}

export default defineComponent({
  name: 'App',
  data(): RootData {
    return {
      darkMode: Settings.darkMode
    };
  },
  setup() {
    provideCurrentUserStore();
    provideUIStore();
    const { t } = useI18n();
    return { t };
  },
  components: {
    NavBar,
  },
  methods: {
  },
  mounted() {
     
  }
});

</script>



<style src="./styles/controls.css" />
<style src="./styles/common.css">

</style>
<style>
* {
  margin: 0;
  padding: 0;
}

html {
  height: 100%;
  width: 100%;
  position: absolute;
  margin: 0;
  padding: 0;
}

body {
  height: 100%;
}

#app {
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  /* margin-top: 60px; */
}

#bg {
  position: fixed;
  width: 100%;
  height: 100%;
  left: 0;
  top: 0;
  z-index: -114514;
  background-color: transparent;

  background-image: url('@/assets/img/sakura.jpg');
  background-size: cover;
  background-position: center center;

  overflow: hidden;
}

.dark-mode #bg {
  background-image: url('@/assets/img/yuri.jpg');
}
</style>
