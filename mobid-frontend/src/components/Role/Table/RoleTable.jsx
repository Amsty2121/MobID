// src/components/Role/Table/RoleTable.jsx
import React, { useState, useEffect, useCallback, useRef } from "react";
import { getRolesPaged } from "../../../api/roleApi";
import GenericTable from "../../GenericTable/GenericTable";
import AddRoleModal from "./AddRoleModal";
import DeleteRoleModal from "./DeleteRoleModal";
import "../../../styles/components/role.css";

export default function RoleTable() {
  const DEFAULT_PAGE_SIZE = 10;

  const [roles, setRoles] = useState([]);
  const [totalCount, setTotalCount] = useState(0);
  const [currentPage, setCurrentPage] = useState(0);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const [showAddModal, setShowAddModal] = useState(false);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [roleToDelete, setRoleToDelete] = useState(null);

  // Prevent double-fetch under StrictMode
  const didFetchRef = useRef(false);

  const fetchRoles = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      const data = await getRolesPaged({ pageIndex: currentPage, pageSize });
      setRoles(data.items || []);
      setTotalCount(data.total || 0);
    } catch {
      setError("Eroare la preluarea rolurilor.");
    } finally {
      setLoading(false);
    }
  }, [currentPage, pageSize]);

  useEffect(() => {
    if (didFetchRef.current) return;
    didFetchRef.current = true;
    fetchRoles();
  }, [fetchRoles]);

  const handleDeleteClick = (row) => {
    setRoleToDelete(row);
    setShowDeleteModal(true);
  };

  const columns = [
    { header: "ID",        accessor: "id" },
    { header: "Nume",      accessor: "name" },
    { header: "Descriere", accessor: "description" },
  ];
  const filterCols = ["name", "description"];

  return (
    <>
      {loading && <p className="loading">Se încarcă...</p>}
      {error   && <p className="error">{error}</p>}

      <GenericTable
        title="Roluri"
        columns={columns}
        filterColumns={filterCols}
        data={roles}
        onAdd={() => setShowAddModal(true)}
        showAddOption
        onDelete={handleDeleteClick}
        showDeleteOption
        currentPage={currentPage}
        totalCount={totalCount}
        pageSize={pageSize}
        onPageChange={setCurrentPage}
        onPageSizeChange={(size) => { setPageSize(size); setCurrentPage(0); }}
      />

      {showAddModal && (
        <AddRoleModal
          onSuccess={() => { setShowAddModal(false); fetchRoles(); }}
          onClose={() => setShowAddModal(false)}
        />
      )}

      {showDeleteModal && roleToDelete && (
        <DeleteRoleModal
          role={roleToDelete}
          onSuccess={() => {
            setShowDeleteModal(false);
            fetchRoles();
          }}
          onCancel={() => setShowDeleteModal(false)}
        />
      )}
    </>
  );
}
