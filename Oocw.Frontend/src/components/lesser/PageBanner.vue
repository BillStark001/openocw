<template>
  <div class="banner">
    <img class="bg-img" src="@/assets/banner/banner4.jpg">
    <div class="fg-txt">
      <p>Gan Si Huang Xu Dong</p>
      <p>Beat Xudong Huang To Death.</p>
    </div>
    
  </div>
  
</template>

<script lang="ts">
import { defineComponent } from 'vue';

export default defineComponent({

  name: "PageBanner", 
})

</script>

<style scoped>

.banner {
  width: 100%; 
  height: var(--banner-height);
  box-sizing: border-box;
  display: block;
  position: relative;
  overflow: hidden;
}

.bg-img {
  position:absolute;
  left: 50%;
  top: 50%;
  transform: translate(-50%, -50%);
}

@media screen and (min-width: 1201px) {
  .bg-img {
    width: 100%;
    height: auto;
  }
}

@media screen and (max-width: 1200px) {
  .bg-img {
    height: 100%;
  }
}

.fg-txt {
  position: absolute;
  bottom: 0px;
  left: 0px;
  text-align: initial;
  margin: 8px;
  padding: 8px;
  max-width: 600px;

  border-radius: 8px;
  backdrop-filter: blur(2px);
  background-color: color-bg-trs3;
}

.fg-txt>p {
  font-size:small;
  text-shadow: 0px 0px 8px #000000;
  color: white;
}
.fg-txt>p:first-child {
  font-size:larger;
  padding: 5px 0;
}


</style>