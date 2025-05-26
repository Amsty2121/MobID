// src/components/Access/TableAccess/DeleteAccessModal.jsx
import React from "react";
import { FaTimes } from "react-icons/fa";
import "../../Access/Access.css";

export default function DeleteAccessModal({ open, accessName, onConfirm, onCancel }) {
  if (!open) return null;
  return (
    <div className="modal-overlay" onClick={onCancel}>
      <div className="modal-content" onClick={e => e.stopPropagation()}>
        <button className="modal-close" onClick={onCancel}>
          <FaTimes />
        </button>
        <h3>Confirmă dezactivarea</h3>
        <p>
          Ești sigur că vrei să dezactivezi accesul{" "}
          <strong>{accessName}</strong>?
        </p>
        <div className="form-actions">
          <button type="button" onClick={onConfirm}>
            Dezactivează
          </button>
          <button type="button" onClick={onCancel}>
            Anulează
          </button>
        </div>
      </div>
    </div>
  );
}
