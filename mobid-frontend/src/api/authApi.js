// src/api/authApi.js

import api from "./api";

export async function loginUser(payload) {
  const { data } = await api.post("/auth/login", payload);
  return data; // UserLoginRsp
}

export async function registerUser(payload) {
  const { data } = await api.post("/auth/register", payload);
  return data; // UserRegisterRsp
}

export async function refreshToken(refreshToken) {
  const { data } = await api.post("/auth/refresh", null, {
    params: { refreshToken }
  });
  return data;
}

export async function revokeToken(refreshToken) {
  await api.post("/auth/revoke", null, {
    params: { refreshToken }
  });
}

export async function verifyToken() {
  await api.get("/auth/token-verify");
}
