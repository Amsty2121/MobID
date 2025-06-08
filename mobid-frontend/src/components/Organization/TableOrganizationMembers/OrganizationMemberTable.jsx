// src/components/Organization/OrganizationMemberTable.jsx
import React, { useState, useEffect, useRef, useCallback } from "react";
import GenericTable from "../../GenericTable/GenericTable";
import AddMemberModal from "./AddMemberModal";
import DeleteMemberModal from "./DeleteMemberModal";
import { getUsersForOrganization } from "../../../api/organizationApi";
import "../../../styles/components/modal/index.css";

export default function OrganizationMemberTable({
  organizationId,
  organizationName
}) {
  const DEFAULT_PAGE_SIZE = 10;

  const [members, setMembers]       = useState([]);
  const [loading, setLoading]       = useState(false);
  const [error, setError]           = useState("");
  const [showAdd, setShowAdd]       = useState(false);
  const [showDel, setShowDel]       = useState(false);
  const [delMember, setDelMember]   = useState(null);

  const [page, setPage]             = useState(0);
  const [pageSize, setPageSize]     = useState(DEFAULT_PAGE_SIZE);

  // Prevent double‐fetch
  const didFetchRef = useRef(false);

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

  useEffect(() => {
    if (!organizationId || didFetchRef.current) return;
    didFetchRef.current = true;
    fetchMembers();
  }, [organizationId, fetchMembers]);

  const handleRemoveClick = member => {
    setDelMember(member);
    setShowDel(true);
  };

  const columns = [
    { header: "User ID",   accessor: "userId"   },
    { header: "User Name", accessor: "userName" },
    { header: "Rol",       accessor: "role"     },
  ];

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
          data={members}
          onAdd={() => setShowAdd(true)}
          showAddOption
          showEditOption={false}
          showDeleteOption
          onDelete={handleRemoveClick}
          currentPage={page}
          totalCount={members.length}
          pageSize={pageSize}
          onPageChange={setPage}
          onPageSizeChange={size => { setPageSize(size); setPage(0); }}
        />
      )}

      {showAdd && (
        <AddMemberModal
          organizationId={organizationId}
          onSuccess={() => {
            setShowAdd(false);
            // allow re-fetch
            didFetchRef.current = false;
            fetchMembers();
          }}
          onClose={() => setShowAdd(false)}
        />
      )}

      {showDel && delMember && (
        <DeleteMemberModal
          organizationId={organizationId}
          member={delMember}
          onSuccess={() => {
            setShowDel(false);
            // refresh members
            didFetchRef.current = false;
            fetchMembers();
          }}
          onCancel={() => setShowDel(false)}
        />
      )}
    </div>
  );
}
