// src/components/Role/Table/DeleteRoleModal.jsx
import React from "react";
import { FaTimes } from "react-icons/fa";
import "../Role.css";

const DeleteRoleModal = ({ role, onConfirm, onCancel }) => {
  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <button className="modal-close" onClick={onCancel}>
          <FaTimes />
        </button>
        <h3>Confirmă Ștergerea</h3>
        <p>
          Ești sigur că vrei să ștergi rolul{" "}
          <strong>{role && role.name}</strong>?
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

export default DeleteRoleModal;
