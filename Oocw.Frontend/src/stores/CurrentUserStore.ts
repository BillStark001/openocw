import { InjectionKey } from "vue";
import { inject, onMounted, provide, reactive } from "vue";
import type { UserInfo, UserInfo2 } from "@/api/user";
import * as UserApi from '@/api/user';
import * as AuthApi from "@/api/auth";
import { UserGroup } from "@/constants";
import { Settings } from "@/utils/settings";

export enum CurrentUserStoreState {
  Initial,
  CheckingLogin,
  LoadingBaseInfo,
  LoadingAdminInfo,
  Done,
}

export class CurrentUserStore {
  baseInfo: UserInfo | null = null;
  adminInfo: UserInfo2 | null = null;
  state = CurrentUserStoreState.Initial;
  private _isLoggedIn: boolean | null = Settings.isLoggedIn;

  get isLogin() {
    if (this._isLoggedIn !== null) return this._isLoggedIn;
    return this.state === CurrentUserStoreState.Done && this.baseInfo != null;
  }

  get isAdmin() {
    return this.isLogin && this.baseInfo?.group === UserGroup.Admin;
  }

  async fetchInfo(force = false) {
    if ((this.baseInfo || this.state > CurrentUserStoreState.Initial) && !force) return;

    if (this._isLoggedIn === null) {
      // 未知登录状态，需要请求 api 判断
      this.state = CurrentUserStoreState.CheckingLogin;
      this._isLoggedIn = await AuthApi.isLoggedIn();
      Settings.isLoggedIn = this._isLoggedIn;
    } else {
      // 已知登录状态，直接保存
      this._isLoggedIn = Settings.isLoggedIn;
    }

    if (this._isLoggedIn === true) {

      this.state = CurrentUserStoreState.LoadingBaseInfo;
      const { status, result: user } = await UserApi.getUserInfo();

      if (status === 200 && user) {
        this.baseInfo = user;


        if (user.group === UserGroup.Admin) {
          this.state = CurrentUserStoreState.LoadingAdminInfo;
          const { status, result: adminInfo } = await UserApi.getUserInfo2(user.userId);
          if (status === 200 && adminInfo) {
            this.adminInfo = adminInfo;
          }
        }
      } else {
        this.clear();
        return;
      }
    }

    this.state = CurrentUserStoreState.Done;
  }


  async clear() {
    this._isLoggedIn = false;
    Settings.isLoggedIn = false;
    await AuthApi.tryLogOut();
    this.baseInfo = null;
    this.adminInfo = null;
    this.state = CurrentUserStoreState.Initial;
  }
}


const key = Symbol('CurrentUserStore') as InjectionKey<CurrentUserStore>;

export function provideCurrentUserStore() {
  const store = new CurrentUserStore();
  provide(key, reactive(store) as CurrentUserStore);
  return store;
}

export function useCurrentUserStore() {
  const store = inject(key);
  if (!store) throw new Error('Failed to inject user store.');
  onMounted(() => store.fetchInfo());
  return store;
}