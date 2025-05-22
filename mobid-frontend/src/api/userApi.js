// src/api/userApi.js
import api from "./api";

export async function createUser(userAddReq) {
  const { data } = await api.post("/User", userAddReq);
  return data;
}

export async function deactivateUser(userId) {
  return api.delete(`/User/${userId}`);
}

export async function getUserById(userId) {
  const { data } = await api.get(`/User/${userId}`);
  return data;
}

export async function getUsersPaged({ pageIndex, pageSize }) {
  const { data } = await api.get("/User/paged", {
    params: { pageIndex, pageSize }
  });
  return data;
}

export async function assignRoleToUser(userId, roleId) {
  const { data } = await api.post(`/User/${userId}/roles/${roleId}`);
  return data;
}

export async function removeRoleFromUser(userId, roleId) {
  return api.delete(`/User/${userId}/roles/${roleId}`);
}

export async function getUserRoles(userId) {
  const { data } = await api.get(`/User/${userId}/roles`);
  return data;
}