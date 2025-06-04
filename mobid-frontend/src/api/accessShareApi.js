import api from "./api";

/**
 * Preia share-urile dintre două organizații
 * GET /api/OrganizationAccessShare/{sourceOrgId}/to/{targetOrgId}
 * @returns {Promise<OrganizationAccessShareDto[]>}
 */
export async function getSharedAccessesBetweenOrganizations(
  sourceOrganizationId,
  targetOrganizationId
) {
  const { data } = await api.get(
    `/OrganizationAccessShare/${sourceOrganizationId}/to/${targetOrganizationId}`
  );
  return data;
}

/**
 * Partajează un access către o altă organizație.
 * POST /api/OrganizationAccessShare
 * @returns {Promise<boolean>} true dacă s-a partajat (204), false altfel
 */
export async function shareAccessWithOrganization(
  accessId,
  fromOrganizationId,
  toOrganizationId
) {
  try {
    const res = await api.post("/OrganizationAccessShare/grant", {
      accessId,
      fromOrganizationId,
      toOrganizationId
    });
    return res.status === 204;
  } catch (err) {
    if (err.response?.status === 404) {
      return false;
    }
    throw err;
  }
}

/**
 * Revocă un share existent (soft-delete).
 * DELETE /api/OrganizationAccessShare/revoke
 * @returns {Promise<boolean>} true dacă s-a revocat (204), false altfel
 */
export async function revokeSharedAccess(
  accessId,
  fromOrganizationId,
  toOrganizationId
) {
  try {
    const res = await api.delete("/OrganizationAccessShare/revoke", {
      data: { accessId, fromOrganizationId, toOrganizationId }
    });
    return res.status === 204;
  } catch (err) {
    if (err.response?.status === 404) {
      return false;
    }
    throw err;
  }
}
