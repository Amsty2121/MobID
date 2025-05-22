// src/components/Organization/OrganizationMembers.jsx
import React, { useEffect, useState } from "react";
import GenericTable from "../GenericTable/GenericTable";
import AddMemberModal from "./AddMemberModal";
import DeleteMemberModal from "./DeleteMemberModal";
import {
  getUsersForOrganization,
  removeUserFromOrganization
} from "../../api/organizationApi";
import { FaTrash } from "react-icons/fa";
import "./Organization.css";

export default function OrganizationMembers({ organizationId, organizationName }) {
  const [members, setMembers]       = useState([]);
  const [loading, setLoading]       = useState(false);
  const [error, setError]           = useState("");
  const [showAdd, setShowAdd]       = useState(false);
  const [showDelete, setShowDelete] = useState(false);
  const [toDelete, setToDelete]     = useState(null);

  const fetchMembers = async () => {
    setLoading(true);
    setError("");
    try {
      const data = await getUsersForOrganization(organizationId);
      setMembers(data);
    } catch {
      setError("Eroare la încărcarea membrilor.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (organizationId) fetchMembers();
  }, [organizationId]);

  const handleRemove = member => {
    setToDelete(member);
    setShowDelete(true);
  };

  const confirmRemove = async () => {
    try {
      await removeUserFromOrganization(organizationId, toDelete.userId);
      setShowDelete(false);
      fetchMembers();
    } catch {
      setError("Nu am putut elimina membrul.");
      setShowDelete(false);
    }
  };

  const memberCols = [
    { header: "User ID",   accessor: "userId"   },
    { header: "User Name", accessor: "userName" },
    { header: "Rol",       accessor: "role"     },
    { header: "Acțiuni",   accessor: "actions"  }
  ];

  const membersWithActions = members.map(m => ({
    ...m,
    actions: (
      <button
        className="icon-btn"
        onClick={() => handleRemove(m)}
        title="Exclude"
      >
        <FaTrash />
      </button>
    )
  }));

  return (
    <div className="org-page">
      <h2 className="org-heading">Membri din „{organizationName}”</h2>
      {error && <p className="error">{error}</p>}
      {loading
        ? <p>Se încarcă membrii...</p>
        : (
          <GenericTable
            title=""
            columns={memberCols}
            filterColumns={["userName", "userId", "role"]}
            data={membersWithActions}
            onAdd={() => setShowAdd(true)}
            showAddOption
            showEditOption={false}
            showDeleteOption={false}
            currentPage={0}
            totalCount={membersWithActions.length}
            pageSize={membersWithActions.length}
            onPageChange={() => {}}
            onPageSizeChange={() => {}}
          />
        )
      }

      {showAdd && (
        <AddMemberModal
          organizationId={organizationId}
          onSuccess={() => { setShowAdd(false); fetchMembers(); }}
          onClose={() => setShowAdd(false)}
        />
      )}
      {showDelete && toDelete && (
        <DeleteMemberModal
          member={toDelete}
          onConfirm={confirmRemove}
          onCancel={() => setShowDelete(false)}
        />
      )}
    </div>
  );
}