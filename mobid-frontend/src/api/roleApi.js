// src/api/roleApi.js
import api from "./api";

export async function getRolesPaged({ pageIndex, pageSize }) {
  const { data } = await api.get("/Role/paged", { params: { pageIndex, pageSize } });
  return data;
}

export async function addRole(name, description) {
  const { data } = await api.post("/Role", { name, description });
  return data;
}

export async function deleteRole(roleId) {
  return api.delete(`/Role/${roleId}`);
}

export async function getRoleById(roleId) {
  const { data } = await api.get(`/Role/${roleId}`);
  return data;
}

export async function getRoleByName(roleName) {
  const { data } = await api.get(`/Role/byname/${roleName}`);
  return data;
}

export async function getAllRoles() {
  const { data } = await api.get("/Role/all");
  return data;
}
