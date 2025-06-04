import api from "./api";

/** Generează un cod QR nou. */
export async function createQrCode(req) {
  const { data } = await api.post("/qrcode", req);
  return data;
}

/** Obține un cod QR după ID. */
export async function getQrCodeById(qrCodeId) {
  const { data } = await api.get(`/qrcode/${qrCodeId}`);
  return data;
}

/** Listează QR-urile asociate unui access. */
export async function getQrCodesForAccess(accessId) {
  const { data } = await api.get(`/qrcode/access/${accessId}`);
  return data;
}

/** Listează QR-urile paginat. */
export async function getQrCodesPaged({ pageIndex, pageSize }) {
  const { data } = await api.get("/qrcode/paged", {
    params: { pageIndex, pageSize }
  });
  return data;
}

/** Validează un cod QR. */
export async function validateQrCode(qrCodeId) {
  const { data } = await api.post(`/qrcode/${qrCodeId}/validate`);
  return data; // { isValid: boolean }
}

/** Dezactivează (soft-delete) un cod QR. */
export async function deactivateQrCode(qrCodeId) {
  await api.delete(`/qrcode/${qrCodeId}`);
}
