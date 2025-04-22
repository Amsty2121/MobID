// src/components/Role/AddRoleModal.jsx
import React, { useState } from "react";
import { FaTimes } from "react-icons/fa";
import { addRole } from "../../api/roleApi";
import "./Role.css";

const AddRoleModal = ({ onSuccess, onClose }) => {
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    try {
      await addRole(name, description);
      onSuccess();        // container‑ul poate reîmprospăta lista
      onClose();
    } catch (err) {
      console.error(err);
      setError("Nu am putut adăuga rolul.");
    }
  };

  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <button className="modal-close" onClick={onClose}><FaTimes/></button>
        <h3>Adaugă Rol Nou</h3>
        {error && <p className="error">{error}</p>}
        <form onSubmit={handleSubmit} className="add-role-form">
          <label htmlFor="roleName">Nume Rol</label>
          <input
            id="roleName"
            type="text"
            value={name}
            onChange={e => setName(e.target.value)}
            required
          />

          <label htmlFor="roleDesc">Descriere</label>
          <input
            id="roleDesc"
            type="text"
            value={description}
            onChange={e => setDescription(e.target.value)}
          />

          <div className="form-actions">
            <button type="submit">Salvează</button>
            <button type="button" onClick={onClose}>Anulează</button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddRoleModal;
