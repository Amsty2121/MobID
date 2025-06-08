// src/api/accessTypeApi.js
import api from "./api";

/** Listează toate tipurile de acces */
export async function getAllAccessTypes() {
  const { data } = await api.get("/accesstype/all");
  return data;
}
