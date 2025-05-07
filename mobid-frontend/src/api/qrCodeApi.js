import api from "./api";

/**
 * Generează un QR code pentru un acces.
 * @param {string} accessId
 */
export async function generateQrCode(accessId) {
    // acum trimitem `{ accessId }` în body
    const { data } = await api.post("/QrCode/generate", { accessId });
    return data;
  }
  
  export async function getQrCodeById(qrCodeId) {
    const { data } = await api.get(`/QrCode/${qrCodeId}`);
    return data;
  }
  
  export async function getQrCodesForAccess(accessId) {
    const { data } = await api.get(`/QrCode/access/${accessId}`);
    return data;
  }
  
  export async function validateQrCode(qrCodeId, scanningUserId) {
    const { data } = await api.post("/QrCode/validate", null, {
      params: { qrCodeId, scanningUserId }
    });
    return data.isValid;
  }
  
  export async function getQrCodesPaged({ pageIndex, pageSize }) {
    const { data } = await api.get("/QrCode/paged", { params: { pageIndex, pageSize } });
    return data;
  }
  
  export async function deactivateQrCode(qrCodeId) {
    return api.delete(`/QrCode/${qrCodeId}`);
  }