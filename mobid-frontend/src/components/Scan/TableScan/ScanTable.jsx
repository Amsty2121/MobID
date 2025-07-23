// src/components/Scan/TableScan/ScanTable.jsx
import React, { useEffect, useState, useRef } from "react";
import "../../../styles/components/scan.css";
import GenericTable from "../../GenericTable/GenericTable";
import { getAllScansWithDetails } from "../../../api/scanApi";
import { FaSearch } from "react-icons/fa";

export default function ScanTable() {
  const [scans, setScans]       = useState([]);
  const [loading, setLoading]   = useState(false);
  const [error, setError]       = useState("");
  const [pageSize, setPageSize] = useState(0);
  const didFetchRef             = useRef(false);

  const fetchScans = async () => {
    setLoading(true);
    setError("");
    try {
      const data = await getAllScansWithDetails();
      setScans(data);
      setPageSize(data.length);
    } catch {
      setError("Eroare la încărcarea scanărilor.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (didFetchRef.current) return;
    didFetchRef.current = true;
    fetchScans();
  }, []);

  const columns = [
    { header: "ID",               accessor: "id" },
    { header: "QR Code ID",       accessor: "qrCodeId" },
    { header: "Access Name",      accessor: "accessName" },
    { header: "Organization",     accessor: "organizationName" },
    { header: "Scanned For",      accessor: "scannedForUserName" },
    {
      header: "Scanned At",
      accessor: "scannedAt",
      format: v => new Date(v).toLocaleString()
    }
  ];

  return (
    <div className="scan-page">
      {error && <p className="error">{error}</p>}

      {loading ? (
        <p>Se încarcă scanările...</p>
      ) : (
        <GenericTable
            title={"All Scans"}
          columns={columns}
          filterColumns={[
            "qrCodeId",
            "accessName",
            "organizationName",
            "scannedForUserName"
          ]}
          data={scans}
          showAddOption={false}
          showEditOption={false}
          showDeleteOption={false}
          currentPage={0}
          totalCount={pageSize}
          pageSize={pageSize}
          onPageChange={() => {}}
          onPageSizeChange={() => {}}
          searchIcon={<FaSearch />}
          placeholder="Filtrează scanările după QR, Access, Org, etc…"
        />
      )}
    </div>
  );
}
