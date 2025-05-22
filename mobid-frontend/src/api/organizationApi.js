// src/api/organizationApi.js
import api from "./api";

// POST   /api/Organization
export async function createOrganization(orgData) {
  const { data } = await api.post("/Organization", orgData);
  return data;
}

// GET    /api/Organization/{orgId}
export async function getOrganizationById(orgId) {
  const { data } = await api.get(`/Organization/${orgId}`);
  return data;
}

// GET    /api/Organization/all
export async function getAllOrganizations() {
  const { data } = await api.get("/Organization/all");
  return data;
}

// GET    /api/Organization/paged?pageIndex=&pageSize=
export async function getOrganizationsPaged({ pageIndex, pageSize }) {
  const { data } = await api.get("/Organization/paged", { params: { pageIndex, pageSize } });
  return data;
}

// PUT    /api/Organization/update
export async function updateOrganization(updateReq) {
  const { data } = await api.put("/Organization/update", updateReq);
  return data;
}

// DELETE /api/Organization/{orgId}
export async function deleteOrganization(orgId) {
  return api.delete(`/Organization/${orgId}`);
}

// GET    /api/Organization/{orgId}/users
export async function getUsersForOrganization(orgId) {
  const { data } = await api.get(`/Organization/${orgId}/users`);
  return data;
}

// POST   /api/Organization/{orgId}/users
export async function addUserToOrganization(orgId, { userId, role }) {
  const { data } = await api.post(
    `/Organization/${orgId}/users`,
    { userId, role }
  );
  return data;
}

// DELETE /api/Organization/{orgId}/removeUser/{userId}
export async function removeUserFromOrganization(orgId, userId) {
  return api.delete(`/Organization/${orgId}/removeUser/${userId}`);
}
