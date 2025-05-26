// src/components/Role/Table/DeleteRoleModal.jsx
import React from "react";
import { FaTimes } from "react-icons/fa";
import "../../../styles/components/modal/index.css";

const DeleteRoleModal = ({ role, onConfirm, onCancel }) => {
  return (
    <div className="modal__overlay">
      <div className="modal__content">
        <button className="modal__close" onClick={onCancel}>
          <FaTimes />
        </button>
        <h3 className="modal__title">Confirmă Ștergerea</h3>
        <p className="modal__message">
          Ești sigur că vrei să ștergi rolul{" "}
          <strong>{role?.name}</strong>?
        </p>
        <div className="modal__actions">
          <button
            type="button"
            className="modal__button--yes"
            onClick={onConfirm}
          >
            Șterge
          </button>
          <button
            type="button"
            className="modal__button--no"
            onClick={onCancel}
          >
            Anulează
          </button>
        </div>
      </div>
    </div>
  );
};

export default DeleteRoleModal;
