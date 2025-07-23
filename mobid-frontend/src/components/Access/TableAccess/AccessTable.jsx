// src/components/Access/TableAccess/AccessTable.jsx
import React, { useEffect, useState, useRef } from "react";
import "../../../styles/components/access.css";
import GenericTable from "../../GenericTable/GenericTable";
import AddAccessModal from "./AddAccessModal";
import EditAccessModal from "./EditAccessModal";      // ← import pentru edit
import DeleteAccessModal from "./DeleteAccessModal";
import {
  getAllAccesses,
  deactivateAccess
} from "../../../api/accessApi";

export default function AccessTable({
  showAddOption    = false,
  showEditOption   = false,
  showDeleteOption = false,
  onRowClick       = () => {},
}) {
  const [accesses, setAccesses] = useState([]);
  const [loading,  setLoading]  = useState(false);
  const [error,    setError]    = useState("");
  const [showAdd,  setShowAdd]  = useState(false);
  const [showEdit, setShowEdit] = useState(false);
  const [showDel,  setShowDel]  = useState(false);
  const [selected, setSelected] = useState(null);

  // paginare locală
  const [page, setPage]         = useState(0);
  const [pageSize, setPageSize] = useState(10);

  const didFetchRef = useRef(false);

  const fetchAccesses = async () => {
    setLoading(true);
    setError("");
    try {
      const data = await getAllAccesses();
      setAccesses(data);
      setPage(0);
    } catch {
      setError("Eroare la încărcarea acceselor.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (didFetchRef.current) return;
    didFetchRef.current = true;
    fetchAccesses();
  }, []);

  const handleDelete = row => {
    setSelected(row);
    setShowDel(true);
  };

  const confirmDelete = async () => {
    await deactivateAccess(selected.id);
    setShowDel(false);
    didFetchRef.current = false;
    fetchAccesses();
  };

  // slice pentru paginare
  const totalCount = accesses.length;
  const start = page * pageSize;
  const pagedData = accesses.slice(start, start + pageSize);

  const columns = [
    { header: "Nume",          accessor: "name" },
    { header: "Tip",           accessor: "accessTypeName" },
    { header: "Organizație",   accessor: "organizationName" },
    {
      header: "Multiscan",
      accessor: "isMultiScan",
      format: v => (v ? "Yes" : "Nu")
    },
    {
      header: "Expieres",
      accessor: "expirationDateTime",
      format: v => v ? new Date(v).toLocaleDateString() : "Unlimited"
    },
    {
      header: "Member restricted",
      accessor: "restrictToOrgMembers",
      format: v => (v ? "Yes" : "No")
    },
    {
      header: "Shareadble",
      accessor: "restrictToOrgSharing",
      format: v => (v ? "Nu" : "Yes")
    },
    {
      header: "IsActive",
      accessor: "isActive",
      format: v => (v ? "✅" : "❌")
    }
  ];

  return (
    <>
      {error && <p className="error">{error}</p>}

      {loading ? (
        <p>Se încarcă accesele...</p>
      ) : (
        <GenericTable
          title="Toate Accesele"
          columns={columns}
          filterColumns={["name","accessTypeName"]}
          data={pagedData}
          currentPage={page}
          totalCount={totalCount}
          pageSize={pageSize}
          onPageChange={setPage}
          onPageSizeChange={size => { setPageSize(size); setPage(0); }}
          showAddOption={showAddOption}
          onAdd={() => setShowAdd(true)}
          showEditOption={showEditOption}
          onEdit={row => { setSelected(row); setShowEdit(true); }}
          showDeleteOption={showDeleteOption}
          onDelete={handleDelete}
          onRowClick={onRowClick}
        />
      )}

      {/* Modal de adăugat acces */}
      {showAdd && (
        <AddAccessModal
          onSuccess={() => {
            setShowAdd(false);
            didFetchRef.current = false;
            fetchAccesses();
          }}
          onClose={() => setShowAdd(false)}
        />
      )}

      {/* Modal de editat acces */}
      {showEdit && selected && (
        <EditAccessModal
          access={selected}
          onSuccess={() => {
            setShowEdit(false);
            didFetchRef.current = false;
            fetchAccesses();
          }}
          onClose={() => setShowEdit(false)}
        />
      )}

      {/* Modal de șters acces */}
      {showDel && selected && (
        <DeleteAccessModal
          open
          accessName={selected.name}
          onConfirm={confirmDelete}
          onCancel={() => setShowDel(false)}
        />
      )}
    </>
  );
}
