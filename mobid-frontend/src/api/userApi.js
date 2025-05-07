// src/api/userApi.js
import api from "./api";

export async function addUser(userAddReq) {
  const { data } = await api.post("/User/add", userAddReq);
  return data;
}

export async function deleteUser(userId) {
  return api.delete(`/User/${userId}`);
}

export async function getUserById(userId) {
  const { data } = await api.get(`/User/${userId}`);
  return data;
}

export async function getUsersPaged({ pageIndex, pageSize }) {
  const { data } = await api.get("/User/paged", { params: { pageIndex, pageSize } });
  return data;
}

export async function assignRoleToUser(userId, roleId) {
  const { data } = await api.post("/User/assignRole", null, { params: { userId, roleId } });
  return data;
}

export async function removeRoleFromUser(userId, roleId) {
  const { data } = await api.post("/User/removeRole", null, { params: { userId, roleId } });
  return data;
}

export async function getUserRoles(userId) {
  const { data } = await api.get(`/User/${userId}/roles`);
  return data;
}