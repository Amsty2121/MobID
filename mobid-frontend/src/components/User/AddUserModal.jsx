// src/components/User/AddUserModal.jsx
import React from "react";
import { FaTimes } from "react-icons/fa";
import "./User.css"; // Poți avea un fișier de stiluri separat pentru User


const AddUserModal = ({
  newUserEmail,
  setNewUserEmail,
  newUserUsername,
  setNewUserUsername,
  newUserPassword,
  setNewUserPassword,
  handleAddUser,
  onClose,
}) => {
  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <button className="modal-close" onClick={onClose}>
          <FaTimes />
        </button>
        <h3>Adaugă Utilizator Nou</h3>
        <form onSubmit={handleAddUser} className="add-user-form">
          <label htmlFor="userEmail">Email</label>
          <input
            id="userEmail"
            type="email"
            value={newUserEmail}
            onChange={(e) => setNewUserEmail(e.target.value)}
            required
          />
          <label htmlFor="userUsername">Username</label>
          <input
            id="userUsername"
            type="text"
            value={newUserUsername}
            onChange={(e) => setNewUserUsername(e.target.value)}
            required
          />
          <label htmlFor="userPassword">Parolă</label>
          <input
            id="userPassword"
            type="password"
            value={newUserPassword}
            onChange={(e) => setNewUserPassword(e.target.value)}
            required
          />
          <div className="form-actions">
            <button type="submit">Salvează</button>
            <button type="button" onClick={onClose}>
              Anulează
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddUserModal;
