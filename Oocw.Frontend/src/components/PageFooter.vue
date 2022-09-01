<template>
  <footer>
    <div class="footer">
      <div class="footer-inner">
        <Logo size="120px" fontSize="70px" />
        <span class="v-div" />
        <div>
          <button class="round-button s-main h" > button1</button>
          <div class="round-button s-sub h"> button2</div>
          <div class="square-button s-hollow h"> button3</div>
          <div class="round-button s-main h"> button4</div>

        </div>
        <span class="v-div" />
        <div>info</div>
        <span class="v-div" />
        <div>links</div>
      </div>
    </div>
  </footer>
</template>

<script lang="tsx">
import { defineComponent } from 'vue';

export default defineComponent({

  methods: {
    
  }


});


</script>

<style scoped>
.footer {
  width: 100%;
  min-height: var(--footer-height);
  background-color: var(--color-bg3);
  color: var(--color-txt1);
  float: bottom;
  display: flex;
  align-items: center;
  flex-direction: column;
}

.footer .footer-inner {
  float: top;
  margin: 0 auto;
  max-width: var(--page-content-max-width);
  box-sizing: border-box;
  padding: 20px;
  display: flex;
  align-items: center;
}

.footer footer-inner>* {
  float: left;
}

.footer .v-div {
  height: max-content;
  width: 1px;
  height: 200px;
  background-color: rgb(102, 102, 102);
  margin: 0 8px;
}
</style>