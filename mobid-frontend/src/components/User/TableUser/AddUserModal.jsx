// src/components/User/Table/AddUserModal.jsx
import React, { useState } from "react";
import { FaTimes } from "react-icons/fa";
import TextField from "@mui/material/TextField";
import { createUser } from "../../../api/userApi";
import "../../../styles/components/modal/index.css";

const AddUserModal = ({ onSuccess, onClose }) => {
  const [email, setEmail]       = useState("");
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError]       = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    try {
      await createUser({ email, username, password });
      onSuccess();
      onClose();
    } catch {
      setError("Nu am putut adăuga utilizatorul.");
    }
  };

  return (
    <div className="modal__overlay">
      <div className="modal__content">
        <button className="modal__close" onClick={onClose}>
          <FaTimes />
        </button>
        <h3 className="modal__title">Add new User</h3>
        {error && <p className="modal__error">{error}</p>}
        <form onSubmit={handleSubmit} className="modal__form">
          <TextField
            label="Email"
            type="email"
            value={email}
            onChange={e => setEmail(e.target.value)}
            required
            variant="outlined"
          />
          <TextField
            label="Username"
            value={username}
            onChange={e => setUsername(e.target.value)}
            required
            variant="outlined"
          />
          <TextField
            label="Password"
            type="password"
            value={password}
            onChange={e => setPassword(e.target.value)}
            required
            variant="outlined"
          />

          <div className="modal__actions">
            <button type="submit" className="modal__button--yes">
              Save
            </button>
            <button
              type="button"
              className="modal__button--no"
              onClick={onClose}>
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddUserModal;
