// src/api/accessShareApi.js
import api from "./api";

/**
 * Partajează un access către o altă organizație.
 * @param {string} accessId
 * @param {string} fromOrganizationId
 * @param {string} toOrganizationId
 */
export async function shareAccessWithOrganization(accessId, fromOrganizationId, toOrganizationId) {
  const req = { accessId, fromOrganizationId, toOrganizationId };
  const { data } = await api.post("/AccessShare", req);
  return data;
}

/**
 * Revocă un share existent (soft-delete).
 * @param {string} accessId
 * @param {string} fromOrganizationId
 * @param {string} toOrganizationId
 */
export async function revokeSharedAccess(accessId, fromOrganizationId, toOrganizationId) {
  const req = { accessId, fromOrganizationId, toOrganizationId };
  // Axios DELETE cu body se trimite în { data }
  await api.delete("/AccessShare/revoke", { data: req });
}

/**
 * Preia toate share-urile pentru o organizație țintă.
 * @param {string} organizationId
 */
export async function getSharesForOrganization(organizationId) {
  const { data } = await api.get(`/AccessShare/organization/${organizationId}`);
  return data;
}
