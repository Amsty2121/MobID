// src/api/roleApi.js
import api from "./api";

/** Creează rol */
export async function createRole(payload) {
  const { data } = await api.post("/role", payload);
  return data;
}

/** Obține rol după ID */
export async function getRoleById(roleId) {
  const { data } = await api.get(`/role/${roleId}`);
  return data;
}

/** Obține rol după nume */
export async function getRoleByName(roleName) {
  const { data } = await api.get(`/role/by-name/${roleName}`);
  return data;
}

/** Listează toate rolurile */
export async function getAllRoles() {
  const { data } = await api.get("/role/all");
  return data;
}

/** Listează roluri paginat */
export async function getRolesPaged(params) {
  const { data } = await api.get("/role/paged", { params });
  return data;
}

/** Dezactivează rol */
export async function deactivateRole(roleId) {
  const response = await api.delete(`/role/${roleId}`);
  return response.status === 204;
}
