// src/api/accessApi.js

import api from "./api";

/**
 * Creează un acces nou.
 * @param {{ organizationId: string, accessTypeId: string, expirationDate?: string }} req
 */
export async function createAccess(req) {
    const { data } = await api.post("/Access/create", req);
    return data;
  }
  
  export async function getAccessById(accessId) {
    const { data } = await api.get(`/Access/${accessId}`);
    return data;
  }
  
  export async function getAccessesForOrganization(orgId) {
    const { data } = await api.get(`/Access/organization/${orgId}`);
    return data;
  }
  
  export async function getAccessesPaged({ pageIndex, pageSize }) {
    const { data } = await api.get("/Access/paged", { params: { pageIndex, pageSize } });
    return data;
  }
  
  export async function deactivateAccess(accessId) {
    return api.delete(`/Access/${accessId}`);
  }


export async function getAccessTypes() {
  const { data } = await api.get("/AccessType/All");
  return data;
}

/**
 * Obține o listă paginată de tipuri de acces.
 * @param {{ pageIndex: number, pageSize: number }} pagedRequest
 */
export async function getAccessTypesPaged({ pageIndex, pageSize }) {
  const { data } = await api.get("/AccessType/paged", {
    params: { pageIndex, pageSize },
  });
  return data;
}

/**
 * Obține un tip de acces după ID.
 * @param {string} typeId
 */
export async function getAccessTypeById(typeId) {
  const { data } = await api.get(`/AccessType/${typeId}`);
  return data;
}