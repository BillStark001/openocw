<!-- eslint-disable no-irregular-whitespace -->
<template>
  <div id="regDiv" class="blurred-panel">
    <form id="form" action="javascript:void(0);" style="padding: 40px">

      <h1 style="text-align: center">{{ t('reg.title') }}</h1>
      <br>

      <table>
        <tr>
          <td>
            <p>{{ t('f.uname') }}</p>
          </td>
          <td><input id="uname" type="text" v-model="uname" v-bind:placeholder="t('f.uname.hint')"><label
              id="uname_l"></label></td>
        </tr>
        <tr>
          <td>
            <p>{{ t('f.pwd') }}</p>
          </td>
          <td><input id="pwd" type="password" v-model="pwd" v-bind:placeholder="t('f.pwd.hint')"><label
              id="pwd_l"></label></td>
        </tr>
      </table>
      <br>

      <div v-if="requiring">
        <p>{{ t('hint.requesting') }}</p>
      </div>

      <div v-if="res != null">
        <div id="status">
          <p>{{ t('hint.code') }}{{ res.code }}</p>
          <p>{{ t('hint.info') }}{{ res.info }}</p>
        </div>
      </div>

      <div style="text-align: center;margin-top: 30px;">
        <button type="submit" class="square-button s-sub h" v-on:click="submit(false)">{{ t('btn.login') }}</button>
        <button type="reset" class="square-button s-sub h" v-on:click="reset">{{ t('btn.reset') }}</button>
        <button type="submit" class="square-button s-sub h" v-on:click="submit(true)">{{ t('btn.reg') }}</button>
      </div>
      <p>{{ t('hint.restr') }}</p>
    </form>
  </div>
</template>

<script lang="ts">

import { defineComponent } from 'vue';
import { AuthResult, requestRegister, requestLogin } from '../api/auth';
import { useI18n } from '../i18n';

interface PanelData {
  uname: string,
  pwd: string,
  requiring: boolean,
  res: AuthResult | null
}

export default defineComponent({
  data(): PanelData {
    return {
      uname: '',
      pwd: '',
      requiring: false,
      res: null,
    };
  },
  setup() {
    const { t } = useI18n();
    return { t };
  },
  methods: {
    async submit(reg: boolean): Promise<void> {
      this.requiring = true;
      this.res = null;
      if (reg) {
        this.res = await requestRegister(this.uname, this.pwd);
        this.requiring = false;
      } else {
        this.res = await requestLogin(this.uname, this.pwd);
        this.requiring = false;
        if (this.res.code == 0)
          this.$router.push('/user');
      }
      
    },
    reset(): void {
      this.uname = '';
      this.pwd = '';
      this.requiring = false;
      this.res = null;
    }
  }
});

</script>

<style>
#regDiv {
  width: 450px;
  display: flex;
  justify-content: center;
  align-items: center;
}

#regDiv p {
  margin: 10px 0px;
}

#regDiv input {
  margin: 0px 10px;
  padding: 0px 10px;
  border-radius: 5px;
  border-style: hidden;
  height: 30px;
  width: 140px;
  background-color: var(--color-txt-trs3);
  outline: none;
}

#regDiv table {
  align-self: center;
  margin: auto;
}

#regDiv #status {
  width: 100%;
  text-align: left;
}
</style>