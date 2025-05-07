// src/components/Access/AddAccessModal.jsx
import React, { useEffect, useState } from "react";
import Select from "react-select";
import { getAccessTypes, createAccess } from "../../api/accessApi";
import "./Access.css";

export default function AddAccessModal({ organizationId, onSuccess, onClose }) {
  const [types, setTypes] = useState([]);
  const [selectedType, setSelectedType] = useState(null);
  const [expiration, setExpiration] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    (async () => {
      setLoading(true);
      try {
        const typesData = await getAccessTypes();
        setTypes(typesData.map(t => ({ value: t.id, label: t.name })));
      } catch {
        setError("Nu am putut încărca tipurile de acces.");
      } finally {
        setLoading(false);
      }
    })();
  }, []);

  const handleSubmit = async e => {
    e.preventDefault();
    setError("");
    if (!selectedType) {
      setError("Selectează un tip de acces.");
      return;
    }
    // build payload
    const payload = {
      organizationId,
      accessTypeId: selectedType.value,
      // only include expirationDate if user picked one
      ...(expiration
        ? { expirationDate: new Date(expiration).toISOString() }
        : {})
    };
    try {
      await createAccess(payload);
      onSuccess();
      onClose();
    } catch (err) {
      setError("Eroare la crearea accesului: " + err.message);
    }
  };

  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <button className="modal-close" onClick={onClose}>×</button>
        <h3>Adaugă Acces</h3>
        {error && <p className="error">{error}</p>}
        <form onSubmit={handleSubmit} className="add-org-form">
          <label htmlFor="accessTypeSelect">Tip Acces</label>
          <Select
            inputId="accessTypeSelect"
            className="org-select"
            classNamePrefix="org-select"
            options={types}
            isLoading={loading}
            value={selectedType}
            onChange={setSelectedType}
            placeholder="Alege tipul de acces..."
            noOptionsMessage={() => "Nu s-au găsit tipuri"}
            formatOptionLabel={({ label, value }) => (
              <div className="org-option">
                <div className="org-option-name">
                  <strong>Tip:</strong> {label}
                </div>
                <div className="org-option-id">
                  Id: {value}
                </div>
              </div>
            )}
          />

          <label htmlFor="expirationDate" style={{ marginTop: "1rem" }}>
            Data expirării (opțional)
          </label>
          <input
            id="expirationDate"
            type="date"
            value={expiration}
            onChange={e => setExpiration(e.target.value)}
          />

          <div className="form-actions">
            <button type="submit">Salvează</button>
            <button type="button" onClick={onClose}>Anulează</button>
          </div>
        </form>
      </div>
    </div>
  );
}
