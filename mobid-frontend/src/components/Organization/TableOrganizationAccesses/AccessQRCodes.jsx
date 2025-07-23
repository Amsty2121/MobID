// src/components/Organization/TableOrganizationAccesses/AccessQRCodes.jsx
import React, { useEffect, useState } from "react";
import GenericTable from "../../GenericTable/GenericTable";
import GenerateQrModal from "./GenerateQrModal";
import QrPreviewModal  from "./QrPreviewModal";
import DeleteQrCodeModal from "./DeleteQrCodeModal";
import { getQrCodesForAccess, deactivateQrCode } from "../../../api/qrCodeApi";
import { FaTrash } from "react-icons/fa";
import "../../../styles/components/modal/index.css";

export default function AccessQRCodes({ access }) {
  const [qrCodes, setQrCodes]               = useState([]);
  const [loading, setLoading]               = useState(false);
  const [error, setError]                   = useState("");
  const [showGenQr, setShowGenQr]           = useState(false);
  const [previewQr, setPreviewQr]           = useState(null);
  const [qrToDelete, setQrToDelete]         = useState(null);
  const [showDeleteModal, setShowDeleteModal] = useState(false);

  // pagination local
  const [page, setPage]         = useState(0);
  const [pageSize, setPageSize] = useState(5);

  // funcție de reload
  const fetchQrs = async () => {
    setLoading(true);
    setError("");
    try {
      const data = await getQrCodesForAccess(access.id);
      setQrCodes(data);
      setPage(0);
    } catch {
      setError("Eroare la încărcarea codurilor QR.");
    } finally {
      setLoading(false);
    }
  };

  // Re-fetch la fiecare schimbare de `access`
  useEffect(() => {
    if (!access) return;
    fetchQrs();
  }, [access]);

  // descarcă SVG-ul curent din previewQr
  const handleDownload = () => {
    const svg = document.getElementById("qr-code-svg");
    const blob = new Blob(
      [new XMLSerializer().serializeToString(svg)],
      { type: "image/svg+xml;charset=utf-8" }
    );
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = `${previewQr.id}.svg`;
    link.click();
    URL.revokeObjectURL(url);
  };

  // gestionare delete
  const handleDeleteClick = row => {
    setQrToDelete(row);
    setShowDeleteModal(true);
  };
  const confirmDelete = async () => {
    await deactivateQrCode(qrToDelete.id);
    setShowDeleteModal(false);
    await fetchQrs();
  };

  // definim coloanele cu formatter pentru Expiră
  const columns = [
    { header: "ID",        accessor: "id" },
    { header: "Description", accessor: "description" },
    { header: "Type",       accessor: "type" },
    {
      header: "ExpiresAt",
      accessor: "expiresAt",
      format: v => v ? new Date(v).toLocaleDateString() : "No"
    },
    {
      header: "CreatedAt",
      accessor: "createdAt",
      format: v => new Date(v).toLocaleDateString()
    }
  ];

  // datele filtrate pentru pagina curentă
  const totalCount = qrCodes.length;
  const start      = page * pageSize;
  const pageData   = qrCodes.slice(start, start + pageSize);

  // adăugăm coloana Acțiuni
  const dataWithActions = pageData.map(q => ({
    ...q,
    actions: (
      <button
        className="generic-table__icon-btn"
        onClick={e => { e.stopPropagation(); handleDeleteClick(q); }}
        title="Dezactivează"
      >
        <FaTrash />
      </button>
    )
  }));

  return (
    <div className="qr-codes-section">
      <h2 className="access-heading">QR Codes for «{access.name}»</h2>
      {error && <p className="error">{error}</p>}

      {loading ? (
        <p>Se încarcă codurile QR...</p>
      ) : (
        <GenericTable
          columns={[...columns, { header: "Actions", accessor: "actions" }]}
          filterColumns={["description", "id"]}
          data={dataWithActions}
          showAddOption
          onAdd={() => setShowGenQr(true)}
          showEditOption={false}
          showDeleteOption={false}
          onRowClick={row => setPreviewQr(row)}

          // pagination props
          currentPage={page}
          totalCount={totalCount}
          pageSize={pageSize}
          onPageChange={setPage}
          onPageSizeChange={size => { setPageSize(size); setPage(0); }}
        />
      )}

      {/* Modal generare QR */}
      {showGenQr && (
        <GenerateQrModal
          accessId={access.id}
          onSuccess={async () => {
            setShowGenQr(false);
            await fetchQrs();      // ← refresh imediat după succes
          }}
          onClose={() => setShowGenQr(false)}
        />
      )}

      {/* Modal preview + download */}
      {previewQr && (
        <QrPreviewModal
          qr={previewQr}
          onClose={() => setPreviewQr(null)}
          onDownload={handleDownload}
        />
      )}

      {/* Modal confirm dezactivare */}
      {showDeleteModal && qrToDelete && (
        <DeleteQrCodeModal
          open={showDeleteModal}
          description={qrToDelete.description}
          onConfirm={confirmDelete}
          onCancel={() => setShowDeleteModal(false)}
        />
      )}
    </div>
  );
}
