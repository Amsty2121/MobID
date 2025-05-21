// src/api/qrCodeApi.js
import api from "./api";

/**
 * Generează un QR code pentru un acces.
 * @param {string} accessId
 */
export async function generateQrCode(accessId) {
  // ATENȚIE: pe backend generator-ul așteaptă accessId ca query string, nu în body
  const { data } = await api.post(
    "/QrCode/generate",
    null,
    { params: { accessId } }
  );
  return data;
}

/**
 * Obține toate codurile QR asociate unui acces.
 * @param {string} accessId
 */
export async function getQrCodesForAccess(accessId) {
  const { data } = await api.get(`/QrCode/access/${accessId}`);
  return data;
}

/**
 * Dezactivează un cod QR.
 * @param {string} qrCodeId
 */
export async function deactivateQrCode(qrCodeId) {
  return api.delete(`/QrCode/${qrCodeId}`);
}
