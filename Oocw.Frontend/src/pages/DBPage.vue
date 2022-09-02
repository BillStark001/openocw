<template>
  <PageFrame>
    <PageBanner />
    <RouteDisplay v-if="treeData"></RouteDisplay>
    <div v-if="!($route.params.target)" class="org-area">
      <div v-for="org in orgs" :key="org.key" class="org-panel">
        <router-link :to="'/db/' + (org.key)">
          <img v-if="org.key=='org.sos'" src="@/assets/svg/tit/chem.svg">
          <img v-if="org.key=='org.soe'" src="@/assets/svg/tit/robotic-arm.svg">
          <img v-if="org.key=='org.somct'" src="@/assets/svg/tit/mat.svg">
          <img v-if="org.key=='org.soc'" src="@/assets/svg/tit/chip.svg">
          <img v-if="org.key=='org.solst'" src="@/assets/svg/tit/bio.svg">
          <img v-if="org.key=='org.soes'" src="@/assets/svg/tit/city.svg">
          <img v-if="org.key=='crs.la'" src="@/assets/svg/tit/art.svg">
          <img v-if="org.key=='org.ext.trsdis'" src="@/assets/svg/tit/idea.svg">
          <p>{{ t(org.key) }}</p>
        </router-link>
      </div>
    </div>
    <div id="subframe" v-if="($route.params.target)">
      <div id="sf-left">
        <NavList v-if="treeData" v-bind:data="treeData"></NavList>
      </div>
      <div id="sf-right">
        test right subframe
        <CourseBrief></CourseBrief>
        <CourseBrief></CourseBrief>
        <CourseBrief></CourseBrief>
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
import CourseBrief from '@/components/courses/CourseBrief.vue';

import OrgTree from '@/assets/meta/orgtree.json';
import Orgs from '@/assets/meta/orgs.json';
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

interface Org {
  key: string, 
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
    const orgs = Orgs as Org[];
    // Something todo ..
    return { t, orgs };
  },
  components: {
    PageFrame,
    RouteDisplay,
    NavList,
    PageBanner, 
    PageFooter, 
    CourseBrief, 
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
  box-sizing:border-box;
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