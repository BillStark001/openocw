


import MainPage from '@/pages/MainPage.vue';
import RegisterPage from '@/pages/RegisterPage.vue';
import InfoPage from '@/pages/InfoPage.vue';
import UserPage from '@/pages/UserPage.vue';
import DBPage from '@/pages/DBPage.vue';

import { createRouter, createWebHashHistory } from 'vue-router';

const routes = [
  { path: '/', component: MainPage, },
  { path: '/auth', component: RegisterPage, },
  { path: '/info', component: InfoPage, },
  { path: '/db', component: DBPage, },
  { path: '/db/:target', component: DBPage, },
  { path: '/user', component: UserPage, },
];

export const router = createRouter({
  history: createWebHashHistory(),
  routes: routes,
});

export function shouldBeActive(routeIn: string, target: string, precise?: boolean): boolean {
  return precise ? routeIn === target : routeIn.startsWith(target);
}
