<!-- eslint-disable no-irregular-whitespace -->
<template>
  <ul class="list-root" v-for="child in node.children" v-bind:key="child.key">
    <li class="list-item">
      <details :class="'nav-list' + (child.children.length ? ' general' : ' unmarked')" v-bind:open="shouldOpen">
        <summary>
          <a href="#">{{ t(child.key) }}</a>
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

export interface NavNode {
  key: string,
  action: string,
  children: NavNode[],
}

function defaultNode(): NavNode {
  return {
    key: "",
    action: "",
    children: []
  }
}

function testNode(): NavNode {
  return {
    key: "0",
    action: "",
    children: [{
      key: "1",
      action: "",
      children: [{
        key: "3",
        action: "",
        children: []
      }, {
        key: "4",
        action: "",
        children: []
      }]
    }, {
      key: "2",
      action: "",
      children: []
    }]
  }
}

export interface NavListData {
  node: NavNode,
  selected: string[],
}

export default defineComponent({
  name: "NavList",
  data(): NavListData {
    if (this.$props.data)
      return this.$props.data as NavListData;
    return {
      node: testNode() || defaultNode(),
      selected: []
    };
  },
  setup() {
    const { t } = useI18n();
    return { t };
  },
  methods: {
    shouldOpen(): boolean {
      return this.selected && this.selected[0] == this.node.key;
    },
    generateData(n: NavNode): NavListData {
      return {
        node: n,
        selected:
          this.selected && this.node.key == this.selected[0] ?
            this.selected.slice(1) :
            [],
      };
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

summary::marker {
  display: none;
}

.general>summary:before,
.general>summary:after {
  content: "";
  margin: auto;
  position: absolute;
  top: 0;
  bottom: 0;
  left: 0;
  transition: .3s;
}

.general>summary:before {
  width: 20px;
  height: var(width);
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
  transform: translate(6px, -4px) rotate(45deg);
}
</style>