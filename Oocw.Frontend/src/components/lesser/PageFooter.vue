<template>
  <footer>
    <div v-bind:class="clsx('footer', !!props.static && 'static')">
      <div class="footer-inner">
        <Logo size="120px" fontSize="70px" />
        <span class="v-div" />
        <div>
          <button class="round-button s-main h"> button1</button>
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

<script setup lang="ts">
import clsx from 'clsx';


const props = defineProps < {
  static?: boolean
} > ();

</script>

<style scoped>
.footer {
  width: 100%;
  height: var(--footer-height);
  background-color: var(--footer-background-color);
  color: var(--color-txt1);
  float: bottom;
  display: flex;
  align-items: center;
  flex-direction: column;
}

.footer.static {
  position: fixed;
  left: 0px;
  bottom: 0px;
  background-color: var(--color-bg-trs3);
  backdrop-filter: blur(10px);
  box-shadow: 0 0 17px #00000077;
}

.footer .footer-inner {
  float: top;
  margin: 0 auto;
  max-width: var(--page-content-max-width);
  box-sizing: border-box;
  padding: 16px;
  display: flex;
  align-items: center;
}

.footer footer-inner>* {
  float: left;
}

.footer .v-div {
  height: max-content;
  width: 1px;
  height: calc(var(--footer-height) - 32px);
  background-color: rgb(102, 102, 102);
  margin: 0 8px;
}
</style>