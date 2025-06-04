// src/components/Access/TableAccess/AccessTable.jsx
import React, { useEffect, useState, useRef } from "react";
import "../../../styles/components/access.css";
import GenericTable from "../../GenericTable/GenericTable";
import AddAccessModal from "./AddAccessModal";
import AccessDetailsModal from "./AccessDetailsModal";
import DeleteAccessModal from "./DeleteAccessModal";
import {
  getAccessesForOrganization,
  deactivateAccess
} from "../../../api/accessApi";

export default function AccessTable({ organizationId, organizationName }) {
  const [accesses, setAccesses]       = useState([]);
  const [loading, setLoading]         = useState(false);
  const [error, setError]             = useState("");
  const [showAdd, setShowAdd]         = useState(false);
  const [showEdit, setShowEdit]       = useState(false);
  const [showDetails, setShowDetails] = useState(false);
  const [showDelete, setShowDelete]   = useState(false);
  const [selected, setSelected]       = useState(null);

  // Prevent double-fetch under StrictMode
  const didFetchRef = useRef(false);

  const fetchAccesses = async () => {
    setLoading(true);
    setError("");
    try {
      const data = await getAccessesForOrganization(organizationId);
      setAccesses(data);
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
    // force a fresh fetch next time
    didFetchRef.current = false;
    fetchAccesses();
  };

  const columns = [
    { header: "ID",           accessor: "id" },
    { header: "Nume",         accessor: "name" },
    { header: "Tip",          accessor: "accessType" },
    { header: "Max Total",    accessor: "totalUseLimit" },
    { header: "Per perioadă", accessor: "useLimitPerPeriod" },
    { header: "Durată",       accessor: "subscriptionPeriod" },
    { header: "Expiră",       accessor: "expirationDateTime" },
    { header: "Activ",        accessor: "isActive" },
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
          filterColumns={["name", "accessType"]}
          data={accesses}
          onAdd={() => setShowAdd(true)}
          showAddOption
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
            // allow re-fetch in effect
            didFetchRef.current = false;
            fetchAccesses();
          }}
          onClose={() => setShowAdd(false)}
        />
      )}

      {showEdit && selected && (
        <AddAccessModal
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

      <DeleteAccessModal
        open={showDelete}
        accessName={selected?.name}
        onConfirm={confirmDelete}
        onCancel={() => setShowDelete(false)}
      />
    </>
  );
}
