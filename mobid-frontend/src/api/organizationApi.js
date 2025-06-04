// src/api/organizationApi.js

import api from "./api";

/** Creează o organizație nouă. */
export async function createOrganization(req) {
  const { data } = await api.post("/organization", req);
  return data;
}

/** Obține o organizație după ID. */
export async function getOrganizationById(orgId) {
  const { data } = await api.get(`/organization/${orgId}`);
  return data;
}

/** Listează organizațiile paginat. */
export async function getOrganizationsPaged({ pageIndex, pageSize }) {
  const { data } = await api.get("/organization/paged", {
    params: { pageIndex, pageSize }
  });
  return data; // PagedResponse<OrganizationDto>
}

/** Listează toate organizațiile. */
export async function getAllOrganizations() {
  const { data } = await api.get("/organization/all");
  return data;
}

/** Actualizează o organizație. */
export async function updateOrganization(req) {
  const { data } = await api.patch("/organization", req);
  return data;
}

/** Dezactivează (soft-delete) o organizație. */
export async function deactivateOrganization(orgId) {
  await api.delete(`/organization/${orgId}`);
}

/** Listează membrii unei organizații. */
export async function getUsersForOrganization(orgId) {
  const { data } = await api.get(`/organization/${orgId}/users`);
  return data;
}

/** Adaugă un utilizator într-o organizație. */
export async function addUserToOrganization(orgId, req) {
  await api.post(`/organization/${orgId}/users`, req);
}

/** Elimină un utilizator din organizație. */
export async function removeUserFromOrganization(orgId, userId) {
  await api.delete(`/organization/${orgId}/users/${userId}`);
}

/** Listează accesele proprii ale organizației. */
export async function getAccessesForOrganization(orgId) {
  const { data } = await api.get(`/organization/${orgId}/accesses`);
  return data;
}

/** Listează accesele partajate către organizație. */
export async function getAccessesSharedToOrganization(orgId) {
  const { data } = await api.get(`/organization/${orgId}/accesses/shared`);
  return data;
}

/** Listează toate accesele ale organizației (proprii + partajate). */
export async function getAllOrganizationAccesses(orgId) {
  const { data } = await api.get(`/organization/${orgId}/accesses/all`);
  return data;
}
