// src/api/qrCodeApi.js
import api from "./api";

/**
 * Generează un cod QR nou
 * @param {string} accessId
 * @param {string} [description]
 * @returns {Promise<QrCodeDto>}
 */
export async function createQrCode(accessId, description) {
  const res = await api.post("/api/qrcode", { accessId, description });
  return res.data;
}

/**
 * Listează QR‐urile asociate unui access
 * @param {string} accessId
 * @returns {Promise<QrCodeDto[]>}
 */
export async function getQrCodesForAccess(accessId) {
  const res = await api.get(`/api/qrcode/access/${accessId}`);
  return res.data;
}

/**
 * Deactivează (soft‐delete) un QR
 * @param {string} qrCodeId
 */
export async function deactivateQrCode(qrCodeId) {
  await api.delete(`/api/qrcode/${qrCodeId}`);
}

/**
 * Validează un QR
 * @param {string} qrCodeId
 * @returns {Promise<{ isValid: boolean }>}
 */
export async function validateQrCode(qrCodeId) {
  const res = await api.post(`/api/qrcode/${qrCodeId}/validate`);
  return res.data;
}

/**
 * Obține un QR după ID
 * @param {string} qrCodeId
 * @returns {Promise<QrCodeDto>}
 */
export async function getQrCodeById(qrCodeId) {
  const res = await api.get(`/api/qrcode/${qrCodeId}`);
  return res.data;
}
