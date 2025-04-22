// src/components/Role/Role.jsx
import React, { useEffect, useState } from "react";
import { getRolesPaged, deleteRole } from "../../api/roleApi";
import GenericTable from "../GenericTable/GenericTable";
import AddRoleModal from "./AddRoleModal";
import DeleteRoleModal from "./DeleteRoleModal";
import "./Role.css";

const Role = () => {
  const [roles, setRoles] = useState([]);
  const [totalCount, setTotalCount] = useState(0);
  const [currentPage, setCurrentPage] = useState(0);
  const [limit, setLimit] = useState(5);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const [showAddModal, setShowAddModal] = useState(false);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [roleToDelete, setRoleToDelete] = useState(null);

  const fetchRoles = async () => {
    setLoading(true);
    try {
      const data = await getRolesPaged({ pageIndex: currentPage, pageSize: limit });
      setRoles(data.items || []);
      setTotalCount(data.total || 0);
    } catch {
      setError("Eroare la preluarea rolurilor.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchRoles(); }, [currentPage, limit]);

  const handleDelete = (row) => {
    setRoleToDelete(row);
    setShowDeleteModal(true);
  };
  const confirmDelete = async () => {
    await deleteRole(roleToDelete.id);
    setShowDeleteModal(false);
    fetchRoles();
  };

  const columns = [
    { header: "ID", accessor: "id" },
    { header: "Nume", accessor: "name" },
    { header: "Descriere", accessor: "description" },
  ];
  const filterCols = ["name", "description"];

  return (
    <>
      {loading && <p>Se încarcă...</p>}
      {error   && <p className="error">{error}</p>}

      <GenericTable
        title="Roluri"
        columns={columns}
        filterColumns={filterCols}
        data={roles}
        onAdd={() => setShowAddModal(true)}
        showAddOption
        onDelete={handleDelete}
        showDeleteOption
        currentPage={currentPage}
        totalCount={totalCount}
        pageSize={limit}
        onPageChange={setCurrentPage}
        onPageSizeChange={size => { setLimit(size); setCurrentPage(0); }}
      />

      {showAddModal && (
        <AddRoleModal
          onSuccess={fetchRoles}
          onClose={() => setShowAddModal(false)}
        />
      )}

      {showDeleteModal && roleToDelete && (
        <DeleteRoleModal
          role={roleToDelete}
          onConfirm={confirmDelete}
          onCancel={() => setShowDeleteModal(false)}
        />
      )}
    </>
  );
};

export default Role;
