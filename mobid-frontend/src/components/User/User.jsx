// src/components/User/User.jsx
import React, { useEffect, useState } from "react";
import { getUsersPaged, deleteUser } from "../../api/userApi";
import GenericTable from "../GenericTable/GenericTable";
import AddUserModal from "./AddUserModal";
import EditUserRolesModal from "./EditUserRolesModal";
import DeleteUserModal from "./DeleteUserModal";
import "./User.css";

const User = () => {
  const [users, setUsers] = useState([]);
  const [totalCount, setTotalCount] = useState(0);
  const [currentPage, setCurrentPage] = useState(0);
  const [limit, setLimit] = useState(5);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const [showAddModal, setShowAddModal] = useState(false);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [userToDelete, setUserToDelete] = useState(null);

  const [showEditRolesModal, setShowEditRolesModal] = useState(false);
  const [userToEditRoles, setUserToEditRoles] = useState(null);

  const fetchUsers = async () => {
    setLoading(true);
    setError("");
    try {
      const data = await getUsersPaged({ pageIndex: currentPage, pageSize: limit });
      setUsers(data.items || []);
      setTotalCount(data.total || 0);
    } catch {
      setError("Eroare la preluarea utilizatorilor.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchUsers(); }, [currentPage, limit]);

  const handleDelete = (row) => {
    setUserToDelete(row);
    setShowDeleteModal(true);
  };
  const confirmDelete = async () => {
    try {
      await deleteUser(userToDelete.id);
      setShowDeleteModal(false);
      fetchUsers();
    } catch {
      setError("Eroare la ștergerea utilizatorului.");
    }
  };

  const handleEditRoles = (row) => {
    setUserToEditRoles(row);
    setShowEditRolesModal(true);
  };

  const columns = [
    { header: "ID", accessor: "id" },
    { header: "Email", accessor: "email" },
    { header: "Username", accessor: "username" },
    { header: "Roluri", accessor: "roles" },
  ];
  const filterCols = ["email", "username", "roles"];
  const data = users.map(u => ({
    ...u,
    roles: Array.isArray(u.roles) ? u.roles.join(", ") : "",
  }));

  return (
    <>
      {loading && <p>Se încarcă...</p>}
      {error   && <p className="error">{error}</p>}

      <GenericTable
        title="Utilizatori"
        columns={columns}
        filterColumns={filterCols}
        data={data}
        onAdd={() => setShowAddModal(true)}
        showAddOption
        onDelete={handleDelete}
        showDeleteOption
        onEdit={handleEditRoles}
        showEditOption
        currentPage={currentPage}
        totalCount={totalCount}
        pageSize={limit}
        onPageChange={setCurrentPage}
        onPageSizeChange={size => { setLimit(size); setCurrentPage(0); }}
      />

      {showAddModal && (
        <AddUserModal
          onSuccess={fetchUsers}
          onClose={() => setShowAddModal(false)}
        />
      )}

      {showDeleteModal && userToDelete && (
        <DeleteUserModal
          user={userToDelete}
          onConfirm={confirmDelete}
          onCancel={() => setShowDeleteModal(false)}
        />
      )}

      {showEditRolesModal && userToEditRoles && (
        <EditUserRolesModal
          user={userToEditRoles}
          onClose={() => {
            setShowEditRolesModal(false);
            fetchUsers();
          }}
        />
      )}
    </>
  );
};

export default User;
