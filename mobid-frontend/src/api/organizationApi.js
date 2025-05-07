// src/api/organizationApi.js
import api from "./api";

export async function createOrganization(orgData) {
  const { data } = await api.post("/Organization/create", orgData);
  return data;
}

export async function getOrganizationById(orgId) {
  const { data } = await api.get(`/Organization/${orgId}`);
  return data;
}

export async function getAllOrganizations() {
  const { data } = await api.get("/Organization/all");
  return data;
}

export async function getOrganizationsPaged({ pageIndex, pageSize }) {
  const { data } = await api.get("/Organization/paged", { params: { pageIndex, pageSize } });
  return data;
}

export async function updateOrganization(orgData) {
  const { data } = await api.put("/Organization/update", orgData);
  return data;
}

export async function deleteOrganization(orgId) {
  return api.delete(`/Organization/${orgId}`);
}

export async function getUsersForOrganization(orgId) {
  const { data } = await api.get(`/Organization/${orgId}/users`);
  return data;
}

export async function addUserToOrganization(orgId, { userId, role }) {
  // acum trimitem rol în body
  const { data } = await api.post(`/Organization/${orgId}/addUser`, { userId, role });
  return data;
}

export async function removeUserFromOrganization(orgId, userId) {
  const { data } = await api.post(`/Organization/${orgId}/removeUser`, null, { params: { userId } });
  return data;
}