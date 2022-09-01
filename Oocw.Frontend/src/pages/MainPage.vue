<template>
  <PageFrame>
    <!--img src="@/assets/img/banner.jpg" class="banner"-->
    <RouteDisplay/>
    <div id="notice">
      <div>
        <img alt="logo" src="/assets/svg/logo.svg">
        <p>
          {{ t('product.name') }}
        </p>

      </div>
      <br />
      <div id="notice-disp">
        {{ t('notice.intest') }}
      </div>

    </div>
  </PageFrame>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { useI18n } from 'vue-i18n'
import PageFrame from '@/components/PageFrame.vue'
import RouteDisplay from '@/components/RouteDisplay.vue'



interface MainPageData {
  
}

export default defineComponent({
  name: "MainPage",
  setup() {
    const { t } = useI18n({
      inheritLocale: true,
      useScope: "local"
    });
    return { t };
  },
  data(): MainPageData {
    return {

    };
  },
  methods: {

  },
  mounted() {

  },
  components: { 
    PageFrame, 
    RouteDisplay 
  }
})
</script>


<style scoped>
#notice {
  font-size: larger;
  padding: 7.5% 7.5%;
  text-align: center;
}

#notice #notice-disp {
  white-space: pre-wrap;
  text-align: left;
}

#btn-dl {
  padding: 10px 40px;
}

#btn-dl p:first-child {
  margin: 3px;
  font-size: 30px;
}



</style>