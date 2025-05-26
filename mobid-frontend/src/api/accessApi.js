// src/api/accessApi.js

import api from "./api";

/**
 * Creează un nou access.
 * @param {Object} req – AccessCreateReq
 * @returns {Promise<AccessDto>}
 */
export async function createAccess(req) {
  const res = await api.post("/api/access", req);
  return res.data;
}

/**
 * Obține toate accesele unei organizații.
 * @param {string} orgId
 * @param {boolean} includeShared – dacă luăm și accesele partajate
 * @returns {Promise<AccessDto[]>}
 */
export async function getAccessesForOrganization(orgId, includeShared = false) {
  const res = await api.get(`/api/access/organization/${orgId}`, {
    params: { includeShared }
  });
  return res.data;
}

/**
 * Obține o pagină de accese (toate).
 * @param {number} pageIndex
 * @param {number} pageSize
 * @param {boolean} includeShared
 */
export async function getAccessesPaged(pageIndex, pageSize, includeShared = false) {
  const res = await api.get("/api/access", {
    params: { pageIndex, pageSize, includeShared }
  });
  return res.data; // { items: AccessDto[], totalCount, ... }
}

/**
 * Șterge (soft-delete) un access.
 * @param {string} accessId
 */
export async function deactivateAccess(accessId) {
  await api.delete(`/api/access/${accessId}`);
}

export async function getAllAccessTypes() {
  const res = await api.get("/api/accesstype/all");
  return res.data;
}