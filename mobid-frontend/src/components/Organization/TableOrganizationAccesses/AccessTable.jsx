// src/components/Organization/TableOrganizationAccesses/AccessTable.jsx
import React, { useEffect, useState, useRef } from "react";
import "../../../styles/components/access.css";
import GenericTable from "../../GenericTable/GenericTable";

import AddAccessModal     from "./AddAccessModal";
import EditAccessModal    from "./EditAccessModal";
import AccessDetailsModal from "./AccessDetailsModal";
import DeleteAccessModal  from "./DeleteAccessModal";
import AccessQRCodes      from "./AccessQRCodes";

import { getAllOrganizationAccesses } from "../../../api/organizationApi";
import { deactivateAccess }           from "../../../api/accessApi";

import { FaEdit, FaTrash } from "react-icons/fa";

export default function AccessTable({ organizationId, organizationName }) {
  const [accesses, setAccesses]         = useState([]);
  const [loading, setLoading]           = useState(false);
  const [error, setError]               = useState("");
  const [showAdd, setShowAdd]           = useState(false);
  const [showEdit, setShowEdit]         = useState(false);
  const [showDelete, setShowDelete]     = useState(false);

  const [selected, setSelected]         = useState(null);
  const [selectedForQr, setSelectedForQr] = useState(null);

  // o singură “pagină”
  const [pageSize, setPageSize]         = useState(0);
  const didFetchRef = useRef(false);

  const fetchAccesses = async () => {
    setLoading(true);
    setError("");
    try {
      const data = await getAllOrganizationAccesses(organizationId);
      setAccesses(data);
      setPageSize(data.length);
    } catch {
      setError("Eroare la încărcarea accese.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (!organizationId || didFetchRef.current) return;
    didFetchRef.current = true;
    fetchAccesses();
  }, [organizationId]);

  const handleDelete = row => {
    setSelected(row);
    setShowDelete(true);
  };
  const confirmDelete = async () => {
    await deactivateAccess(selected.id);
    setShowDelete(false);
    didFetchRef.current = false;
    fetchAccesses();
  };

  const handleEdit = row => {
    setSelected(row);
    setShowEdit(true);
  };

  // coloanele + acțiuni
  const columns = [
    { header: "Name",        accessor: "name" },
    { header: "Type",         accessor: "accessTypeName" },
    { header: "Organization", accessor: "organizationName" },
    {
      header: "Multiscan",
      accessor: "isMultiScan",
      format: v => (v ? "Yes" : "No")
    },
    {
      header: "Expires",
      accessor: "expirationDateTime",
      format: v => (v ? new Date(v).toLocaleDateString() : "Unlimited")
    },
    {
      header: "Members Restricted",
      accessor: "restrictToOrgMembers",
      format: v => (v ? "Yes" : "No")
    },
    {
      header: "Shareable",
      accessor: "restrictToOrgSharing",
      format: v => (v ? "Yes" : "No")
    },
    {
      header: "IsActive",
      accessor: "isActive",
      format: v => (v ? "✅" : "❌")
    },
    {
      header: "Actions",
      accessor: "actions"
    }
  ];

  // construim datele cu butoanele noastre FaEdit / FaTrash
  const dataWithActions = accesses.map(row => {
    const own = row.organizationId === organizationId;
    return {
      ...row,
      actions: own ? (
        <div style={{ display: "flex", gap: "0.5rem" }}>
          <button
            className="generic-table__icon-btn"
            title="Editează"
            onClick={e => { e.stopPropagation(); handleEdit(row); }}
          >
            <FaEdit />
          </button>
          <button
            className="generic-table__icon-btn"
            title="Șterge"
            onClick={e => { e.stopPropagation(); handleDelete(row); }}
          >
            <FaTrash />
          </button>
        </div>
      ) : null
    };
  });

  return (
    <>
      <h2 className="access-heading">
        Accesses for «{organizationName}»
      </h2>

      {error && <p className="error">{error}</p>}

      {loading ? (
        <p>Se încarcă accesele...</p>
      ) : (
        <GenericTable
          columns={columns}
          filterColumns={["name","accessTypeName","organizationName"]}
          data={dataWithActions}
          // dezactivează butoanele implicite
          showAddOption
          showEditOption={false}
          showDeleteOption={false}
          onAdd={() => setShowAdd(true)}
          onRowClick={row => setSelectedForQr(row)}
          currentPage={0}
          totalCount={pageSize}
          pageSize={pageSize}
          onPageChange={() => {}}
          onPageSizeChange={() => {}}
        />
      )}

      {showAdd && (
        <AddAccessModal
          organizationId={organizationId}
          onSuccess={() => { setShowAdd(false); didFetchRef.current = false; fetchAccesses(); }}
          onClose={() => setShowAdd(false)}
        />
      )}

      {showEdit && selected && (
        <EditAccessModal
          access={selected}
          onSuccess={() => { setShowEdit(false); didFetchRef.current = false; fetchAccesses(); }}
          onClose={() => setShowEdit(false)}
        />
      )}

      {showDelete && selected && (
        <DeleteAccessModal
          open
          accessName={selected.name}
          onConfirm={confirmDelete}
          onCancel={() => setShowDelete(false)}
        />
      )}

      {selectedForQr && (
        <div style={{ marginTop: "2rem" }}>
          <AccessQRCodes access={selectedForQr} />
        </div>
      )}
    </>
  );
}
