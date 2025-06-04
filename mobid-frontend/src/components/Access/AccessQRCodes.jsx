// src/components/Access/AccessQRCodes.jsx
import React, { useEffect, useState, useRef } from "react";
import GenericTable from "../GenericTable/GenericTable";
import GenerateQrModal from "./GenerateQrModal";
import DeleteQrCodeModal from "./DeleteQrCodeModal";
import { getQrCodesForAccess, deactivateQrCode } from "../../api/qrCodeApi";
import { QRCodeSVG } from "qrcode.react";
import "./Access.css";

export default function AccessQRCodes({ access }) {
  const [qrCodes, setQrCodes]         = useState([]);
  const [loading, setLoading]         = useState(false);
  const [error, setError]             = useState("");
  const [showGenQr, setShowGenQr]     = useState(false);
  const [previewQr, setPreviewQr]     = useState(null);
  const [qrToDelete, setQrToDelete]   = useState(null);

  const fetchQrs = async () => {
    setLoading(true);
    setError("");
    try {
      const data = await getQrCodesForAccess(access.id);
      setQrCodes(data);
    } catch {
      setError("Eroare la încărcarea codurilor QR.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (access) fetchQrs();
  }, [access]);

  const handleRowClick = row => setPreviewQr(row);

  const handleDownload = () => {
    const svg = document.getElementById("qr-code-svg");
    const serializer = new XMLSerializer();
    const svgBlob = new Blob(
      [serializer.serializeToString(svg)],
      { type: "image/svg+xml;charset=utf-8" }
    );
    const url = URL.createObjectURL(svgBlob);
    const link = document.createElement("a");
    link.href = url;
    link.download = `${previewQr.id}.svg`;
    link.click();
    URL.revokeObjectURL(url);
  };

  const confirmDeactivate = async () => {
    await deactivateQrCode(qrToDelete.id);
    setQrToDelete(null);
    fetchQrs();
  };

  const columns = [
    { header: "ID",        accessor: "id" },
    { header: "Descriere", accessor: "description" },
    { header: "Tip",       accessor: "type" },
    { header: "Expiră",    accessor: "expiresAt" },
    { header: "Creat",     accessor: "createdAt" },
    { header: "Acțiuni",   accessor: "actions" }
  ];

  const dataWithActions = qrCodes.map(q => ({
    ...q,
    actions: (
      <button
        className="icon-btn"
        onClick={e => {
          e.stopPropagation();
          setQrToDelete(q);
        }}
        title="Dezactivează"
      >
        🗑
      </button>
    )
  }));

  return (
    <div className="qr-codes-section">
      <h2 className="access-heading">
        QR Codes pentru «{access.name}»
      </h2>

      {error && <p className="error">{error}</p>}

      {loading ? (
        <p>Se încarcă codurile QR...</p>
      ) : (
        <GenericTable
          columns={columns}
          filterColumns={["description", "id"]}
          data={dataWithActions}
          onAdd={() => setShowGenQr(true)}
          showAddOption
          showEditOption={false}
          showDeleteOption={false}
          onRowClick={handleRowClick}
        />
      )}

      {/* Modal generare QR */}
      {showGenQr && (
        <GenerateQrModal
          accessId={access.id}
          onSuccess={() => { setShowGenQr(false); fetchQrs(); }}
          onClose={() => setShowGenQr(false)}
        />
      )}

      {/* Modal de preview */}
      {previewQr && (
        <div
          className="qr-modal-overlay"
          onClick={() => setPreviewQr(null)}
        >
          <div
            className="qr-modal-content"
            onClick={e => e.stopPropagation()}
          >
            <h3 className="qr-modal-title">
              {previewQr.name}
            </h3>
            <div className="qr-design-frame">
              <div className="qr-code-wrap">
                <QRCodeSVG
                  id="qr-code-svg"
                  value={previewQr.qrEncodedText}
                  size={200}
                  includeMargin={false}
                  bgColor="#fff"
                  fgColor="var(--color-tuna-light)"
                />
              </div>
              <div className="qr-footer">
                <span className="qr-footer-text">
                  Scan me to get access
                </span>
              </div>
            </div>
            <button className="download-btn" onClick={handleDownload}>
              Descarcă QR ca imagine
            </button>
          </div>
        </div>
      )}

      {/* Modal confirm dezactivare QR */}
      <DeleteQrCodeModal
        open={!!qrToDelete}
        description={qrToDelete?.description}
        onConfirm={confirmDeactivate}
        onCancel={() => setQrToDelete(null)}
      />
    </div>
  );
}
