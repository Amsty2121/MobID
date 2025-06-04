// src/components/Organization/TableOrganization/EditOrganizationModal.jsx
import React, { useState, useEffect, useRef } from "react";
import { FaTimes } from "react-icons/fa";
import TextField from "@mui/material/TextField";
import Select from "react-select";
import { getUsersForOrganization } from "../../../api/organizationApi";
// stilurile comune pentru modal + overrides MUI
import "../../../styles/components/modal/index.css";

export default function EditOrganizationModal({ organization, onSubmit, onClose }) {
  const [newName, setNewName]        = useState("");
  const [members, setMembers]        = useState([]);
  const [selectedOwner, setSelected] = useState(null);
  const [loading, setLoading]        = useState(true);
  const [error, setError]            = useState("");
  const didFetch = useRef(false);

  useEffect(() => {
    // initialize form cu datele organizației
    setNewName(organization.name);
    setError("");
    setSelected({ value: organization.ownerId, label: organization.ownerName });

    if (didFetch.current) return;
    didFetch.current = true;

    (async () => {
      try {
        const opts = await getUsersForOrganization(organization.id);
        setMembers(
          opts.map(m => ({
            value: m.userId,
            label: m.userName,
            id:    m.userId,
            username: m.userName
          }))
        );
      } catch {
        setError("Nu am putut încărca membrii.");
      } finally {
        setLoading(false);
      }
    })();
  }, [organization]);

  const handleSubmit = async e => {
    e.preventDefault();
    if (!selectedOwner) {
      setError("Trebuie să selectezi un proprietar.");
      return;
    }
    setError("");
    await onSubmit({
      organizationId: organization.id,
      name:           newName.trim() || null,
      ownerId:        selectedOwner.value
    });
    onClose();
  };

  return (
    <div className="modal__overlay">
      <div className="modal__content">
        <button className="modal__close" onClick={onClose}>
          <FaTimes />
        </button>
        <h3 className="modal__title">Actualizează Organizația</h3>
        {error && <p className="modal__error">{error}</p>}

        <form onSubmit={handleSubmit} className="modal__form">
          <TextField
            label="Nume Organizație"
            value={newName}
            onChange={e => setNewName(e.target.value)}
            required
            variant="outlined"
            fullWidth
          />

          <Select
            className="modal__react-select"
            classNamePrefix="modal__react-select"
            options={members}
            isLoading={loading}
            value={selectedOwner}
            onChange={setSelected}
            placeholder="Alege proprietar…"
            noOptionsMessage={() => "Niciun membru găsit"}
            formatOptionLabel={({ label, value }) => (
              <div className="modal__react-select__option">
                <span className="modal__react-select__option-label">
                  Name: {label}
                </span>
                <span className="modal__react-select__option-id">
                  Id: {value}
                </span>
              </div>
            )}
            // portal și poziționare fixă
            menuPortalTarget={document.body}
            menuPosition="fixed"
            // asigurăm z-index mare și scroll doar pe listă
            styles={{
              menuPortal: base => ({ ...base, zIndex: 9999 }),
              menuList:   base => ({
                ...base,
                maxHeight: "400px",
                overflowY: "auto"
              })
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
