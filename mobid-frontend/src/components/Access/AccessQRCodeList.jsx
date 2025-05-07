// src/components/Access/AccessQRCodeList.jsx
import React, { useEffect, useState } from "react";
import GenericTable from "../GenericTable/GenericTable";
import GenerateQrModal from "./GenerateQrModal";
import { getQrCodesForAccess, deactivateQrCode } from "../../api/qrCodeApi";
import { FaTrash } from "react-icons/fa";
import "./Access.css";

export default function AccessQRCodeList({ access }) {
  const [qrCodes, setQrCodes] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [showGenQr, setShowGenQr] = useState(false);

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
    if (access) {
      fetchQrs();
    }
  }, [access]);

  const handleDeactivate = async (qr) => {
    await deactivateQrCode(qr.id);
    fetchQrs();
  };

  const columns = [
    { header: "ID", accessor: "id" },
    { header: "Activ", accessor: "isActive" },
    { header: "Acțiuni", accessor: "actions" },
  ];

  const dataWithActions = qrCodes.map((q) => ({
    ...q,
    actions: (
      <button
        className="icon-btn"
        onClick={() => handleDeactivate(q)}
        title="Dezactivează"
      >
        <FaTrash />
      </button>
    ),
  }));

  return (
    <>
      {error && <p className="error">{error}</p>}
      {loading && <p>Se încarcă codurile QR...</p>}

      <GenericTable
        title="Coduri QR"
        columns={columns}
        filterColumns={[]}
        data={dataWithActions}
        onAdd={() => setShowGenQr(true)}
        showAddOption
        showEditOption={false}
        showDeleteOption={false}
        currentPage={0}
        totalCount={dataWithActions.length}
        pageSize={dataWithActions.length}
        onPageChange={() => {}}
        onPageSizeChange={() => {}}
      />

      {showGenQr && (
        <GenerateQrModal
          accessId={access.id}
          onSuccess={() => {
            setShowGenQr(false);
            fetchQrs();
          }}
          onClose={() => setShowGenQr(false)}
        />
      )}
    </>
  );
}
