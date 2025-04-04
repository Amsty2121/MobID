// src/components/Role/Role.jsx
import React, { useEffect, useState } from "react";
import { getRolesPaged, addRole, deleteRole } from "../../api/roleApi";
import GenericTable from "../GenericTable/GenericTable";
import AddRoleModal from "./AddRoleModal";
import DeleteRoleModal from "./DeleteRoleModal";
import "./Role.css";

const Role = () => {
  const [roles, setRoles] = useState([]);
  const [totalCount, setTotalCount] = useState(0);
  const [currentPage, setCurrentPage] = useState(0); // PageIndex
  const [limit, setLimit] = useState(5);             // PageSize
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  // State pentru adăugare
  const [showAddModal, setShowAddModal] = useState(false);
  const [newRoleName, setNewRoleName] = useState("");
  const [newRoleDescription, setNewRoleDescription] = useState("");

  // State pentru ștergere
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [roleToDelete, setRoleToDelete] = useState(null);

  const fetchRoles = async () => {
    setLoading(true);
    setError("");
    try {
      const data = await getRolesPaged({ pageIndex: currentPage, pageSize: limit });
      setRoles(data.items || []);
      setTotalCount(data.total || 0);
    } catch (err) {
      console.error(err);
      setError("Eroare la preluarea rolurilor.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchRoles();
  }, [currentPage, limit]);

  const handleAdd = async () => {
    setShowAddModal(true);
  };

  const handleDelete = async (row) => {
    setRoleToDelete(row);
    setShowDeleteModal(true);
  };

  const confirmDelete = async () => {
    try {
      await deleteRole(roleToDelete.id);
      setShowDeleteModal(false);
      setRoleToDelete(null);
      fetchRoles();
    } catch (err) {
      console.error(err);
    }
  };

  const cancelDelete = () => {
    setShowDeleteModal(false);
    setRoleToDelete(null);
  };

  const columns = [
    { header: "ID", accessor: "id" },
    { header: "Nume", accessor: "name" },
    { header: "Descriere", accessor: "description" },
  ];

  const filterColumns = ["name", "description"];

  return (
    <>
      {loading && <p>Se încarcă...</p>}
      {error && <p className="error">{error}</p>}
      <GenericTable
        title="Roluri"
        columns={columns}
        filterColumns={filterColumns}
        data={roles}
        onAdd={handleAdd}
        showAddOption={true}
        onDelete={handleDelete}
        showDeleteOption={true}
        // Nu transmitem onEdit și showEditOption pentru roluri momentan
        currentPage={currentPage}
        totalCount={totalCount}
        pageSize={limit}
        onPageChange={(newPage) => setCurrentPage(newPage)}
        onPageSizeChange={(newSize) => {
          setLimit(newSize);
          setCurrentPage(0);
        }}
      />

      {showAddModal && (
        <AddRoleModal
          newRoleName={newRoleName}
          setNewRoleName={setNewRoleName}
          newRoleDescription={newRoleDescription}
          setNewRoleDescription={setNewRoleDescription}
          handleAddRole={async (e) => {
            e.preventDefault();
            try {
              await addRole(newRoleName, newRoleDescription);
              setShowAddModal(false);
              setNewRoleName("");
              setNewRoleDescription("");
              setCurrentPage(0);
              fetchRoles();
            } catch (err) {
              console.error(err);
            }
          }}
          onClose={() => setShowAddModal(false)}
        />
      )}

      {showDeleteModal && roleToDelete && (
        <DeleteRoleModal
          role={roleToDelete}
          onConfirm={confirmDelete}
          onCancel={cancelDelete}
        />
      )}
    </>
  );
};

export default Role;
