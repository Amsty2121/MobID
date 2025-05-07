// src/components/Access/GenerateQrModal.jsx
import React, { useState } from "react";
import { generateQrCode } from "../../api/qrCodeApi";
import "./Access.css";

export default function GenerateQrModal({ accessId, onSuccess, onClose }) {
  const [eroare, setEroare] = useState("");

  const handleGenerate = async () => {
    setEroare("");
    try {
      await generateQrCode(accessId);
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
          <button onClick={handleGenerate}>Generează</button>
          <button onClick={onClose}>Anulează</button>
        </div>
      </div>
    </div>
  );
}
