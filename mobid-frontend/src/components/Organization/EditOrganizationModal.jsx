// src/components/Organization/EditOrganizationModal.jsx
import React, { useState } from "react";
import { FaTimes } from "react-icons/fa";
import "./Organization.css";

const EditOrganizationModal = ({ organization, onSubmit, onClose }) => {
  const [newName, setNewName] = useState(organization.name);
  const [newOwnerId, setNewOwnerId] = useState(organization.ownerId || "");

  const handleSubmit = (e) => {
    e.preventDefault();
    onSubmit({
      organizationId: organization.id,
      name: newName.trim() === "" ? null : newName,
      ownerId: newOwnerId.trim() === "" ? null : newOwnerId,
    });
  };

  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <button className="modal-close" onClick={onClose}>
          <FaTimes />
        </button>
        <h3>Actualizează Organizația</h3>
        <form onSubmit={handleSubmit} className="edit-org-form">
          <label htmlFor="orgName">Nume Organizație</label>
          <input
            id="orgName"
            type="text"
            value={newName}
            onChange={(e) => setNewName(e.target.value)}
          />
          <label htmlFor="orgOwner">ID Proprietar</label>
          <input
            id="orgOwner"
            type="text"
            value={newOwnerId}
            onChange={(e) => setNewOwnerId(e.target.value)}
          />
          <div className="form-actions">
            <button type="submit">Salvează</button>
            <button type="button" onClick={onClose}>
              Anulează
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default EditOrganizationModal;
