<template>
  <div class="course-brief">
    <img class="desc-img" src="@/assets/img/sakura.jpg">
    <p>
      <span class="name">{{name}}</span>
      <span class="sub">{{id}} / {{className}}</span>
    </p>
    <p>
      <span class="tag round-button s-sub" v-for="tag in tags" :key="tag">
      {{tag}}
      </span>
    </p>
    <p class="desc">
      {{description}}
    </p>
    <p>
      <span class="lect round-button s-hollow" v-for="lect in lecturers" :key="lect.id">
      {{lect.name}}
      </span> 
    </p>

  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import { CourseBrief as CourseBriefInfo, defaultCourseBrief } from '@/api/query';


export default defineComponent({
  name: "CourseBrief", 

  data(): CourseBriefInfo {
    if (!!this.$props.info && typeof(this.$props.info) == 'object')
      return this.$props.info as CourseBriefInfo;
    return defaultCourseBrief();
  },

  methods: {

  }, 
  props: ['info']

});
</script>

<style scoped>

.course-brief {
  width: calc(100% - 50px);
  height: max-content;
  margin: 25px;
  box-sizing: border-box;
  padding: 15px;

  background-color: var(--db-panel-color);
  box-shadow: 0 0 17px #00000055;
  overflow: hidden;
}

.course-brief * {
  float: top;
}

.desc-img {
  float: right;
  height: 160px;
  width: 240px;
  
  border-radius: 10px;
  border: 1px solid var(--color-txt-trs3);
}

.name {
  font-size: larger;
  margin-right: 10px;
}

.sub {
  font-size: smaller;
  color: var(--color-txt3);
}

.tag, .lect {
  font-size: smaller;
}

.desc {
  margin: 10px 0;
}

.desc, 
.desc * {
  white-space: pre-wrap;
  font-size: normal;
}



</style>