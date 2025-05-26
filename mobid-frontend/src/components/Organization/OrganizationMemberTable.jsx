// src/components/Organization/OrganizationMemberTable.jsx
import React, { useState, useEffect } from "react";
import GenericTable from "../GenericTable/GenericTable";
import AddMemberModal from "./AddMemberModal";
import DeleteMemberModal from "./DeleteMemberModal";
import {
  getUsersForOrganization,
  removeUserFromOrganization
} from "../../api/organizationApi";
import { FaTrash } from "react-icons/fa";
import "./Organization.css";

export default function OrganizationMemberTable({ organizationId, organizationName }) {
  const DEFAULT_PAGE_SIZE = 10;

  const [members,    setMembers]    = useState([]);
  const [loading,    setLoading]    = useState(false);
  const [error,      setError]      = useState("");
  const [showAdd,    setShowAdd]    = useState(false);
  const [showDel,    setShowDel]    = useState(false);
  const [delMember,  setDelMember]  = useState(null);

  // paginare client-side
  const [page,       setPage]       = useState(0);
  const [pageSize,   setPageSize]   = useState(DEFAULT_PAGE_SIZE);

  const fetchMembers = async () => {
    setLoading(true);
    setError("");
    try {
      const data = await getUsersForOrganization(organizationId);
      setMembers(data);
      setPage(0);
    } catch {
      setError("Eroare la încărcarea membrilor.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (organizationId) fetchMembers();
  }, [organizationId]);

  const handleRemove = m => {
    setDelMember(m);
    setShowDel(true);
  };
  const confirmRemove = async () => {
    await removeUserFromOrganization(organizationId, delMember.userId);
    setShowDel(false);
    fetchMembers();
  };

  // pregătim coloanele și datele cu butoane
  const columns = [
    { header: "User ID",   accessor: "userId"   },
    { header: "User Name", accessor: "userName" },
    { header: "Rol",       accessor: "role"     },
    { header: "Acțiuni",   accessor: "actions"  }
  ];
  const dataWithActions = members.map(m => ({
    ...m,
    actions: (
      <button className="icon-btn" onClick={() => handleRemove(m)} title="Exclude">
        <FaTrash />
      </button>
    )
  }));

  // segmentul de afișat pe pagina curentă
  const start = page * pageSize;
  const pageData = dataWithActions.slice(start, start + pageSize);

  return (
    <div className="org-page">
      {error && <p className="error">{error}</p>}

      {loading ? (
        <p>Se încarcă membrii...</p>
      ) : (
        <GenericTable
          title={`Membri din „${organizationName}”`}
          columns={columns}
          filterColumns={["userName","userId","role"]}
          data={pageData}
          onAdd={() => setShowAdd(true)}
          showAddOption
          showEditOption={false}
          showDeleteOption={false}
          currentPage={page}
          totalCount={dataWithActions.length}
          pageSize={pageSize}
          onPageChange={setPage}
          onPageSizeChange={size => { setPageSize(size); setPage(0); }}
        />
      )}

      {showAdd && (
        <AddMemberModal
          organizationId={organizationId}
          onSuccess={() => { setShowAdd(false); fetchMembers(); }}
          onClose={() => setShowAdd(false)}
        />
      )}
      {showDel && delMember && (
        <DeleteMemberModal
          member={delMember}
          onConfirm={confirmRemove}
          onCancel={() => setShowDel(false)}
        />
      )}
    </div>
  );
}
