<template>
  <div id="regDiv" class="blurred-panel">
    <form id="form" action="javascript:void(0);" style="padding: 40px">
      <h1 style="text-align: center">{{ t('reg.title') }}</h1>
      <br>

      <div id="regGrid">
        <p>{{ t('f.uname') }}</p>
        <input id="uname" type="text" v-model="uname" :placeholder="t('f.uname.hint')">
        <p>{{ t('f.pwd') }}</p>
        <input id="pwd" type="password" v-model="pwd" :placeholder="t('f.pwd.hint')">
      </div>

      <br>

      <div v-if="requiring">
        <p>{{ t('hint.requesting') }}</p>
      </div>

      <div v-if="res !== null">
        <div id="status">
          <p>{{ t('hint.code') }}{{ res.code }}</p>
          <p>{{ t('hint.info') }}{{ res.info }}</p>
        </div>
      </div>

      <div style="text-align: center;margin-top: 30px;">
        <button type="submit" class="square-button s-sub h" @click="submit(false)">{{ t('btn.login') }}</button>
        <button type="reset" class="square-button s-sub h" @click="reset">{{ t('btn.reset') }}</button>
        <button type="submit" class="square-button s-sub h" @click="submit(true)">{{ t('btn.reg') }}</button>
      </div>
      <p>{{ t('hint.restr') }}</p>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { AuthResult, requestRegister, requestLogin } from '../api/auth';
import { useI18n } from '../i18n';

const router = useRouter();
const { t } = useI18n();

const uname = ref('');
const pwd = ref('');
const requiring = ref(false);
const res = ref<AuthResult | null>(null);

async function submit(reg: boolean): Promise<void> {
  requiring.value = true;
  res.value = null;
  if (reg) {
    res.value = await requestRegister(uname.value, pwd.value);
  } else {
    res.value = await requestLogin(uname.value, pwd.value);
    if (res.value.code === 0) {
      router.push('/user');
    }
  }
  requiring.value = false;
}

function reset(): void {
  uname.value = '';
  pwd.value = '';
  requiring.value = false;
  res.value = null;
}
</script>

<style scoped>
:root {
  width: 450px;
  display: flex;
  justify-content: center;
  align-items: center;
}

p {
  margin: 10px 0px;
}

input {
  margin: 0px 10px;
  padding: 0px 10px;
  border-radius: 5px;
  border-style: hidden;
  height: 30px;
  min-width: 140px;
  background-color: var(--color-txt-trs3);
  outline: none;
}

table {
  align-self: center;
  margin: auto;
}

#status {
  width: 100%;
  text-align: left;
}

#regGrid {
  display: grid;
  grid-template-columns: auto 1fr;
  justify-items: stretch;
  align-items: center;
  grid-gap: 2px;
}
</style>