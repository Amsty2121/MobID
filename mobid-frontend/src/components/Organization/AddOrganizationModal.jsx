// src/components/Organization/AddOrganizationModal.jsx
import React from "react";
import { FaTimes } from "react-icons/fa";
import "./Organization.css";

const AddOrganizationModal = ({
  newOrgName,
  setNewOrgName,
  newOrgOwnerId,
  setNewOrgOwnerId,
  handleAddOrg,
  onClose,
}) => {
  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <button className="modal-close" onClick={onClose}>
          <FaTimes />
        </button>
        <h3>Adaugă Organizație Nouă</h3>
        <form onSubmit={handleAddOrg} className="add-org-form">
          <label htmlFor="orgName">Nume Organizație</label>
          <input
            id="orgName"
            type="text"
            value={newOrgName}
            onChange={(e) => setNewOrgName(e.target.value)}
            required
          />
          <label htmlFor="orgOwner">ID Proprietar</label>
          <input
            id="orgOwner"
            type="text"
            value={newOrgOwnerId}
            onChange={(e) => setNewOrgOwnerId(e.target.value)}
            required
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

export default AddOrganizationModal;
