// src/components/Organization/TableOrganizationAccesses/GenerateQrModal.jsx
import React, { useState } from "react";
import TextField from "@mui/material/TextField";
import { createQrCode } from "../../../api/qrCodeApi";
import "../../../styles/components/modal/index.css";
import "../../../styles/components/access.css";

export default function GenerateQrModal({ accessId, onSuccess, onClose }) {
  const [description, setDescription] = useState("");
  const [error, setError]             = useState("");

  const handleGenerate = async () => {
    setError("");
    try {
      await createQrCode({ accessId, description: description || null });
      onSuccess();
      onClose();
    } catch (err) {
      setError("Nu am putut genera codul QR: " + err.message);
    }
  };

  return (
    <div className="modal__overlay" onClick={onClose}>
      <div className="modal__content" onClick={e => e.stopPropagation()}>
        <button className="modal__close" onClick={onClose}>×</button>
        <h3 className="modal__title">Generează Cod QR</h3>

        {error && <p className="modal__error">{error}</p>}

        <form className="modal__form" onSubmit={e => { e.preventDefault(); handleGenerate(); }}>
          <TextField
            label="Descriere (opțional)"
            variant="outlined"
            fullWidth
            margin="normal"
            value={description}
            onChange={e => setDescription(e.target.value)}
          />

          <div className="modal__actions">
            <button
              type="submit"
              className="modal__button--yes"
            >
              Generează
            </button>
            <button
              type="button"
              className="modal__button--no"
              onClick={onClose}
            >
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
