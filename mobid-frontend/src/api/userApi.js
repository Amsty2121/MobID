// src/api/userApi.js

import api from "./api";

/** Creează un utilizator nou. */
export async function createUser(req) {
  const { data } = await api.post("/user", req);
  return data;
}

/** Obține un utilizator după ID. */
export async function getUserById(userId) {
  const { data } = await api.get(`/user/${userId}`);
  return data;
}

/** Listează utilizatorii paginat. */
export async function getUsersPaged({ pageIndex, pageSize }) {
  const { data } = await api.get("/user/paged", {
    params: { pageIndex, pageSize }
  });
  return data; // PagedResponse<UserDto>
}

/** Dezactivează (soft-delete) un utilizator. */
export async function deactivateUser(userId) {
  await api.delete(`/user/${userId}`);
}

/** Listează rolurile unui utilizator. */
export async function getUserRoles(userId) {
  const { data } = await api.get(`/user/${userId}/roles`);
  return data;
}

/** Atribuie un rol unui utilizator. */
export async function assignRoleToUser(userId, roleId) {
  await api.post(`/user/${userId}/roles/${roleId}`);
}

/** Înlătură un rol de la un utilizator. */
export async function removeRoleFromUser(userId, roleId) {
  await api.delete(`/user/${userId}/roles/${roleId}`);
}
