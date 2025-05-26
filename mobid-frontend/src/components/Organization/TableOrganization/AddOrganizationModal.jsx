// src/components/Organization/TableOrganization/AddOrganizationModal.jsx
import React, { useState, useRef } from "react";
import { TextField } from "@mui/material";
import Select from "react-select";
import { getUsersPaged } from "../../../api/userApi";
import { createOrganization } from "../../../api/organizationApi";
import "../../../styles/components/modal/index.css";

export default function AddOrganizationModal({ onSuccess, onClose }) {
  const [name, setName]         = useState("");
  const [options, setOptions]   = useState([]);
  const [loading, setLoading]   = useState(false);
  const [selected, setSelected] = useState(null);
  const [error, setError]       = useState("");
  const loadedRef = useRef(false);

  // Încarcă utilizatorii o singură dată
  const loadUsers = async () => {
    if (loadedRef.current) return;
    loadedRef.current = true;
    setLoading(true);
    try {
      const res = await getUsersPaged({ pageIndex: 0, pageSize: 1000 });
      setOptions(
        (res.items || []).map(u => ({
          value: u.id,
          label: u.username,
          id:    u.id,
          username: u.username
        }))
      );
    } catch {
      setError("Nu am putut încărca utilizatorii.");
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async e => {
    e.preventDefault();
    setError("");
    if (!name.trim()) {
      setError("Introdu un nume pentru organizație.");
      return;
    }
    if (!selected) {
      setError("Selectează un proprietar.");
      return;
    }
    try {
      await createOrganization({ name, ownerId: selected.value });
      onSuccess();
      onClose();
    } catch (err) {
      setError("Nu am putut crea organizația: " + err.message);
    }
  };

  return (
    <div className="modal__overlay">
      <div className="modal__content">
        <button className="modal__close" onClick={onClose}>×</button>
        <h3 className="modal__title">Adaugă Organizație Nouă</h3>

        {error && <p className="modal__error">{error}</p>}

        <form onSubmit={handleSubmit} className="modal__form">
          <TextField
            id="orgName"
            label="Nume Organizație *"
            variant="outlined"
            value={name}
            onChange={e => setName(e.target.value)}
            fullWidth
            required
          />

          <Select
            options={options}
            isLoading={loading}
            onMenuOpen={loadUsers}
            value={selected}
            onChange={setSelected}
            placeholder="Proprietar…"
            className="modal__react-select"
            classNamePrefix="modal__react-select"
            formatOptionLabel={({ username, id }) => (
              <div className="modal__react-select__option">
                <span className="modal__react-select__option-label">
                  Name: {username}
                </span>
                <span className="modal__react-select__option-id">
                  Id: {id}
                </span>
              </div>
            )}

            /* —–––––––––––––––––––––––––––––––––––––––––––––––––––––—
               ➊ scoate meniul în portal, peste orice z-index
            —–––––––––––––––––––––––––––––––––––––––––––––––––––––— */
            menuPortalTarget={document.body}
            menuPosition="fixed"
            styles={{
              // ➋ se asigură că z-index-ul portalului e mai sus decât modalul
              menuPortal: base => ({ ...base, zIndex: 9999 }),

              // ➌ poți controla înălțimea maximă a listei dropdown
              menu: base => ({ ...base, maxHeight: 200 }),

              // restul stilurilor rămân la fel
            }}
          />

          <div className="modal__actions">
            <button type="submit" className="modal__button--yes">
              Salvează
            </button>
            <button type="button" className="modal__button--no" onClick={onClose}>
              Anulează
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
