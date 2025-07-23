// src/api/accessApi.js
import api from "./api";

/**
 * Creează un access nou.
 * @param {object} payload – AccessCreateReq sau AccessUpdateReq
 * @returns {Promise<AccessDto>}
 */
export async function createAccess(payload) {
  const { data } = await api.post("/access", payload);
  return data;
}

/**
 * Obține un access după ID.
 * @param {string} accessId
 * @returns {Promise<AccessDto>}
 */
export async function getAccessById(accessId) {
  const { data } = await api.get(`/access/${accessId}`);
  return data;
}

/**
 * Obține toate accesele pentru o organizație.
 * @param {string} organizationId
 * @returns {Promise<AccessDto[]>}
 */
export async function getAccessesForOrganization(organizationId) {
  const { data } = await api.get(`/access/organization/${organizationId}`);
  return data;
}

/**
 * Obține toate accesele din sistem.
 * @returns {Promise<AccessDto[]>}
 */
export async function getAllAccesses() {
  const { data } = await api.get("/access/all");
  return data;
}

/**
 * Actualizează un access existent.
 * @param {object} payload – AccessUpdateReq cu proprietatea `id`
 * @returns {Promise<AccessDto>}
 */
export async function updateAccess(payload) {
  const { data } = await api.put(`/access/${payload.id}`, payload);
  return data;
}

/**
 * Dezactivează (soft-delete) un access.
 * @param {string} accessId
 * @returns {Promise<boolean>} – true dacă a returnat 204
 */
export async function deactivateAccess(accessId) {
  const response = await api.delete(`/access/${accessId}`);
  return response.status === 204;
}
