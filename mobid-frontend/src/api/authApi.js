// src/api/authApi.js
import api from "./api";

/** Login (JWT + refresh) */
export async function loginUser(payload) {
  const { data } = await api.post("/auth/login", payload);
  return data;
}

/** Înregistrare utilizator */
export async function registerUser(payload) {
  const { data } = await api.post("/auth/register", payload);
  return data;
}

/** Reîmprospătare token */
export async function refreshToken(refreshToken) {
  const { data } = await api.post("/auth/refresh", null, { params: { refreshToken } });
  return data;
}

/** Revocare token */
export async function revokeToken(refreshToken) {
  const response = await api.post("/auth/revoke", null, { params: { refreshToken } });
  return response.status === 204;
}

/** Verificare validitate token */
export async function verifyToken() {
  await api.get("/auth/token-verify");
}
