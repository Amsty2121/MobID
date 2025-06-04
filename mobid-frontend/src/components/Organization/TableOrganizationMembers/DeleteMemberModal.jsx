// src/components/Organization/DeleteMemberModal.jsx
import React, { useState } from "react";
import { FaTimes } from "react-icons/fa";
import { removeUserFromOrganization } from "../../../api/organizationApi";
import "../../../styles/components/modal/index.css";

export default function DeleteMemberModal({
  organizationId,
  member,
  onSuccess,
  onCancel
}) {
  const [loading, setLoading] = useState(false);
  const [error, setError]     = useState("");

  const handleConfirm = async () => {
    setLoading(true);
    setError("");
    try {
      await removeUserFromOrganization(organizationId, member.userId);
      onSuccess();
    } catch {
      setError("Nu am putut elimina membrul.");
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
        <h3 className="modal__title">Confirmă Eliminarea</h3>
        {error && <p className="modal__error">{error}</p>}
        <p className="modal__message">
          Ești sigur că vrei să elimini membrul{" "}
          <strong>
            {member.userName} | {member.userId}
          </strong>
          ?
        </p>
        <div className="modal__actions">
          <button
            type="button"
            className="modal__button--yes"
            onClick={handleConfirm}
            disabled={loading}
          >
            {loading ? "Elimin..." : "Elimină"}
          </button>
          <button
            type="button"
            className="modal__button--no"
            onClick={onCancel}
            disabled={loading}
          >
            Anulează
          </button>
        </div>
      </div>
    </div>
  );
}

