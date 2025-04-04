// src/api/organizationApi.js
import api from "./api";

export async function createOrganization(orgData) {
  // orgData: { name, ownerId }
  const response = await api.post("/Organization/create", orgData);
  return response.data;
}

export async function getOrganizationById(orgId) {
  const response = await api.get(`/Organization/${orgId}`);
  return response.data;
}

export async function getAllOrganizations() {
  const response = await api.get("/Organization/all");
  return response.data;
}

export async function getOrganizationsPaged(pagedRequest) {
  const response = await api.get("/Organization/paged", {
    params: {
      pageIndex: pagedRequest.pageIndex,
      pageSize: pagedRequest.pageSize,
    },
  });
  return response.data;
}

export async function addUserToOrganization(orgId, userId) {
  const response = await api.post(`/Organization/${orgId}/addUser`, null, {
    params: { userId },
  });
  return response.data;
}

export async function removeUserFromOrganization(orgId, userId) {
  const response = await api.post(`/Organization/${orgId}/removeUser`, null, {
    params: { userId },
  });
  return response.data;
}

export async function getUsersForOrganization(orgId) {
  const response = await api.get(`/Organization/${orgId}/users`);
  return response.data;
}

export async function updateOrganization(orgData) {
    const response = await api.put("/Organization/update", orgData);
    return response.data;
  }
  
  export async function deleteOrganization(orgId) {
    const response = await api.delete(`/Organization/${orgId}`);
    return response.data;
  }