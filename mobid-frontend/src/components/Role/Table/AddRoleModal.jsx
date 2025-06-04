// src/components/Role/Table/AddRoleModal.jsx
import React, { useState } from "react";
import { FaTimes } from "react-icons/fa";
import TextField from "@mui/material/TextField";
import Button from "@mui/material/Button";
import { createRole } from "../../../api/roleApi";
import "../../../styles/components/modal/index.css";

export default function AddRoleModal({ onSuccess, onClose }) {
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    try {
      await createRole(name, description);
      onSuccess();
      onClose();
    } catch {
      setError("Nu am putut adăuga rolul.");
    }
  };

  return (
    <div className="modal__overlay">
      <div className="modal__content">
        <button className="modal__close" onClick={onClose}>
          <FaTimes />
        </button>

        <h3 className="modal__title">Adaugă Rol Nou</h3>

        {error && <div className="modal__error">{error}</div>}

        <form onSubmit={handleSubmit} className="modal__form">
          <TextField
            label="Nume Rol *"
            variant="outlined"
            value={name}
            onChange={(e) => setName(e.target.value)}
            required
            fullWidth
          />

          <TextField
            label="Descriere"
            variant="outlined"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            fullWidth
          />
          <div className="modal__actions">
            <button type="submit" className="modal__button--yes">
              Salvează
            </button>
            <button
              type="button"
              className="modal__button--no"
              onClick={onClose}>
              Anulează
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
