// src/components/Organization/TableOrganization/EditOrganizationModal.jsx
import React, { useState, useEffect, useRef } from "react";
import { FaTimes } from "react-icons/fa";
import TextField from "@mui/material/TextField";
import Select from "react-select";
import { getUsersForOrganization } from "../../../api/organizationApi";
// importăm stilurile comune pentru modale (layout, buttons, forms, MUI overrides, react-select, etc.)
import "../../../styles/components/modal/index.css";

export default function EditOrganizationModal({ organization, onSubmit, onClose }) {
  const [newName, setNewName]        = useState("");
  const [members, setMembers]        = useState([]);
  const [selectedOwner, setSelectedOwner] = useState(null);
  const [loading, setLoading]        = useState(true);
  const [error, setError]            = useState("");
  const didFetch = useRef(false);

  useEffect(() => {
    // initialize form fields
    setNewName(organization.name);
    setError("");
    setSelectedOwner({
      value: organization.ownerId,
      label: organization.ownerName,
      id: organization.ownerId,
      username: organization.ownerName
    });

    if (didFetch.current) return;
    didFetch.current = true;

    (async () => {
      try {
        const opts = await getUsersForOrganization(organization.id);
        setMembers(
          opts.map(m => ({
            value: m.userId,
            label: m.userName,
            id: m.userId,
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
            options={members}
            isLoading={loading}
            value={selectedOwner}
            onChange={setSelectedOwner}
            placeholder="Alege proprietar…"
            className="modal__react-select"
            classNamePrefix="modal__react-select"
            formatOptionLabel={({ username, id }) => (
              <div style={{ display: "flex", flexDirection: "column" }}>
                <span style={{ fontWeight: 600, color: "var(--color-primary)" }}>
                  Name: {username}
                </span>
                <span style={{
                  color: "var(--color-iron)",
                  fontSize: "0.85rem",
                  marginTop: 4
                }}>
                  Id: {id}
                </span>
              </div>
            )}
            theme={theme => ({
              ...theme,
              borderRadius: 4,
              colors: {
                ...theme.colors,
                primary25: "var(--color-primary-hover)",
                primary:   "var(--color-primary)"
              }
            })}
            styles={{
              control: base => ({ ...base, minHeight: 56 }),
              valueContainer: base => ({ ...base, height: 56, padding: "0 8px" }),
              indicatorsContainer: base => ({ ...base, height: 56 }),
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
