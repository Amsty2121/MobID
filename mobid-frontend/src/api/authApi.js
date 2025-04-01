// src/api/authApi.js
import api from "./api";

export async function loginUser(userLoginReq) {
  const response = await api.post("/Auth/login", userLoginReq);
  return response.data;
}

export async function verifyToken() {
  const response = await api.get("/Auth/tokenVerify");
  return response.data;
}
