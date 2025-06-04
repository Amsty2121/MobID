// src/api/accessApi.js

import api from "./api";

/** Creează un nou access. */
export async function createAccess(req) {
  const { data } = await api.post("/access", req);
  return data;
}

/** Obține un access după ID. */
export async function getAccessById(accessId) {
  const { data } = await api.get(`/access/${accessId}`);
  return data;
}

/** Listează toate accesele cu paginare. */
export async function getAccessesPaged({ pageIndex, pageSize }) {
  const { data } = await api.get("/access", {
    params: { pageIndex, pageSize }
  });
  return data; // PagedResponse<AccessDto>
}

/** Listează accesele proprii ale unei organizații. */
export async function getAccessesForOrganization(orgId) {
  const { data } = await api.get(`/organization/${orgId}/accesses`);
  return data;
}

/** Listează accesele partajate către o organizație. */
export async function getAccessesSharedToOrganization(orgId) {
  const { data } = await api.get(`/organization/${orgId}/accesses/shared`);
  return data;
}

/** Listează toate accesele (proprii + partajate) ale organizației. */
export async function getAllOrganizationAccesses(orgId) {
  const { data } = await api.get(`/organization/${orgId}/accesses/all`);
  return data;
}

/** Dezactivează (soft-delete) un access. */
export async function deactivateAccess(accessId) {
  await api.delete(`/access/${accessId}`);
}

/** Listează toate tipurile de acces */
export async function getAllAccessTypes() {
  const res = await api.get("/accesstype/all");
  return res.data;
}