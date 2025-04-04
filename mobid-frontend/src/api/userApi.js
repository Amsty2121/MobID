// src/api/userApi.js
import api from "./api";

export async function addUser(userAddReq) {
  // userAddReq este un obiect cu proprietăți: email, username, password
  const response = await api.post("/User/add", userAddReq);
  return response.data;
}

export async function deleteUser(userId) {
  const response = await api.delete(`/User/${userId}`);
  return response.data;
}

export async function getUserById(userId) {
  const response = await api.get(`/User/${userId}`);
  return response.data;
}

export async function getUsersPaged(pagedRequest) {
  // pagedRequest: { pageIndex, pageSize }
  const response = await api.get("/User/paged", {
    params: {
      pageIndex: pagedRequest.pageIndex,
      pageSize: pagedRequest.pageSize,
    },
  });
  return response.data;
}

export async function assignRoleToUser(userId, roleId) {
    const response = await api.post("/User/assignRole", null, {
      params: { userId, roleId },
    });
    return response.data;
}

export async function removeRoleFromUser(userId, roleId) {
    const response = await api.post("/User/removeRole", null, {
      params: { userId, roleId },
    });
    return response.data;
}

export async function getUserRoles(userId) {
    const response = await api.get(`/User/${userId}/roles`);
    return response.data;
}

export async function getAllRoles() {
    const response = await api.get("/Role/all");
    return response.data;
}
