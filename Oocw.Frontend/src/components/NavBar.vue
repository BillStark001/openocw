<template>
  <div class="navbar navbar-main">
    <div class="container clearfix">
      <nav class="navbar-menu">
        <NavItem @click="uiStore.showSideBar()">
          <img src="@/assets/svg/menu.svg" />
        </NavItem>
      </nav>
      <nav class="navbar-logo" v-if="!isDesktop()">
        <NavItem :to="isSuperGreen() ? '/main' : '/'" prevent-active>
          <img class="logo" src="@/assets/svg/logo.svg" />
          <span class="logo-text">{{ t('product.name') }}</span>
        </NavItem>
      </nav>
      <nav class="navbar-links">
        <NavLinks />
      </nav>
      <nav class="align-right">
        <NavItem to="/user">
          <img src="@/assets/svg/user.svg" />

          <template v-slot:dropdown v-if="user.baseInfo">
            <div class="submenu align-right">
              <div class="item-list">
                <RouterLink to="/user">
                  {{ t('navbar.user') }}
                </RouterLink>
                <RouterLink v-if="user.adminInfo" to="/admin/user">
                  {{ t('navbar.admin') }}
                </RouterLink>
                <a href="javascript:void(0)" @click="logout">{{
                  t('btn.logout')
                }}</a>
              </div>
            </div>
          </template>
        </NavItem>
      </nav>
      <nav class="navbar-actions align-right">
        <NavActions />
      </nav>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import NavItem from '@/components/base/NavItem.vue';
import NavLinks from '@/components/nav/NavLinks.vue';
import NavActions from './nav/NavActions.vue';
import { useCurrentUserStore } from '@/stores/CurrentUserStore';
import { useUIStore } from '@/stores/UIStore';

const router = useRouter();
const user = useCurrentUserStore();
const uiStore = useUIStore();
const { t } = useI18n();

function logout() {
  user.clear();
  router.push('/auth');
}

function isSuperGreen() {
  return false;
}

function isDesktop() {
  return false;
}

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

.navbar.navbar-main {
  position: sticky;
  width: 100%;
}

.navbar .container nav {
  display: flex;
  flex-wrap: wrap;
  float: left;
}

.navbar .container nav.align-right {
  float: right;
}

.navbar .container img.logo {
  margin-right: 4px;
  width: 36px;
  height: 36px;
  filter: none;
  margin-right: 5px;
}

.dark-mode .navbar .container img.logo {
  filter: invert(1);
}

.navbar>.navbar-inner>.align-right {
  float: right;
  display: block;
  margin-right: 0;
}

.navbar a.item {
  padding: 0 14px;
}

.navbar :deep(.container img) {
  filter: invert(1);
}

.navbar nav.navbar-menu {
  display: none;
}

@media only screen and (max-width: 768px) {
  .navbar .container {
    display: flex;
    flex-direction: row;
  }

  .navbar .container nav,
  .navbar .container nav.align-right {
    float: none;
  }

  .navbar nav.navbar-logo {
    flex: 1;
    display: flex;
    justify-content: center;
  }

  .navbar nav.navbar-menu {
    display: flex;
  }

  .navbar nav.navbar-links,
  .navbar nav.navbar-actions {
    display: none;
  }
}

.desktop-mode .navbar {
  padding: 0 35px;
}
</style>
