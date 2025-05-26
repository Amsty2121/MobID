/* src/components/User/Table/DeleteUserModal.jsx */
import React from "react";
import { FaTimes } from "react-icons/fa";
import "../../../styles/components/modal/index.css";

const DeleteUserModal = ({ user, onConfirm, onCancel }) => {
  return (
    <div className="modal__overlay">
      <div className="modal__content">
        <button className="modal__close" onClick={onCancel}>
          <FaTimes />
        </button>
        <h3 className="modal__title">Confirmă Ștergerea</h3>
        <p className="modal__message">
          Ești sigur că vrei să ștergi utilizatorul{" "}
          <strong>{user?.username}</strong>?
        </p>
        <div className="modal__actions">
          <button className="modal__button--yes" onClick={onConfirm}>
            Șterge
          </button>
          <button className="modal__button--no" onClick={onCancel}>
            Anulează
          </button>
        </div>
      </div>
    </div>
  );
};

export default DeleteUserModal;
