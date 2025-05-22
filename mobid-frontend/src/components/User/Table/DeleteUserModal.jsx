// src/components/User/Table/DeleteUserModal.jsx
import React from "react";
import { FaTimes } from "react-icons/fa";
import "../User.css";

const DeleteUserModal = ({ user, onConfirm, onCancel }) => {
  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <button className="modal-close" onClick={onCancel}>
          <FaTimes />
        </button>
        <h3>Confirmă Ștergerea</h3>
        <p>
          Ești sigur că vrei să ștergi utilizatorul{" "}
          <strong>{user && user.username}</strong>?
        </p>
        <div className="form-actions">
          <button type="button" onClick={onConfirm}>
            Șterge
          </button>
          <button type="button" onClick={onCancel}>
            Anulează
          </button>
        </div>
      </div>
    </div>
  );
};

export default DeleteUserModal;
