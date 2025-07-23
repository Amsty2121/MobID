// src/components/Organization/TableOrganization/OrganizationTable.jsx
import React, { useState, useEffect, useCallback, useRef } from "react";
import GenericTable from "../../GenericTable/GenericTable";
import AddOrganizationModal from "./AddOrganizationModal";
import EditOrganizationModal from "./EditOrganizationModal";
import DeleteOrganizationModal from "./DeleteOrganizationModal";
import {
  getOrganizationsPaged,
  deactivateOrganization,
  updateOrganization
} from "../../../api/organizationApi";
import "../../../styles/components/generic-table.css";

export default function OrganizationTable({ onSelect }) {
  const DEFAULT_PAGE_SIZE = 10;

  const [items,    setItems]    = useState([]);
  const [total,    setTotal]    = useState(0);
  const [page,     setPage]     = useState(0);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);
  const [loading,  setLoading]  = useState(false);
  const [error,    setError]    = useState("");

  const [showAdd,  setShowAdd]  = useState(false);
  const [showEdit, setShowEdit] = useState(false);
  const [editOrg,  setEditOrg]  = useState(null);
  const [showDel,  setShowDel]  = useState(false);
  const [delOrg,   setDelOrg]   = useState(null);

  // referință pentru a ști dacă fetch-ul inițial a avut loc
  const didFetchRef = useRef(false);

  const fetchOrgs = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      const data = await getOrganizationsPaged({ pageIndex: page, pageSize });
      setItems(data.items || []);
      setTotal(data.total || 0);
    } catch {
      setError("Eroare la încărcarea organizațiilor.");
    } finally {
      setLoading(false);
    }
  }, [page, pageSize]);

  // 1) Fetch inițial la montare
  useEffect(() => {
    if (didFetchRef.current) return;
    fetchOrgs();
    didFetchRef.current = true;
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  // 2) Refetch când se schimbă page sau pageSize, după primul fetch
  useEffect(() => {
    if (didFetchRef.current) return;
    fetchOrgs();
  }, [page, pageSize, fetchOrgs]);

  const handleDelete  = org => { setDelOrg(org);   setShowDel(true); };
  const confirmDelete = async () => {
    await deactivateOrganization(delOrg.id);
    setShowDel(false);
    fetchOrgs();
  };

  const handleEdit  = org => { setEditOrg(org);   setShowEdit(true); };
  const confirmEdit = async upd => {
    await updateOrganization(upd);
    setShowEdit(false);
    fetchOrgs();
  };

  const columns = [
    { header: "ID",               accessor: "id"        },
    { header: "Organization Name", accessor: "name"      },
    { header: "Owner Name",       accessor: "ownerUsername" }
  ];
  const filterCols = ["name", "ownerUsername"];

  return (
    <>
      {loading && <p>Se încarcă...</p>}
      {error   && <p style={{ color: "#ff5555", margin: "0.5rem 0" }}>{error}</p>}

      <GenericTable
        title="Organizations"
        columns={columns}
        filterColumns={filterCols}
        data={items}
        onAdd={() => setShowAdd(true)}
        showAddOption
        onEdit={handleEdit}
        showEditOption
        onDelete={handleDelete}
        showDeleteOption
        onRowClick={onSelect}
        currentPage={page}
        totalCount={total}
        pageSize={pageSize}
        onPageChange={setPage}
        onPageSizeChange={size => { setPageSize(size); setPage(0); }}
      />

      {showAdd && (
        <AddOrganizationModal
          onSuccess={() => { setShowAdd(false); fetchOrgs(); }}
          onClose={() => setShowAdd(false)}
        />
      )}
      {showEdit && editOrg && (
        <EditOrganizationModal
          organization={editOrg}
          onSubmit={confirmEdit}
          onClose={() => setShowEdit(false)}
        />
      )}
      {showDel && delOrg && (
        <DeleteOrganizationModal
          organization={delOrg}
          onConfirm={confirmDelete}
          onCancel={() => setShowDel(false)}
        />
      )}
    </>
  );
}
