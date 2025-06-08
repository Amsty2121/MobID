// src/api/qrCodeApi.js
import api from "./api";

/** Generează un cod QR */
export async function createQrCode(payload) {
  const { data } = await api.post("/qrcode", payload);
  return data;
}

/** Obține detaliu cod QR */
export async function getQrCodeById(qrCodeId) {
  const { data } = await api.get(`/qrcode/${qrCodeId}`);
  return data;
}

/** Listează codurile QR ale unui access */
export async function getQrCodesForAccess(accessId) {
  const { data } = await api.get(`/qrcode/forAccess/${accessId}`);
  return data;
}

/** Listează codurile QR paginat */
export async function getQrCodesPaged(params) {
  const { data } = await api.get("/qrcode/paged", { params });
  return data;
}

/** Dezactivează un cod QR */
export async function deactivateQrCode(qrCodeId) {
  const response = await api.delete(`/qrcode/${qrCodeId}`);
  return response.status === 204;
}

/** Validează un cod QR */
export async function validateQrCode(qrCodeId) {
  const { data } = await api.post(`/qrcode/${qrCodeId}/validate`);
  return data;
}
