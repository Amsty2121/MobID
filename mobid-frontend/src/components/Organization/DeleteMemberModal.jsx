// src/components/Organization/DeleteMemberModal.jsx
import React from "react";
import { FaTimes } from "react-icons/fa";
import "./Organization.css";

const DeleteMemberModal = ({ member, onConfirm, onCancel }) => (
  <div className="modal-overlay">
    <div className="modal-content">
      <button className="modal-close" onClick={onCancel}><FaTimes/></button>
      <h3>Confirmă Eliminarea</h3>
      <p>
        Ești sigur că vrei să elimini membrul{" "}
        <strong>{member.userName} | {member.userId}</strong>?
      </p>
      <div className="form-actions">
        <button onClick={onConfirm}>Elimină</button>
        <button onClick={onCancel}>Anulează</button>
      </div>
    </div>
  </div>
);

export default DeleteMemberModal;
