import { UserGroup } from '@/constants';
import {
  getInfo,
  StandardResult,
  QueryResult,
  postInfo,
  TimedResult,
} from '@/utils/query';

export interface UserInfo {
  userId: number;
  userName: string;
  group: UserGroup;
  selectedPersona: string;
  selectedPersonaId: number;
  personaCount: number;
}

export async function getUserInfo() {
  return getInfo<UserInfo>('/api/player/info');
}

export async function requestPasswordChange(oldPwd: string, newPwd: string) {
  return postInfo<StandardResult>('/api/player/pwd/change', { old: oldPwd, new: newPwd });
}

export async function deleteUser(pwd: string) {
  return postInfo<StandardResult>('/api/player/delete', { pwd });
}


// admin parts
export interface LoginRecord {
  macAddress: string;
  machineCode: string;
  date: string | Date;
}

export interface UserInfo2 extends UserInfo {
  deleted: boolean;
  userRefresh: string | Date;
  personaRefresh: string | Date;
  macAddress: string;
  activeBanRecord?: {
    reason: string;
    expiryTime: string;
  };
  logins: LoginRecord[];
}

export async function getUserInfo2(
  id: number
) {
  return getInfo<UserInfo2>(`/api/admin/info/user/${id}`);
}

export async function searchUser(
  pattern: string,
  machineCode?: string
) {
  return getInfo<UserInfo2[]>(`/api/admin/info/user/search`, {
    pattern: pattern,
    machineCode: machineCode,
  });
}

export async function banPlayer(
  id: number,
  span: string,
  reason: string
) {
  return postInfo<UserInfo2>(`/api/admin/ban/`, {
    userId: id,
    banTime: span,
    reason: reason,
  });
}

export async function unbanPlayer(id: number, reason: string) {
  return banPlayer(id, 'unban', reason);
}

export async function bannedUntil(
  type: 'user' | 'person',
  value: number
): Promise<QueryResult<TimedResult>>;
export async function bannedUntil(
  type: 'ip' | 'mac',
  value: string
): Promise<QueryResult<TimedResult>>; // currently useless
export async function bannedUntil(
  type: unknown,
  value: unknown
): Promise<QueryResult<TimedResult>> {
  return getInfo(`/api/admin/ban/until/${type}/${value}`);
}