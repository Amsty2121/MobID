// src/api/organizationApi.js
import api from "./api";


/**
 * Paginated fetch
 * GET /api/organization/paged?pageIndex=0&pageSize=10
 */
export async function getOrganizationsPaged({ pageIndex, pageSize }) {
  const response = await api.get("/organization/paged", {
    params: { pageIndex, pageSize }
  });
  return response.data;
}

/**
 * Create new organization
 * POST /api/organization
 */
export async function createOrganization(payload) {
  const response = await api.post("/organization", payload);
  return response.data;
}

/**
 * Update organization
 * PATCH /api/organization
 */
export async function updateOrganization(payload) {
  const response = await api.patch("/organization", payload);
  return response.data;
}

/**
 * Deactivate (soft-delete)
 * DELETE /api/organization/{id}
 */
export async function deactivateOrganization(id) {
  const response = await api.delete(`/organization/${id}`);
  return response.data;
}

/** Obține o organizație după ID */
export async function getOrganizationById(orgId) {
  const res = await api.get(`/organization/${orgId}`);
  return res.data;
}

/** Listează toate organizațiile */
export async function getAllOrganizations() {
  const res = await api.get("/organization/all");
  return res.data;
}

/** Adaugă un utilizator într-o organizație */
export async function addUserToOrganization(orgId, req) {
  await api.post(`/organization/${orgId}/users`, req);
}

/** Elimină un utilizator din organizație */
export async function removeUserFromOrganization(orgId, userId) {
  await api.delete(`/organization/${orgId}/users/${userId}`);
}

/** Listează membrii unei organizații */
export async function getUsersForOrganization(orgId) {
  const res = await api.get(`/organization/${orgId}/users`);
  return res.data;
}

/** Listează share-urile primite de organizație */
export async function getAccessesSharedToOrganization(orgId) {
  // ruta originală era GET api/organization/organization/{orgId},
  // ideal ar fi să o schimbi în GET api/organization/{orgId}/shares
  const res = await api.get(`/organization/organization/${orgId}`);
  return res.data;
}

/** Obține toate accesele (proprii + partajate) */
export async function getAllOrganizationAccesses(orgId) {
  const res = await api.get(`/organization/${orgId}/all-accesses`);
  return res.data;
}
