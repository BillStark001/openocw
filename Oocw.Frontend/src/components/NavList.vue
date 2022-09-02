<!-- eslint-disable no-irregular-whitespace -->
<template>
  <ul class="list-root" v-for="child in node.children" v-bind:key="child.key">
    <li class="list-item">
      <details :class="'nav-list' + (child.children.length ? ' general' : ' unmarked')" v-bind:open="shouldOpen(child)">
        <summary>
          <router-link v-if="hasAction(child)" :to="getActionHref(child)">{{ t(child.key) }}</router-link>
          <span v-if="!hasAction(child)">{{ t(child.key) }}</span>
        </summary>
        <!--here places the nav list-->
        <NavList :data="generateData(child)"></NavList>
      </details>
    </li>
  </ul>
</template>

<script lang="tsx">
import { defineComponent } from 'vue';
import { useI18n } from 'vue-i18n';
import * as utils from '@/utils/query';

export interface NavNode {
  key: string,
  action: "none" | "self" | "parent" | "children" | "uncat",
  children: NavNode[],
}

function defaultNode(): NavNode {
  return {
    key: "",
    action: "none",
    children: []
  }
}

/*
function testNode(): NavNode {
  return {
    key: "0",
    action: "none",
    children: [{
      key: "1",
      action: "none",
      children: [{
        key: "3",
        action: "none",
        children: []
      }, {
        key: "4",
        action: "none",
        children: []
      }]
    }, {
      key: "2",
      action: "none",
      children: []
    }]
  }
}
*/

export interface NavListData {
  node: NavNode,
  selected: string[],
  path: string[], 
}

export default defineComponent({
  name: "NavList",
  data(): NavListData {
    if (this.$props.data) {
      var data = this.$props.data as NavListData;
      return data;
    }

    return {
      node: defaultNode(),
      selected: [], 
      path: []
    };
  },
  setup() {
    const { t } = useI18n();
    return { t };
  },
  methods: {
    shouldOpen(n: NavNode): boolean {
      return this.selected && this.selected[0] == n.key;
    },

    hasAction(n: NavNode): boolean {
      const a = n.action;
      return a == 'self' || a == 'parent' || a == 'children' || a == 'uncat'; 
    }, 

    getActionHref(n: NavNode): string {
      if (n.action == 'uncat')
        return '/db/' + n.action;
      return '/db/' + utils.encodePath(this.path.concat([n.key]));
    },
    generateData(n: NavNode): NavListData {
      var ret: NavListData = {
        node: n,
        selected:
          this.shouldOpen(n) ?
            this.selected.slice(1) :
            [],
        path: this.path.concat([n.key])
      };
      return ret;
    },

  },
  props: ['source', 'data']
});

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
  overflow-x:hidden;
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

summary>* {
  display: inline-block;
  max-width: 90%;
}

summary::marker {
  display: none;
}

.general>summary:before,
.general>summary:after {
  content: "";
  margin: auto;
  position: absolute;
  top: 0;
  left: 0;
  transition: .3s;
}

.general>summary:before {
  width: 20px;
  height: 20px;
  border-radius: 114514px;
}

.general>summary:hover:before {
  background-color: var(--color-txt-trs3);
}

.general>summary:after {
  content: "+";
  font-size: 22px;
  box-sizing: border-box;
  transform: translate(2px, -6px);
}

.general[open]>summary:after {
  transform: translate(3.5px, -5.5px) rotate(45deg);
}
</style>