// src/api/organizationApi.js
import api from "./api";

/** Creează o organizație */
export async function createOrganization(payload) {
  const { data } = await api.post("/organization", payload);
  return data;
}

/** Obține o organizație după ID */
export async function getOrganizationById(id) {
  const { data } = await api.get(`/organization/${id}`);
  return data;
}

/** Actualizează o organizație */
export async function updateOrganization(payload) {
  const { data } = await api.patch("/organization", payload);
  return data;
}

/** Dezactivează (soft-delete) o organizație */
export async function deactivateOrganization(id) {
  const response = await api.delete(`/organization/${id}`);
  return response.status === 204;
}

/** Listează organizațiile paginat */
export async function getOrganizationsPaged(params) {
  const { data } = await api.get("/organization/paged", { params });
  return data;
}

/** Listează toate organizațiile */
export async function getAllOrganizations() {
  const { data } = await api.get("/organization/all");
  return data;
}


//Users
/** Adaugă user în organizație */
export async function addUserToOrganization(orgId, payload) {
  const response = await api.post(`/organization/${orgId}/users`, payload);
  return response.status === 204;
}

/** Înlătură user din organizație */
export async function removeUserFromOrganization(orgId, userId) {
  const response = await api.delete(`/organization/${orgId}/users/${userId}`);
  return response.status === 204;
}

/** Obține membrii organizației */
export async function getUsersForOrganization(orgId) {
  const { data } = await api.get(`/organization/${orgId}/users`);
  return data;
}


//Access
/** Accese proprii ale organizației */
export async function getOrganizationAccesses(orgId) {
  const { data } = await api.get(`/organization/${orgId}/accesses`);
  return data;
}

/** Accese partajate către organizație */
export async function getSharedAccessesToOrganization(orgId) {
  const { data } = await api.get(`/organization/${orgId}/accesses/shared`);
  return data;
}

/** Toate accesele (proprii + partajate) */
export async function getAllOrganizationAccesses(orgId) {
  const { data } = await api.get(`/organization/${orgId}/accesses/all`);
  return data;
}
