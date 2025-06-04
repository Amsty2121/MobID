import React, { useState } from "react";
import { FaTimes } from "react-icons/fa";
import { deactivateUser } from "../../../api/userApi";
import "../../../styles/components/modal/index.css";

export default function DeleteUserModal({ user, onSuccess, onCancel }) {
  const [loading, setLoading] = useState(false);
  const [error, setError]     = useState("");

  const handleConfirm = async () => {
    setLoading(true);
    setError("");
    try {
      await deactivateUser(user.id);
      onSuccess();    // anunță UserTable să reîncarce lista
    } catch {
      setError("Nu am putut șterge utilizatorul.");
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
        <h3 className="modal__title">Confirmă Ștergerea</h3>
        {error && <p className="modal__error">{error}</p>}
        <p className="modal__message">
          Ești sigur că vrei să ștergi utilizatorul <strong>{user.username}</strong>?
        </p>
        <div className="modal__actions">
          <button
            type="button"
            className="modal__button--yes"
            onClick={handleConfirm}
            disabled={loading}
          >
            {loading ? "Șterg..." : "Șterge"}
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
