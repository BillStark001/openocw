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

<script setup lang="ts">
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';
import CourseCard from './CourseBrief.vue';
import { CourseBrief } from '@/api/query';
import { QueryResult } from '@/utils/query';

const props = defineProps<{
  query: QueryResult<CourseBrief[]> | undefined,
  place?: 'db' | 'faculty' | string
}>();

const { t } = useI18n();

const result = computed(() => {
  if (!props.query || typeof props.query !== 'object') {
    return { status: -1, info: 'No Result Assigned' } as QueryResult<CourseBrief[]>;
  }
  return props.query;
});

const placeStr = computed(() => {
  if (props.place === 'db') return 'crsl.noitem.db';
  if (props.place === 'faculty') return 'crsl.noitem.faculty';
  return 'crsl.noitem.search';
});
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