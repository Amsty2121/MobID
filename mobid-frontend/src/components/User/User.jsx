// src/components/User/User.jsx
import React, { useEffect, useState } from "react";
import { getUsersPaged, addUser, deleteUser } from "../../api/userApi";
import GenericTable from "../GenericTable/GenericTable";
import AddUserModal from "./AddUserModal";
import EditUserRolesModal from "./EditUserRolesModal";
import DeleteUserModal from "./DeleteUserModal";
import "./User.css";

const User = () => {
  const [users, setUsers] = useState([]);
  const [totalCount, setTotalCount] = useState(0);
  const [currentPage, setCurrentPage] = useState(0); // PageIndex
  const [limit, setLimit] = useState(5);             // PageSize
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  // State pentru modal de adăugare
  const [showAddModal, setShowAddModal] = useState(false);
  const [newUserEmail, setNewUserEmail] = useState("");
  const [newUserUsername, setNewUserUsername] = useState("");
  const [newUserPassword, setNewUserPassword] = useState("");

  // State pentru modal de ștergere
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [userToDelete, setUserToDelete] = useState(null);

  // State pentru modal de editare roluri
  const [showEditRolesModal, setShowEditRolesModal] = useState(false);
  const [userToEditRoles, setUserToEditRoles] = useState(null);

  const fetchUsers = async () => {
    setLoading(true);
    setError("");
    try {
      const data = await getUsersPaged({ pageIndex: currentPage, pageSize: limit });
      setUsers(data.items || []);
      setTotalCount(data.total || 0);
    } catch (err) {
      console.error(err);
      setError("Eroare la preluarea utilizatorilor.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUsers();
  }, [currentPage, limit]);

  // Funcția pentru adăugare utilizator
  const handleAdd = () => {
    setShowAddModal(true);
  };

  // Funcția pentru ștergere utilizator
  const handleDelete = (row) => {
    setUserToDelete(row);
    setShowDeleteModal(true);
  };

  const confirmDelete = async () => {
    try {
      await deleteUser(userToDelete.id);
      setShowDeleteModal(false);
      setUserToDelete(null);
      fetchUsers();
    } catch (err) {
      console.error(err);
      setError("Eroare la ștergerea utilizatorului.");
    }
  };

  const cancelDelete = () => {
    setShowDeleteModal(false);
    setUserToDelete(null);
  };

  // Funcția pentru editarea rolurilor unui utilizator
  const handleEditRoles = (row) => {
    setUserToEditRoles(row);
    setShowEditRolesModal(true);
  };

  // Definirea coloanelor (adăugăm o coloană suplimentară pentru editarea rolurilor)
  const columns = [
    { header: "ID", accessor: "id" },
    { header: "Email", accessor: "email" },
    { header: "Username", accessor: "username" },
    { header: "Roluri", accessor: "roles" }, // Afișăm rolurile ca string
  ];

  // Pentru filtrare, includem și coloana "roles"
  const filterColumns = ["email", "username", "roles"];

  // Pentru a afișa rolurile ca string, mapăm lista de roluri la un șir
  const processedData = users.map(u => ({
    ...u,
    roles: u.roles && Array.isArray(u.roles) ? u.roles.join(", ") : "",
  }));

  return (
    <>
      {loading && <p>Se încarcă...</p>}
      {error && <p className="error">{error}</p>}
      <GenericTable
        title="Utilizatori"
        columns={columns}
        filterColumns={filterColumns}
        data={processedData}
        onAdd={handleAdd}
        showAddOption={true}
        onDelete={handleDelete}
        showDeleteOption={true}
        onEdit={handleEditRoles}
        showEditOption={true}
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
        <AddUserModal
          newUserEmail={newUserEmail}
          setNewUserEmail={setNewUserEmail}
          newUserUsername={newUserUsername}
          setNewUserUsername={setNewUserUsername}
          newUserPassword={newUserPassword}
          setNewUserPassword={setNewUserPassword}
          handleAddUser={async (e) => {
            e.preventDefault();
            try {
              const userAddReq = {
                email: newUserEmail,
                username: newUserUsername,
                password: newUserPassword,
              };
              await addUser(userAddReq);
              setShowAddModal(false);
              setNewUserEmail("");
              setNewUserUsername("");
              setNewUserPassword("");
              setCurrentPage(0);
              fetchUsers();
            } catch (err) {
              console.error(err);
              setError("Eroare la adăugarea utilizatorului.");
            }
          }}
          onClose={() => setShowAddModal(false)}
        />
      )}

      {showDeleteModal && userToDelete && (
        <DeleteUserModal
          user={userToDelete}
          onConfirm={confirmDelete}
          onCancel={cancelDelete}
        />
      )}

      {showEditRolesModal && userToEditRoles && (
        <EditUserRolesModal
          user={userToEditRoles}
          onClose={() => {
            setShowEditRolesModal(false);
            setUserToEditRoles(null);
            fetchUsers();
          }}
        />
      )}
    </>
  );
};

export default User;
