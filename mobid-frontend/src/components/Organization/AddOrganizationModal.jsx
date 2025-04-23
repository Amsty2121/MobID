// src/components/Organization/AddOrganizationModal.jsx
import React, { useEffect, useState } from "react";
import Select from "react-select";
import { getUsersPaged } from "../../api/userApi";
import { createOrganization } from "../../api/organizationApi";
import "./Organization.css";

export default function AddOrganizationModal({ onSuccess, onClose }) {
  const [name, setName] = useState("");
  const [allUsers, setAllUsers] = useState([]);
  const [selectedOwner, setSelectedOwner] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    (async () => {
      try {
        const { items } = await getUsersPaged({ pageIndex: 0, pageSize: 1000 });
        setAllUsers(items || []);
      } catch {
        setError("Nu am putut încărca lista de utilizatori.");
      } finally {
        setLoading(false);
      }
    })();
  }, []);

  const ownerOptions = allUsers.map(u => ({
    value: u.id,
    name:  u.username,
    id:    u.id,
    label: u.username, // fallback label
  }));

  const handleSubmit = async e => {
    e.preventDefault();
    setError("");
    if (!name.trim()) {
      setError("Introdu un nume pentru organizație.");
      return;
    }
    if (!selectedOwner) {
      setError("Selectează un proprietar.");
      return;
    }
    try {
      await createOrganization({
        name,
        ownerId: selectedOwner.value
      });
      onSuccess();
    } catch (err) {
      setError("Nu am putut crea organizația: " + err.message);
    }
  };

  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <button className="modal-close" onClick={onClose}>×</button>
        <h3>Adaugă Organizație Nouă</h3>
        {(error || loading) && (
          <p className="error">{ error || "Se încarcă utilizatorii..." }</p>
        )}
        <form onSubmit={handleSubmit} className="add-org-form">
          <label htmlFor="orgName">Nume Organizație</label>
          <input
            id="orgName"
            type="text"
            value={name}
            onChange={e => setName(e.target.value)}
            required
          />

          <label htmlFor="ownerSelect">Proprietar (user)</label>
          <Select
            inputId="ownerSelect"
            className="org-select"
            classNamePrefix="org-select"
            options={ownerOptions}
            isLoading={loading}
            value={selectedOwner}
            onChange={setSelectedOwner}
            placeholder="Caută proprietar..."
            noOptionsMessage={() => "Nu s‑au găsit utilizatori"}
            formatOptionLabel={({ name, id }) => (
              <div className="org-option">
                <div className="org-option-name">
                  <strong>Name:</strong> {name}
                </div>
                <div className="org-option-id">
                  Id: {id}
                </div>
              </div>
            )}
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
 