// src/components/Organization/TableOrganization/DeleteOrganizationModal.jsx
import React from "react";
import { FaTimes } from "react-icons/fa";
// importăm stilurile comune pentru modale
import "../../../styles/components/modal/index.css";

const DeleteOrganizationModal = ({ organization, onConfirm, onCancel }) => {
  return (
    <div className="modal__overlay">
      <div className="modal__content">
        <button className="modal__close" onClick={onCancel}>
          <FaTimes />
        </button>
        <h3 className="modal__title">Confirm Delete</h3>
        <p className="modal__message">
          Are you sure you want to delete the organization{" "}
          <strong>{organization?.name}</strong>?
        </p>
        <div className="modal__actions">
          <button
            type="button"
            className="modal__button--yes"
            onClick={onConfirm}
          >
            Delete
          </button>
          <button
            type="button"
            className="modal__button--no"
            onClick={onCancel}
          >
            Cancel
          </button>
        </div>
      </div>
    </div>
  );
};

export default DeleteOrganizationModal;
