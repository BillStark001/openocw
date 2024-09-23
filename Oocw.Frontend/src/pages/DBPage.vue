<template>
  <PageFrame>
    <PageBanner />
    <RouteDisplay v-if="treeData"></RouteDisplay>
    <div v-if="!route.params.target" class="org-area">
      <div v-for="org in orgs" :key="org.key" class="org-panel">
        <router-link :to="'/db/' + (org.key)">
          <img v-if="org.key == 'org.sos'" src="@/assets/svg/tit/chem.svg">
          <img v-if="org.key == 'org.soe'" src="@/assets/svg/tit/robotic-arm.svg">
          <img v-if="org.key == 'org.somct'" src="@/assets/svg/tit/mat.svg">
          <img v-if="org.key == 'org.soc'" src="@/assets/svg/tit/chip.svg">
          <img v-if="org.key == 'org.solst'" src="@/assets/svg/tit/bio.svg">
          <img v-if="org.key == 'org.soes'" src="@/assets/svg/tit/city.svg">
          <img v-if="org.key == 'crs.la'" src="@/assets/svg/tit/art.svg">
          <img v-if="org.key == 'org.ext.trsdis'" src="@/assets/svg/tit/idea.svg">
          <p>{{ t(org.key) }}</p>
        </router-link>
      </div>
    </div>
    <div id="subframe" v-if="route.params.target">
      <div id="sf-left">
        <NavList v-if="treeData" :data="treeData"></NavList>
      </div>
      <div id="sf-right">
        <CourseList v-if="courses" :query="courses" place="db" />
      </div>
    </div>
  </PageFrame>
  <PageFooter></PageFooter>
</template>

<script setup lang="ts">
import { ref, onMounted, watch, nextTick } from 'vue';
import { useRoute } from 'vue-router';
import { useI18n } from 'vue-i18n';

import PageFrame from '../components/PageFrame.vue';
import RouteDisplay from '@/components/RouteDisplay.vue';
import NavList from '@/components/NavList.vue';
import { NavNode, NavListData } from '@/components/NavList.vue';
import PageBanner from '@/components/lesser/PageBanner.vue';
import PageFooter from '@/components/lesser/PageFooter.vue';
import CourseList from '@/components/courses/CourseList.vue';

import OrgTree from '@/assets/meta/orgtree.json';
import Orgs from '@/assets/meta/orgs.json';
import * as utils from '@/utils/query';
import { CourseBrief, getCourseListByDepartment } from '@/api/query';

const { t } = useI18n();
const route = useRoute();

const treeData = ref<NavListData | undefined>();
const curOpr = ref<string | undefined>();
const courses = ref<utils.QueryResult<CourseBrief[]> | undefined>();

const orgs = Orgs as { key: string }[];

function getCurrentOpr(cur: NavNode, parents: NavNode[]): string[] {
  let ret: string[] = [];
  if (cur.action === 'self') ret.push(cur.key);
  else if (cur.action === 'uncat') ret.push(cur.action);
  else if (cur.action === 'parent') ret = parents.length > 1 ? [parents[0].key] : [];
  else if (cur.action === 'children') {
    cur.children.forEach(c => ret = ret.concat(getCurrentOpr(c, [cur, ...parents])));
  }
  return ret;
}

function identifyCurrentOpr(root: NavNode, path: string[]): string | undefined {
  let parent: NavNode[] = [];
  let i = 0;

  let cur: NavNode = root;
  while (i < path.length) {
    const matched = cur.children.find(child => child.key === path[i]);
    if (matched) {
      parent = [cur, ...parent];
      cur = matched;
      i++;
    } else {
      cur = root;
      parent = [];
      break;
    }
  }

  const rets = getCurrentOpr(cur, parent);
  return rets.length > 0 ? rets.join(",") : undefined;
}

function getTargetPath() {
  const target = route.params.target;
  if (target === undefined) return [];
  if (Array.isArray(target)) return utils.decodePath(target.join());
  return utils.decodePath(target);
}


async function updateData() {
  const target = getTargetPath();
  curOpr.value = identifyCurrentOpr(OrgTree as NavNode, target);

  const _res = await getCourseListByDepartment(curOpr.value ?? "uncat");
  courses.value = undefined;

  treeData.value = {
    node: OrgTree as NavNode,
    selected: target,
    path: [],
  }

  await nextTick();
  courses.value = _res;
}

onMounted(updateData);

watch(() => route.params, updateData);
</script>

<style scoped>
#subframe {
  overflow: hidden;
}

#sf-left {
  width: var(--leftbar-width);
  box-sizing: border-box;
  padding: 10px 10px 10px 0;
  float: left;
}

#sf-right {
  width: calc(100% - var(--leftbar-width));
  float: left;
}

.org-area {
  text-align: center;
}

.org-panel {
  display: inline-block;
  width: calc(24% - 60px);
  max-width: 320px;
  min-width: 120px;
  box-sizing: border-box;
  margin: 30px;
  background-color: var(--db-panel-color);
  box-shadow: 0 0 17px #00000055;
}

.org-panel a {
  display: block;
  width: 100%;
  height: 100%;
  align-items: center;
  text-align: center;
  padding: 45px 0;
}

.org-panel * {
  transition: 300ms;
}

.org-panel a:hover {
  background-color: #ffffff22;
}

.org-panel a:hover * {
  opacity: 0.5;
}

.dark-mode .org-panel a:hover * {
  opacity: 1;
}

.org-panel img {
  height: 100px;
}

.dark-mode .org-panel img {
  filter: invert(1);
}

.org-panel p {
  margin: auto;
  margin-top: 10px;
  font-size: large;
  max-width: calc(100% - 30px);
}
</style>
