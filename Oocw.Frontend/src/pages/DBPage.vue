<template>
  <PageFrame>
    <PageBanner />
    <RouteDisplay v-if="treeData"></RouteDisplay>
    <div v-if="!($route.params.target)">
      test top hierarchy
      <div v-for="child in treeData!.node.children" :key="child.key">
        <router-link :to="'/db/' + (child.action == 'uncat' ? child.action : child.key)">{{ t(child.key) }}</router-link>
      </div>
    </div>
    <div id="subframe" v-if="($route.params.target)">
      <div id="sf-left">
        <NavList v-if="treeData" v-bind:data="treeData"></NavList>
      </div>
      <div id="sf-right">
        test right subframe
      </div>
    </div>

  </PageFrame>
  <PageFooter></PageFooter>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { useI18n } from 'vue-i18n'
import PageFrame from '../components/PageFrame.vue'
import RouteDisplay from '@/components/RouteDisplay.vue';
import NavList from '@/components/NavList.vue';
import { NavNode, NavListData } from '@/components/NavList.vue';
import PageBanner from '@/components/lesser/PageBanner.vue';
import PageFooter from '@/components/lesser/PageFooter.vue';

import OrgTree from '@/assets/meta/orgtree.json';
import * as utils from '@/utils/query';
import { RouteParams } from 'vue-router';

function identifyCurrentOpr(root: NavNode, path: string[]): string | undefined {
  var parent: string = "";
  var i: number = 0;

  // find the current node
  var cur: NavNode = root;
  while (i < path.length) {
    var matched = false;
    for (var child of cur.children) {
      if (child.key == path[i]) {
        parent = cur.key;
        cur = child;
        ++i;
        matched = true;
        break;
      }
    }
    if (!matched) {
      cur = root;
      parent = "";
      break;
    }
  }

  var ret: string | undefined = undefined;
  if (cur.action == 'self')
    ret = cur.key;
  else if (cur.action == 'parent')
    ret = parent;
  else if (cur.action == 'children')
    ret = cur.children.map(child => child.key || "").join();

  // TODO fix this after the restriction scheme is determined
  return ret;
}

export interface DBPageData {
  treeData?: NavListData;
  curOpr?: string;
}

export default defineComponent({
  name: "DBPage",
  data(): DBPageData {

    var target = this.getTargetPath();

    var data: NavListData = {
      node: OrgTree as NavNode,
      selected: target,
      path: [],
    }
    return {
      treeData: data,
      curOpr: identifyCurrentOpr(OrgTree as NavNode, target),
    };
  },
  setup() {
    const { t } = useI18n({
      inheritLocale: true,
      useScope: "local"
    });
    // Something todo ..
    return { t };
  },
  components: {
    PageFrame,
    RouteDisplay,
    NavList,
    PageBanner, 
    PageFooter, 
  },
  methods: {
    getTargetPath(params?: RouteParams): string[] {
      var target = (params || this.$route.params).target;
      if (target == undefined)
        target = '';
      else if (target instanceof Array)
        target = target.join(); // TODO in what case???
      return utils.decodePath(target);
    }
  },
  async updated() {

    var target = this.getTargetPath();
    this.treeData = undefined;
    var data: NavListData = {
      node: OrgTree as NavNode,
      selected: target,
      path: [],
    }
    await this.$nextTick();

    this.treeData = data;
    this.curOpr = identifyCurrentOpr(OrgTree as NavNode, target);
  },
});
</script>


<style scoped>
#subframe {
  overflow: hidden;
}

#sf-left {
  width: var(--leftbar-width);
  float: left;
}

#sf-right {
  float: left;
}
</style>