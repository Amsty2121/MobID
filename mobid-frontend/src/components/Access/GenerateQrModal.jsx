// src/components/Access/GenerateQrModal.jsx
import React, { useState } from "react";
import { createQrCode } from "../../api/qrCodeApi";
import "./Access.css";

export default function GenerateQrModal({ accessId, onSuccess, onClose }) {
  const [eroare, setEroare] = useState("");

  const handleGenerate = async () => {
    setEroare("");
    try {
      await createQrCode(accessId);
      onSuccess();
    } catch (err) {
      setEroare("Nu am putut genera codul QR: " + err.message);
    }
  };

  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <button className="modal-close" onClick={onClose}>×</button>
        <h3>Generează Cod QR</h3>
        {eroare && <p className="error">{eroare}</p>}
        <div className="form-actions">
          <button onClick={handleGenerate}>Generate</button>
          <button onClick={onClose}>Cancel</button>
        </div>
      </div>
    </div>
  );
}
