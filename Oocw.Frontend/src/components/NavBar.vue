<template>
  <div v-bind:class="n()" v-if="!!$slots.default">
    <div class="notice">
      <slot></slot>
    </div>
    <div class="close-btn" v-on:click="closeNotice">
      <p>×</p>
    </div>
  </div>

  <div class="navbar navbar-main">
    <ul class="navbar-inner">
      <li>
        <router-link class="item" to="/">
          <img src="../assets/svg/logo.svg" style="margin-right: 4px; height: 36px; position: relative; top: 12px;" />
          <span class="logo-text">{{ t('product.name') }}</span>
        </router-link>
      </li>

      <li class="navbar-titles">

        <PopupItem v-bind:class="i(0)">
          <template v-slot:header>
            <router-link class="a" to="/">{{ t('navbar.homepage') }}</router-link>
          </template>
          <div class="item-list">
            <a href="/none/">test text</a>
            <a href="/none/">test text</a>
            <a href="/none/">test text</a>
            <a href="/none/">test text</a>
          </div>
        </PopupItem>

        <PopupItem v-bind:class="i(1)">
          <template v-slot:header>
            <router-link class="a" to="/db">{{ t('navbar.database') }}</router-link>
          </template>

        </PopupItem>

        <a v-bind:class="i(2)" href="javascript:void(0);" v-on:click="processUser">{{ t('navbar.user') }}</a>

      </li>

      <li class="align-right">
        <router-link v-bind:class="i(3)" to="/info">{{ t("product.version") }}</router-link>
        <PopupItem class="item" align-right="true">
          <template v-slot:header>
            <a class="item" href="javascript: void(0);">
              <img src="../assets/svg/lang.svg">
            </a>
          </template>
          <div class="item-list">
            <a href="javascript: void(0);" style="font-size: 12px; line-height: 20px;" v-on:click="changeLocale()">
              <p>{{  t('lang.detect')  }}</p>
              <p>{{  t('lang.current') + t('lang.name')  }}</p>
            </a>
            <a href="javascript: void(0);" v-on:click="changeLocale('en')">{{  t('lang.en')  }}</a>
            <a href="javascript: void(0);" v-on:click="changeLocale('zh-CN')">{{  t('lang.zh-CN')  }}</a>
            <a href="javascript: void(0);" v-on:click="changeLocale('ja-JP')">{{  t('lang.ja-JP')  }}</a>
          </div>
        </PopupItem>


        <a class="item" href="javascript: void(0);" v-on:click="processUser">
          <img src="../assets/svg/user.svg">
        </a>
      </li>

      <li class="search-bar align-right" style="margin-right: 5px;">
        <div class="search-inner">
          <input type="search" class="search-input" v-model="searchText" v-on:keyup="triggerSearch" autocomplete="off"
            v-bind:placeholder="t('searchbar.search')" v-bind:aria-label="t('searchbar.search.hint')">
          <div class="img-container" v-on:click="triggerSearch2">
            <img src="../assets/svg/search.svg">
          </div>

        </div>
      </li>
    </ul>
  </div>
</template>

<script lang="ts">

import { defineComponent } from 'vue';
import { useI18n, changeLocale } from '../i18n';
import PopupItem from './PopupItem.vue';
import { isLoggedIn } from '@/api/auth';

interface NavBarData {
  page: number,
  noticeOn: boolean,
  searchText: string,
}


export default defineComponent({
  data(): NavBarData {
    this.refreshPage();
    var ret: NavBarData =  {
      page: 0,
      noticeOn: !!this.$slots.default,
      searchText: "",
    }
    return ret;
  },
  setup() {
    const { t } = useI18n();
    return { t };
  },
  components: {
    PopupItem
  },
  watch: {
    '$route' () {
      this.refreshPage();
    }
  }, 
  methods: {
    refreshPage() {
      const path = this.$route.fullPath;
      if (path.startsWith('/db'))
        this.page = 1;
      else if (path.startsWith('/user'))
        this.page = 2;
      else if (path.startsWith('/auth'))
        this.page = 2;
      else if (path.startsWith('/info'))
        this.page = 3;
      else
        this.page = 0;
    },
    i(x: number): string {
      return "item" + (x == this.page ? " active" : "");
    },
    n() {
      return "noticebar" + (this.noticeOn ? "" : " closed");
    },
    closeNotice(): void {
      this.noticeOn = false;
    },
    triggerSearch(e: KeyboardEvent): void {
      if (e.key === 'Enter')
        this.triggerSearch2();
    },
    triggerSearch2(): void {
      // TODO
      console.log(`search ${this.searchText}`);
    },

    changeLocale(lang?: string): void {
      changeLocale(lang);
    },

    async processUser(): Promise<void> {
      var status = await isLoggedIn();
      if (status)
        this.$router.push('/user');
      else
        this.$router.push('/auth');
    }

  },
  props: ['notice']
});


</script>

<style scoped>
.navbar {
  background-color: var(--navbar-background-color);
  color: white;
  position: relative;
  top: 0;
  left: 0;
  width: 100%;
  z-index: 1;
  box-sizing: border-box;
}

.navbar-main {
  position: sticky;
  width: 100%;
}

.navbar ul {
  list-style-type: none
}

