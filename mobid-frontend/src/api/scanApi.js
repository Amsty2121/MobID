// src/api/scanApi.js
import api from "./api";

/** Obține toate scanările cu detalii complete (QR → Access → Org → useri) */
export async function getAllScansWithDetails() {
  const { data } = await api.get("/scan/full");
  return data;
}
