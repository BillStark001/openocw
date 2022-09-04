<template>
  <div class="course-list">

    <div class="res" v-if="result.status != 200">
      <p>{{ t('crsl.noitem.error') }}{{ result.status }} / {{ result.info }}</p>
    </div>
    <div class="res" v-if="result.status == 200 && (!result.result || result.result.length == 0)">
      <p>{{ t(placeStr) }}</p>
    </div>
    <div class="real-res" v-if="result.result && result.result.length > 0">
      <CourseCard v-for="course in result.result" :key="course.id" :info="course"></CourseCard>
    </div>

  </div>
</template>


<script lang="ts">
import { CourseBrief } from '@/api/query';
import { QueryResult } from '@/utils/query';
import { defineComponent } from 'vue';
import { useI18n } from 'vue-i18n';
import CourseCard from './CourseBrief.vue';


export interface CourseListData {
  result: QueryResult<CourseBrief[]>,
  placeStr: string
}

export default defineComponent({
  name: "CourseList",
  data(): CourseListData {
    var res: QueryResult<CourseBrief[]>;
    if (!this.$props.query || typeof (this.$props.query) != 'object') {
      res = { status: -1, info: 'No Result Assigned', }
    }
    res = this.$props.query as QueryResult<CourseBrief[]>;

    var placeStr = 'crsl.noitem.search';
    if (this.$props.place == 'db')
      placeStr = 'crsl.noitem.db';
    else if (this.$props.place == 'faculty')
      placeStr = 'crsl.noitem.faculty';

    return {
      result: res,
      placeStr: placeStr
    };
  },
  setup() {
    const { t } = useI18n();
    return { t };
  },
  components: {
    CourseCard
  },
  props: ['query', 'place'],
})

</script>

<style scoped>

.course-list {
  width: 100%;
  height: 100%;
  position: relative;
  text-align: center;
}

.res {
  width: 100%;
  height: max-content;
  text-align: center;
  padding: 50px 20px;
  box-sizing: border-box;
  font-size: larger;
}

.real-res {
  width: 100%;
  height: max-content;
  text-align: justify;
}

</style>