.navbar li {
  margin: 1px;
  float: left;
  width: auto;
}

.noticebar {
  text-align: center;
  min-height: var(--navbar-notice-min-height);
  background-color: var(--navbar-notice-background-color);
  position: relative;
  overflow: hidden;
  color: var(--color-txt1);
}


.noticebar * {
  color: inherit;
}

.noticebar.closed {
  animation-duration: 0.6s;
  animation-name: slide-up;
  min-height: 0;
  height: 0;
}


@keyframes slide-up {
  from {
    min-height: var(--navbar-notice-min-height);
  }

  to {
    min-height: 0;
    height: 0;
  }
}

.noticebar .notice {
  margin-left: auto;
  margin-right: auto;
  float: top;
  min-height: 100% !important;
  height: 100%;
  display: block;
}

.noticebar .close-btn {
  position: absolute;
  height: 100%;
  float: right;
  min-width: var(--navbar-notice-min-height);
  right: 0;
  top: 0;
  line-height: 100%;
  cursor: pointer;
}


.noticebar .close-btn>p {
  display: block;
  float: bottom;
  font-size: calc(160%);
  line-height: var(--navbar-notice-min-height);
  margin: auto auto;
}

.navbar .navbar-inner {
  width: 90%;
  height: var(--navbar-height);
  padding-left: 20px;
  padding-right: 20px;
  margin-left: auto;
  margin-right: auto;
  justify-content: center;
  display: block;
}

.navbar .navbar-inner li {
  list-style: none;

  margin: auto auto;
  display: block;
}


.navbar .navbar-titles {
  display: block;
  float: left;
  margin-left: 0;
}

.navbar li.search-bar {
  --some-height-related-value: calc(var(--navbar-height) * 0.72);
  height: var(--some-height-related-value);
  width: var(--navbar-height);
  box-sizing: border-box;
  border-radius: var(--navbar-height);
  margin: calc(var(--navbar-height) * 0.14) 0;
  justify-content: center;
  align-items: center;
  transition: all 0.3s;
  overflow: hidden;
}

.navbar li.search-bar:hover,
.navbar li.search-bar:focus-within {
  width: 300px;
  background-color: var(--color-button-hover);
}

.navbar .align-right {
  float: right;
  display: block;
  margin-right: 0;
}

.navbar .item {
  float: left;
  height: 100%;
  position: relative;
  line-height: var(--navbar-height);
  font-weight: 300;

  transition: all 0.2s;
}

.navbar .item:hover,
.navbar .item:active {
  background-color: var(--color-button-hover);
}

/* item list*/

.navbar .item-list {
  position: relative;
  top: 3px;
  width: var(--navbar-item-list-width);
  background-color: var(--color-bg2);
  color: var(--color-txt1);
}

.navbar .item-list>a {
  display: block;
  padding: 6px 6px 6px 18px;
  height: calc(var(--navbar-height) * 0.8);
  line-height: calc(var(--navbar-height) * 0.8);
  transition: 250ms;
  position: relative;
}

.navbar .item-list>a:hover {
  background-color: var(--color-button-hover);
}

.navbar .item-list>a::after {
  display: block;
  content: '';
  width: 6px;
  height: calc(100% - var(--top) * 2);
  border-radius: 114514px;
  background: var(--navbar-hint-color);
  position: absolute;
  --top: 4px;
  top: var(--top);
  left: var(--top);
}

.align-right .item-list>a {
  padding: 6px 18px 6px 6px;
}

.align-right .item-list>a::after {
  left: auto;
  right: var(--top);
}

/* img */
.navbar img {
  position: relative;
  height: 24px;
  top: 6px;
  filter: invert(1);
}

.navbar .item.active::after {
  display: block;
  content: '';
  width: 100%;
  height: 5px;
  background: var(--navbar-hint-color);
  position: absolute;
  bottom: 0;
  left: 0;
}

.navbar .item .a {
  display: block;
  height: 100%;
  width: 100%;

  padding: 0 14px;
}

.navbar a.item {
  padding: 0 14px;
}

.search-bar {
  width: 100%;
  margin: 0;
  padding: 0;
  display: flex;
  justify-content: center;
  align-items: center;
}

.search-inner {
  height: 100%;
  position: relative;
}

.search-inner input {
  position: absolute;
  left: 10px;

  display: inline-block;
  background: none;
  border: none;
  outline: none;
  color: white;
  font-size: 14px;
  width: calc(100% - var(--some-height-related-value) - 3px);
  height: 100%;
  padding: 10px;
}

.search-inner input::placeholder {
  color: #ffffff88;
}

.search-inner input::-webkit-search-cancel-button {
  position: relative;
  right: -5px;

  -webkit-appearance: none;
  height: 12px;
  width: 12px;
  border-radius: 9999px;
  background: #ffffff44;
  border: 1px solid white;
  content: '×';
}

.search-inner .img-container {
  height: 100%;
  position: absolute;
  top: 0;
  right: 0;
  line-height: var(--some-height-related-value);
  float: right;
  width: var(--some-height-related-value);
  --navbar-search-inner-height: calc(100% - 1px);
}

.navbar .search-inner .img-container img {
  top: 0px;
  left: 10px;
  position: relative;
  vertical-align: middle;
  cursor: pointer;
}
</style>