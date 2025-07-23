// src/components/User/TableUserOrganization/UserOrganizationTable.jsx
import React, { useEffect, useState, useRef } from "react";
import GenericTable from "../../GenericTable/GenericTable";
import { getUserOrganizations } from "../../../api/userApi";
import "../../../styles/components/user.css";

export default function UserOrganizationTable({ userId, userName }) {
  const [orgs, setOrgs]           = useState([]);
  const [loading, setLoading]     = useState(false);
  const [error, setError]         = useState("");
  const [page, setPage]           = useState(0);
  const [pageSize, setPageSize]   = useState(10);
  const [total, setTotal]         = useState(0);
  const didFetchRef               = useRef(false);

  const fetchOrgs = async () => {
    setLoading(true);
    setError("");
    try {
      const data = await getUserOrganizations(userId);
      setOrgs(data);
      // display all on one page
      setPage(0);
      setPageSize(data.length);
      setTotal(data.length);
    } catch {
      setError("Eroare la încărcarea organizațiilor.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (!userId || didFetchRef.current) return;
    didFetchRef.current = true;
    fetchOrgs();
  }, [userId]);

  const columns = [
    { header: "ID",          accessor: "id" },
    { header: "Nume",        accessor: "name" },
    { header: "Description",   accessor: "description" },
    {
      header: "Join Date",
      accessor: "createdAt",
      format: v => (v ? new Date(v).toLocaleDateString() : "")
    },
    {
      header: "Activ",
      accessor: "isActive",
      format: v => (v ? "✅" : "❌")
    }
  ];

  return (
    <>
      {error && <p className="error">{error}</p>}

      {loading ? (
        <p className="loading">Se încarcă organizațiile...</p>
      ) : (
        <GenericTable
          title={`Organizații pentru User «${userName}»`}
          columns={columns}
          filterColumns={["name", "description"]}
          data={orgs}
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
