// src/components/Organization/TableOrganizationAccesses/DeleteAccessModal.jsx
import React from "react";
import { FaTimes } from "react-icons/fa";
import "../../../styles/components/modal/index.css";

export default function DeleteAccessModal({ open, accessName, onConfirm, onCancel }) {
  if (!open) return null;
  return (
    <div className="modal__overlay" onClick={onCancel}>
      <div className="modal__content" onClick={e => e.stopPropagation()}>
        <button className="modal__close" onClick={onCancel}>
          <FaTimes />
        </button>
        <h3 className="modal__title">Confirm Deactivation</h3>
        <p className="modal__message">
          Are you sure you want to deactivate access <strong>{accessName}</strong>?
        </p>
        <div className="modal__actions">
          <button className="modal__button--yes" onClick={onConfirm}>
            Deactivate
          </button>
          <button className="modal__button--no" onClick={onCancel}>
            Cancel
          </button>
        </div>
      </div>
    </div>
  );
}
