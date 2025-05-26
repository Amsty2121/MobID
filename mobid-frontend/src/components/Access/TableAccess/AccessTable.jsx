// src/components/Access/TableAccess/AccessTable.jsx
import React, { useEffect, useState } from "react";
import GenericTable from "../../GenericTable/GenericTable";
import AddAccessModal from "./AddAccessModal";
import AccessDetailsModal from "./AccessDetailsModal";
import DeleteAccessModal from "./DeleteAccessModal";
import {
  getAccessesForOrganization,
  deactivateAccess,
} from "../../../api/accessApi";

export default function AccessTable({ organizationId, organizationName }) {
  const [accesses, setAccesses] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const [showAdd, setShowAdd] = useState(false);
  const [showEdit, setShowEdit] = useState(false);
  const [showDetails, setShowDetails] = useState(false);
  const [showDelete, setShowDelete] = useState(false);
  const [selected, setSelected] = useState(null);

  const fetch = async () => {
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
    if (organizationId) fetch();
  }, [organizationId]);

  const columns = [
    { header: "ID", accessor: "id" },
    { header: "Nume", accessor: "name" },
    { header: "Tip", accessor: "accessType" },
    { header: "Max Total", accessor: "totalUseLimit" },
    { header: "Per perioadă", accessor: "useLimitPerPeriod" },
    { header: "Durată", accessor: "subscriptionPeriod" },
    { header: "Expiră", accessor: "expirationDateTime" },
    { header: "Activ", accessor: "isActive" },
  ];

  const rows = accesses.map(a => ({
    ...a,
    actions: (
      <button
        className="icon-btn"
        onClick={e => {
          e.stopPropagation();
          setSelected(a);
          setShowDelete(true);
        }}
      >
        🗑
      </button>
    )
  }));

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
          data={rows}
          onAdd={() => setShowAdd(true)}
          showAddOption
          showEditOption
          onEdit={row => { setSelected(row); setShowEdit(true); }}
          showDeleteOption={false}
          onRowClick={row => { setSelected(row); setShowDetails(true); }}
        />
      )}

      {/* Add / Edit / Details / Delete Modals */}
      {showAdd && (
        <AddAccessModal
          organizationId={organizationId}
          onSuccess={() => { setShowAdd(false); fetch(); }}
          onClose={() => setShowAdd(false)}
        />
      )}
      {showEdit && selected && (
        <EditAccessModal
          access={selected}
          onSuccess={() => { setShowEdit(false); fetch(); }}
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
        onConfirm={async () => {
          await deactivateAccess(selected.id);
          setShowDelete(false);
          fetch();
        }}
        onCancel={() => setShowDelete(false)}
      />
    </>
  );
}
