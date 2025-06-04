// src/api/roleApi.js

import api from "./api";

/** Creează un rol nou. */
export async function createRole(req) {
  const { data } = await api.post("/role", req);
  return data;
}

/** Obține un rol după ID. */
export async function getRoleById(roleId) {
  const { data } = await api.get(`/role/${roleId}`);
  return data;
}

/** Obține un rol după nume. */
export async function getRoleByName(roleName) {
  const { data } = await api.get(`/role/by-name/${roleName}`);
  return data;
}

/** Listează toate rolurile. */
export async function getAllRoles() {
  const { data } = await api.get("/role/all");
  return data;
}

/** Listează rolurile paginat. */
export async function getRolesPaged({ pageIndex, pageSize }) {
  const { data } = await api.get("/role/paged", {
    params: { pageIndex, pageSize }
  });
  return data; // PagedResponse<RoleDto>
}

/** Dezactivează (soft-delete) un rol. */
export async function deactivateRole(roleId) {
  await api.delete(`/role/${roleId}`);
}
