// src/api/accessShareApi.js
import api from "./api";

/**
 * Shares an access from one org to another.
 * @param {string} sourceOrganizationId
 * @param {string} targetOrganizationId
 * @param {string} accessId
 * @returns {Promise<boolean>}
 */
export async function shareAccessWithOrganization(
  sourceOrganizationId,
  targetOrganizationId,
  accessId
) {
  const payload = { sourceOrganizationId, targetOrganizationId, accessId };
  const res = await api.post("/organizationaccessshare", payload);
  return res.status === 204;
}

/**
 * Revokes a previously shared access.
 * @param {string} sourceOrganizationId
 * @param {string} targetOrganizationId
 * @param {string} accessId
 * @returns {Promise<boolean>}
 */
export async function revokeSharedAccess(
  sourceOrganizationId,
  targetOrganizationId,
  accessId
) {
  const data = { sourceOrganizationId, targetOrganizationId, accessId };
  const res = await api.delete("/organizationaccessshare/revoke", { data });
  return res.status === 204;
}

/**
 * Lists the shares from sourceOrg → targetOrg
 */
export async function getSharedAccessesBetweenOrgs(
  sourceOrganizationId,
  targetOrganizationId
) {
  return api
    .get(
      `/organizationaccessshare/${sourceOrganizationId}/to/${targetOrganizationId}`
    )
    .then((res) => res.data);
}
