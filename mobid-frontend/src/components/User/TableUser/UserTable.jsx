// src/components/User/Table/UserTable.jsx
import React, { useState, useEffect, useCallback } from "react";
import { getUsersPaged, deactivateUser } from "../../../api/userApi";
import GenericTable from "../../GenericTable/GenericTable";
import AddUserModal from "./AddUserModal";
import EditUserRolesModal from "./EditUserRolesModal";
import DeleteUserModal from "./DeleteUserModal";
import "../../../styles/components/user.css";

export default function UserTable({ onSelect }) {
  const DEFAULT_PAGE_SIZE = 10;
  const [users, setUsers]             = useState([]);
  const [totalCount, setTotalCount]   = useState(0);
  const [currentPage, setCurrentPage] = useState(0);
  const [pageSize, setPageSize]       = useState(DEFAULT_PAGE_SIZE);
  const [loading, setLoading]         = useState(false);
  const [error, setError]             = useState("");

  const [showAddModal, setShowAddModal]             = useState(false);
  const [showDeleteModal, setShowDeleteModal]       = useState(false);
  const [userToDelete, setUserToDelete]             = useState(null);
  const [showEditRolesModal, setShowEditRolesModal] = useState(false);
  const [userToEditRoles, setUserToEditRoles]       = useState(null);

  // 1) fetchUsers e memoizat pe currentPage și pageSize
  const fetchUsers = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      const { items = [], total = 0 } = await getUsersPaged({ pageIndex: currentPage, pageSize });
      setUsers(items);
      setTotalCount(total);
    } catch {
      setError("Eroare la preluarea utilizatorilor.");
    } finally {
      setLoading(false);
    }
  }, [currentPage, pageSize]);

  // 2) apelăm fetchUsers la montare și ori de câte ori se schimbă pagina / dimensiunea paginii
  useEffect(() => {
    fetchUsers();
  }, [fetchUsers]);

  const handleDeleteClick = row => {
    setUserToDelete(row);
    setShowDeleteModal(true);
  };
  const confirmDelete = async () => {
    try {
      await deactivateUser(userToDelete.id);
      setShowDeleteModal(false);
      fetchUsers();
    } catch {
      setError("Eroare la ștergerea utilizatorului.");
    }
  };

  const handleEditRoles = row => {
    setUserToEditRoles(row);
    setShowEditRolesModal(true);
  };

  // transform roles array -> string
  const data = users.map(u => ({
    ...u,
    roles: Array.isArray(u.roles) ? u.roles.join(", ") : ""
  }));

  const columns = [
    { header: "ID",       accessor: "id"       },
    { header: "Email",    accessor: "email"    },
    { header: "Username", accessor: "username" },
    { header: "Roluri",   accessor: "roles"    },
  ];
  const filterCols = ["email", "username", "roles"];

  return (
    <>
      {loading && <p className="loading">Se încarcă...</p>}
      {error   && <p className="error">{error}</p>}

      <GenericTable
        title="Users"
        columns={columns}
        filterColumns={filterCols}
        data={data}
        onAdd={() => setShowAddModal(true)}
        showAddOption
        onDelete={handleDeleteClick}
        showDeleteOption
        onEdit={handleEditRoles}
        showEditOption
        onRowClick={row => onSelect && onSelect(row)}

        // ** PAGINATION props **
        currentPage={currentPage}
        totalCount={totalCount}
        pageSize={pageSize}
        onPageChange={page => setCurrentPage(page)}
        onPageSizeChange={size => {
          setPageSize(size);
          setCurrentPage(0);
        }}
      />

      {showAddModal && (
        <AddUserModal
          onSuccess={() => {
            setShowAddModal(false);
            fetchUsers();
          }}
          onClose={() => setShowAddModal(false)}
        />
      )}

      {showDeleteModal && userToDelete && (
        <DeleteUserModal
          user={userToDelete}
          onSuccess={() => {
            setShowDeleteModal(false);
            fetchUsers();
          }}
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
}
