// src/components/Role/AddRoleModal.jsx
import React from "react";
import { FaTimes } from "react-icons/fa";
import "./Role.css";

const AddRoleModal = ({
  newRoleName,
  setNewRoleName,
  newRoleDescription,
  setNewRoleDescription,
  handleAddRole,
  onClose,
}) => {
  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <button className="modal-close" onClick={onClose}>
          <FaTimes />
        </button>
        <h3>Adaugă Rol Nou</h3>
        <form onSubmit={handleAddRole} className="add-role-form">
          <label htmlFor="roleName">Nume Rol</label>
          <input
            id="roleName"
            type="text"
            value={newRoleName}
            onChange={(e) => setNewRoleName(e.target.value)}
            required
          />
          <label htmlFor="roleDesc">Descriere</label>
          <input
            id="roleDesc"
            type="text"
            value={newRoleDescription}
            onChange={(e) => setNewRoleDescription(e.target.value)}
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

export default AddRoleModal;
