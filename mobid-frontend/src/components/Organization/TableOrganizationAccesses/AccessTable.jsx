// src/components/Organization/TableOrganizationAccesses/AccessTable.jsx
import React, { useEffect, useState, useRef } from "react";
import "../../../styles/components/access.css";
import GenericTable from "../../GenericTable/GenericTable";
import AddAccessModal from "./AddAccessModal";
import EditAccessModal from "./EditAccessModal";
import AccessDetailsModal from "./AccessDetailsModal";
import DeleteAccessModal from "./DeleteAccessModal";
// ← pull in the new combined‐accesses call:
import { getAllOrganizationAccesses } from "../../../api/organizationApi";
import { deactivateAccess } from "../../../api/accessApi";

export default function AccessTable({ organizationId, organizationName }) {
  const [accesses, setAccesses]       = useState([]);
  const [loading, setLoading]         = useState(false);
  const [error, setError]             = useState("");
  const [showAdd, setShowAdd]         = useState(false);
  const [showEdit, setShowEdit]       = useState(false);
  const [showDetails, setShowDetails] = useState(false);
  const [showDelete, setShowDelete]   = useState(false);
  const [selected, setSelected]       = useState(null);

  // disable paging: single‐page view
  const [page, setPage]       = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [total, setTotal]     = useState(0);

  const didFetchRef = useRef(false);

  const fetchAccesses = async () => {
    setLoading(true);
    setError("");
    try {
      // ← switch to the new method
      const data = await getAllOrganizationAccesses(organizationId);
      setAccesses(data);

      // show all on one page
      setPage(0);
      setPageSize(data.length);
      setTotal(data.length);
    } catch {
      setError("Eroare la încărcarea accese.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (!organizationId) return;
    if (didFetchRef.current) return;
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

  const columns = [
    { header: "Nume",        accessor: "name" },
    { header: "Tip",         accessor: "accessTypeName" },
    {
      header: "Multiscan",
      accessor: "isMultiScan",
      format: v => (v ? "Da" : "Nu")
    },
    {
      header: "Expiră",
      accessor: "expirationDateTime",
      format: v => (v ? new Date(v).toLocaleDateString() : "Nelimitat")
    },
    {
      header: "Doar membri",
      accessor: "restrictToOrgMembers",
      format: v => (v ? "Da" : "Nu")
    },
    {
      header: "Partajabil",
      accessor: "restrictToOrgSharing",
      format: v => (v ? "Da" : "Nu")
    },
    // ← show which org this access came from
    {
      header: "Organizație",
      accessor: "organizationName"
    },
    {
      header: "Activ",
      accessor: "isActive",
      format: v => (v ? "✅" : "❌")
    }
  ];

  return (
    <>
      <h2 className="access-heading">
        Accese pentru «{organizationName}»
      </h2>

      {error && <p className="error">{error}</p>}

      {loading ? (
        <p>Se încarcă accesele...</p>
      ) : (
        <GenericTable
          columns={columns}
          filterColumns={["name", "accessTypeName", "organizationName"]}
          data={accesses}
          currentPage={page}
          totalCount={total}
          pageSize={pageSize}
          onPageChange={() => {}}
          onPageSizeChange={() => {}}
          showAddOption
          onAdd={() => setShowAdd(true)}
          showEditOption
          onEdit={row => { setSelected(row); setShowEdit(true); }}
          showDeleteOption
          onDelete={handleDelete}
          onRowClick={row => { setSelected(row); setShowDetails(true); }}
        />
      )}

      {showAdd && (
        <AddAccessModal
          organizationId={organizationId}
          onSuccess={() => {
            setShowAdd(false);
            didFetchRef.current = false;
            fetchAccesses();
          }}
          onClose={() => setShowAdd(false)}
        />
      )}

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

      {showDetails && selected && (
        <AccessDetailsModal
          access={selected}
          onClose={() => setShowDetails(false)}
        />
      )}

      {showDelete && selected && (
        <DeleteAccessModal
          open={showDelete}
          accessName={selected.name}
          onConfirm={confirmDelete}
          onCancel={() => setShowDelete(false)}
        />
      )}
    </>
  );
}
