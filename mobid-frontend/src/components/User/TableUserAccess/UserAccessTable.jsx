// src/components/User/TableUserAccess/UserAccessTable.jsx

import React, { useEffect, useState, useRef } from "react";
import GenericTable from "../../GenericTable/GenericTable";
import { getAllUserAccesses } from "../../../api/userApi";
import "../../../styles/components/access.css";

export default function UserAccessTable({ userId, userName }) {
  const [accesses, setAccesses] = useState([]);
  const [loading, setLoading]   = useState(false);
  const [error, setError]       = useState("");

  // disable pagination (show all at once)
  const [page, setPage]         = useState(0);
  const [pageSize, setPageSize] = useState(0);
  const [total, setTotal]       = useState(0);

  const didFetch = useRef(false);

  const fetchAccesses = async () => {
    setLoading(true);
    setError("");
    try {
      const data = await getAllUserAccesses(userId);
      setAccesses(data);
      setPage(0);
      setPageSize(data.length);
      setTotal(data.length);
    } catch {
      setError("Eroare la încărcarea acceselor.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (!userId) return;
    if (didFetch.current) return;
    didFetch.current = true;
    fetchAccesses();
  }, [userId]);

  const columns = [
    { header: "Name",           accessor: "name" },
    { header: "Type",            accessor: "accessTypeName" },
    {
      header: "Organization",
      accessor: "organizationName"
    },
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
      header: "Member Restricted",
      accessor: "restrictToOrgMembers",
      format: v => (v ? "Yes" : "No")
    },
    {
      header: "Shareadble",
      accessor: "restrictToOrgSharing",
      format: v => (v ? "Yes" : "No")
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
          title={`Accesses for User - «${userName}»`}
          columns={columns}
          filterColumns={["name", "accessTypeName", "organizationName"]}
          data={accesses}
          currentPage={page}
          totalCount={total}
          pageSize={pageSize}
          onPageChange={() => {}}
          onPageSizeChange={() => {}}
          showAddOption={false}
          showEditOption={false}
          showDeleteOption={false}
        />
      )}
    </>
  );
}
