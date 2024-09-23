<template>
  <ul class="list-root" v-for="child in node.children" :key="child.key">
    <li class="list-item">
      <details :class="['nav-list', child.children.length ? 'general' : 'unmarked']" :open="shouldOpen(child)">
        <summary>
          <router-link v-if="hasAction(child)" :to="getActionHref(child)">{{ t(child.key) }}</router-link>
          <span v-else>{{ t(child.key) }}</span>
        </summary>
        <NavList :data="generateData(child)"></NavList>
      </details>
    </li>
  </ul>
</template>

<script setup lang="ts">
import { defineProps, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import * as utils from '@/utils/query';

export interface NavNode {
  key: string;
  action: "none" | "self" | "parent" | "children" | "uncat";
  children: NavNode[];
}

export interface NavListData {
  node: NavNode;
  selected: string[];
  path: string[];
}

const props = defineProps<{
  source?: any;
  data?: NavListData;
}>();

const { t } = useI18n();

const navData = computed<NavListData>(() => {
  if (props.data) {
    return props.data;
  }
  return {
    node: defaultNode(),
    selected: [],
    path: []
  };
});

const node = computed(() => navData.value.node);
const selected = computed(() => navData.value.selected);
const path = computed(() => navData.value.path);

function defaultNode(): NavNode {
  return {
    key: "",
    action: "none",
    children: []
  };
}

function shouldOpen(n: NavNode): boolean {
  return selected.value && selected.value[0] === n.key;
}

function hasAction(n: NavNode): boolean {
  const a = n.action;
  return a === 'self' || a === 'parent' || a === 'children' || a === 'uncat';
}

function getActionHref(n: NavNode): string {
  if (n.action === 'uncat') {
    return '/db/' + n.action;
  }
  return '/db/' + utils.encodePath(path.value.concat([n.key]));
}

function generateData(n: NavNode): NavListData {
  return {
    node: n,
    selected: shouldOpen(n) ? selected.value.slice(1) : [],
    path: path.value.concat([n.key])
  };
}
</script>

<style scoped>
* {
  font-size: 16px;
  list-style: none;
}

ul .list-item {
  position: relative;
  left: 10px;
}

.list-root {
  position: relative;
  height: max-content;
  overflow-x: hidden;
}

.nav-list {
  padding: 4px;
}

summary {
  position: relative;
  display: block;
  padding-left: 22px;
  cursor: pointer;
}

summary > * {
  display: inline-block;
  max-width: 90%;
}

summary::marker {
  display: none;
}

.general > summary:before,
.general > summary:after {
  content: "";
  margin: auto;
  position: absolute;
  top: 0;
  left: 0;
  transition: 0.3s;
}

.general > summary:before {
  width: 20px;
  height: 20px;
  border-radius: 114514px;
}

.general > summary:hover:before {
  background-color: var(--color-txt-trs3);
}

.general > summary:after {
  content: "+";
  font-size: 22px;
  box-sizing: border-box;
  transform: translate(2px, -6px);
}

.general[open] > summary:after {
  transform: translate(3.5px, -5.5px) rotate(45deg);
}
</style>