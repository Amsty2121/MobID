// src/api/authApi.js
import api from "./api";

export async function loginUser(payload) {
  const res = await api.post("/api/auth/login", payload);
  return res.data; // { id, username, token, refreshToken }
}

export async function registerUser(payload) {
  const res = await api.post("/api/auth/register", payload);
  return res.data;
}

export async function refreshToken(refreshToken) {
  const res = await api.post("/api/auth/refresh", null, {
    params: { refreshToken }
  });
  return res.data;
}

export async function revokeToken(refreshToken) {
  await api.post("/api/auth/revoke", null, {
    params: { refreshToken }
  });
}

export async function verifyToken() {
  await api.get("/api/auth/token-verify");
}
