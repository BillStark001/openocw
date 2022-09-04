<template>
  <div id="search-panel" class="screen-centered">
    <div class="logo-main">
      <img src="../assets/svg/logo.svg" style="height: 160px;" />
      <br />
      <span class="logo-text" style="font-size: 30px;">{{ t('product.name') }}</span>
      <div style="display: inline-block; position: relative; height: 30px; width: 8px;">
        <span style="font-size: 8px; position: absolute; top: 0">(?)</span>
      </div>

    </div>
    <div class="search-inner">
      <input type="search" class="search-input" v-model="searchText" v-on:keyup="triggerSearch" autocomplete="off"
        v-bind:placeholder="t('searchbar.search')" v-bind:aria-label="t('searchbar.search.hint')">
      <div class="img-container" v-on:click="triggerSearch2">
        <img src="../assets/svg/search.svg">
      </div>

    </div>
    <div class="button-panel">
      <span>{{ t('hp.search.hint') }}</span>
      <button class="round-button s-main h">{{ t('hp.search.kw') }}</button>
      <button class="round-button s-main h">{{ t('hp.search.faculty') }}</button>
      <button class="round-button s-main h">{{ t('hp.search.code') }}</button>
      <button class="round-button s-main h">{{ t('hp.search.name') }}</button>
      <span>{{ t('hp.search.hint.rear') }}</span>
    </div>
  </div>

  <PageFooter static="true"></PageFooter>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import { useI18n } from 'vue-i18n';
import PageFooter from '@/components/lesser/PageFooter.vue';



interface MainPageData {
  searchText: string,
}

export default defineComponent({
  name: "MainPage",
  setup() {
    const { t } = useI18n({
      inheritLocale: true,
      useScope: "local"
    });
    return { t };
  },
  data(): MainPageData {
    return {
      searchText: "",
    };
  },
  methods: {
    triggerSearch(e: KeyboardEvent): void {
      if (e.key === 'Enter')
        this.triggerSearch2();
    },
    triggerSearch2(): void {
      // TODO
      console.log(`search ${this.searchText}`);
    },
  },
  mounted() {

  },
  components: {
    PageFooter
  }
})
</script>


<style scoped>
#search-panel {
  text-align: center;
  color: var(--color-txt1);
}

.logo-main {
  margin: 20px;
  filter: drop-shadow(0px 0px 10px var(--color-bg1));
}

.dark-mode .logo-main img {
  filter: invert(1);
}

.button-panel {
  padding: 20px 0 70px 0;
}

.search-inner {

  box-sizing: border-box;
  width: var(--main-page-search-bar-width);
  height: var(--main-page-search-bar-height);

  backdrop-filter: blur(10px);
  background-color: #ffffff11;
  padding: calc((var(--main-page-search-bar-height) - var(--main-page-search-bar-txt-size)) / 2);
  border-radius: 114514px;
  border: 1px solid #ffffff11;
  box-shadow: 0px 0px 17px #00000077;
}

.search-input {

  line-height: var(--main-page-search-bar-txt-size);
  font-size: var(--main-page-search-bar-font-size);

  border: none;
  outline: none;
  background-color: transparent;
  display: inline-block;
  width: calc(var(--main-page-search-bar-width) - var(--main-page-search-bar-txt-size) * 2);
  color: var(--color-txt-1);
}

.search-input::placeholder {
  color: var(--color-txt-trs2);
}

.search-input::-webkit-search-cancel-button {
  position: relative;

  -webkit-appearance: none;
  height: 12px;
  width: 12px;
  border-radius: 9999px;
  background: #ffffff44;
  border: 1px solid white;
  content: '×';
}

.img-container {
  display: inline-block;

  height: var(--main-page-search-bar-txt-size);
  width: var(--main-page-search-bar-txt-size);
  line-height: var(--main-page-search-bar-txt-size);

  border-radius: 114514px;
  transition: 300ms;
  cursor: pointer;
}

.img-container:hover {
  background-color: var(--color-txt-trs3);
}

.img-container img {
  transform: translate(0, 2px);
  height: var(--main-page-search-bar-font-size);
}

.dark-mode .img-container img {
  filter: invert(1);
}

#notice #notice-disp {
  white-space: pre-wrap;
  text-align: left;
}

#btn-dl {
  padding: 10px 40px;
}

#btn-dl p:first-child {
  margin: 3px;
  font-size: 30px;
}
</style>