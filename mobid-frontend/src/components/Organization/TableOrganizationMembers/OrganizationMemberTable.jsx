// src/components/Organization/OrganizationMemberTable.jsx
import React, { useState, useEffect, useRef, useCallback } from "react";
import GenericTable from "../../GenericTable/GenericTable";
import AddMemberModal from "./AddMemberModal";
import DeleteMemberModal from "./DeleteMemberModal";
import {
  getUsersForOrganization,
} from "../../../api/organizationApi";
import { FaTrash } from "react-icons/fa";
import "../../../styles/components/modal/index.css";

export default function OrganizationMemberTable({
  organizationId,
  organizationName
}) {
  const DEFAULT_PAGE_SIZE = 10;

  const [members, setMembers]     = useState([]);
  const [loading, setLoading]     = useState(false);
  const [error, setError]         = useState("");
  const [showAdd, setShowAdd]     = useState(false);
  const [showDel, setShowDel]     = useState(false);
  const [delMember, setDelMember] = useState(null);

  const [page, setPage]           = useState(0);
  const [pageSize, setPageSize]   = useState(DEFAULT_PAGE_SIZE);

  // guard ca să nu refetch-ăm de două ori
  const didFetchRef = useRef(false);

  // memoize fetchMembers pentru a îl putea folosi ca dependență în useEffect
  const fetchMembers = useCallback(async () => {
    if (!organizationId) return;
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
  }, [organizationId]);

  // apelăm o singură dată fetchMembers per organizationId
  useEffect(() => {
    if (!organizationId || didFetchRef.current) return;
    didFetchRef.current = true;
    fetchMembers();
  }, [organizationId, fetchMembers]);

  const handleRemoveClick = m => {
    setDelMember(m);
    setShowDel(true);
  };

  const columns = [
    { header: "User ID",   accessor: "userId"   },
    { header: "User Name", accessor: "userName" },
    { header: "Rol",       accessor: "role"     },
    { header: "Acțiuni",   accessor: "actions"  }
  ];

  const dataWithActions = members.map(m => ({
    ...m,
    actions: (
      <button
        className="icon-btn"
        onClick={() => handleRemoveClick(m)}
        title="Exclude"
      >
        <FaTrash />
      </button>
    )
  }));

  // datele paginate
  const start    = page * pageSize;
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
          filterColumns={["userName", "userId", "role"]}
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
          onSuccess={() => { setShowDel(false); fetchMembers(); }}
          onCancel={() => setShowDel(false)}
        />
      )}
    </div>
  );
}
