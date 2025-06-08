// src/api/userApi.js
import api from "./api";

/** Creează utilizator */
export async function createUser(payload) {
  const { data } = await api.post("/user", payload);
  return data;
}

/** Obține user după ID */
export async function getUserById(userId) {
  const { data } = await api.get(`/user/${userId}`);
  return data;
}

/** Listează useri paginat */
export async function getUsersPaged(params) {
  const { data } = await api.get("/user/paged", { params });
  return data;
}

/** Dezactivează user (soft-delete) */
export async function deactivateUser(userId) {
  const response = await api.delete(`/user/${userId}`);
  return response.status === 204;
}

/** Listează rolurile userului */
export async function getUserRoles(userId) {
  const { data } = await api.get(`/user/${userId}/roles`);
  return data;
}

/** Atribuie rol userului */
export async function assignRoleToUser(userId, roleId) {
  const response = await api.post(`/user/${userId}/roles/${roleId}`);
  return response.status === 200;
}

/** Elimină rol userului */
export async function removeRoleFromUser(userId, roleId) {
  const response = await api.delete(`/user/${userId}/roles/${roleId}`);
  return response.status === 204;
}

/** Listează toate accesele asociate unui user (direct, prin organizație și prin share) */
export async function getAllUserAccesses(userId) {
  const { data } = await api.get(`/user/${userId}/all-accesses`);
  return data;
}

/** Listează toate organizațiile din care face parte userul */
export async function getUserOrganizations(userId) {
  const { data } = await api.get(`/user/${userId}/organizations`);
  return data; // List<OrganizationDto>
}