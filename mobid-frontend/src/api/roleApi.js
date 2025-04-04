// src/api/roleApi.js
import api from "./api";

/**
 * Obține toate rolurile (fără paginare).
 */
export async function getAllRoles() {
  const response = await api.get("/Role/all");
  return response.data;
}

/**
 * Adaugă un nou rol.
 */
export async function addRole(roleName, description) {
  const response = await api.post("/Role/add", null, {
    params: { roleName, description },
  });
  return response.data;
}

/**
 * Șterge un rol.
 */
export async function deleteRole(roleId) {
  const response = await api.delete(`/Role/delete/${roleId}`);
  return response.data;
}

/**
 * Obține un rol după ID.
 */
export async function getRoleById(roleId) {
  const response = await api.get(`/Role/${roleId}`);
  return response.data;
}

/**
 * Obține un rol după nume.
 */
export async function getRoleByName(roleName) {
  const response = await api.get(`/Role/byname/${roleName}`);
  return response.data;
}

/**
 * Obține o listă de roluri paginată de la server, folosind un obiect PagedRequest.
 * @param {{ pageIndex: number, pageSize: number }} pagedRequest
 * @returns { pageIndex, pageSize, total, items } - structura PagedResponse
 */
export async function getRolesPaged(pagedRequest) {
  const response = await api.get("/Role/paged", {
    params: {
      pageIndex: pagedRequest.pageIndex,
      pageSize: pagedRequest.pageSize,
    },
  });
  return response.data;
}
