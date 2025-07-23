// src/components/Role/Table/DeleteRoleModal.jsx
import React, { useState } from "react";
import { FaTimes } from "react-icons/fa";
import { deactivateRole } from "../../../api/roleApi";
import "../../../styles/components/modal/index.css";

export default function DeleteRoleModal({ 
  role, 
  onSuccess,    // callback apelat după ștergere cu succes
  onCancel 
}) {
  const [loading, setLoading] = useState(false);
  const [error, setError]     = useState("");

  const handleConfirm = async () => {
    setLoading(true);
    setError("");
    try {
      await deactivateRole(role.id);
      onSuccess();    // anunță RoleTable să reîncarce lista
    } catch {
      setError("Nu am putut şterge rolul.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="modal__overlay">
      <div className="modal__content">
        <button className="modal__close" onClick={onCancel}>
          <FaTimes />
        </button>
        <h3 className="modal__title">Confirm Delete</h3>
        {error && <p className="modal__error">{error}</p>}
        <p className="modal__message">
          Are you sure you want to delete the role <strong>{role.name}</strong>?
        </p>
        <div className="modal__actions">
          <button
            type="button"
            className="modal__button--yes"
            onClick={handleConfirm}
            disabled={loading}
          >
            {loading ? "Şterg..." : "Şterge"}
          </button>
          <button
            type="button"
            className="modal__button--no"
            onClick={onCancel}
            disabled={loading}
          >
            Cancel
          </button>
        </div>
      </div>
    </div>
  );
}
