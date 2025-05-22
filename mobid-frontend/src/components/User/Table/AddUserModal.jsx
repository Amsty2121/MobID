// src/components/User/Table/AddUserModal.jsx
import React, { useState } from "react";
import { FaTimes } from "react-icons/fa";
import { createUser } from "../../../api/userApi";
import "../User.css";

const AddUserModal = ({ onSuccess, onClose }) => {
  const [email, setEmail] = useState("");
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    try {
      await createUser({ email, username, password });
      onSuccess();   // reîmprospătează lista
      onClose();
    } catch {
      setError("Nu am putut adăuga utilizatorul.");
    }
  };

  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <button className="modal-close" onClick={onClose}>
          <FaTimes />
        </button>
        <h3>Adaugă Utilizator Nou</h3>
        {error && <p className="error">{error}</p>}
        <form onSubmit={handleSubmit} className="add-user-form">
          <label htmlFor="userEmail">Email</label>
          <input
            id="userEmail"
            type="email"
            value={email}
            onChange={e => setEmail(e.target.value)}
            required
          />

          <label htmlFor="userUsername">Username</label>
          <input
            id="userUsername"
            type="text"
            value={username}
            onChange={e => setUsername(e.target.value)}
            required
          />

          <label htmlFor="userPassword">Parolă</label>
          <input
            id="userPassword"
            type="password"
            value={password}
            onChange={e => setPassword(e.target.value)}
            required
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

export default AddUserModal;
