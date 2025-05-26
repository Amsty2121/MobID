// src/api/accessTypeApi.js

import api from "./api";

/** Listează toate tipurile de acces */
export async function getAllAccessTypes() {
  const res = await api.get("/api/accesstype/all");
  return res.data;
}