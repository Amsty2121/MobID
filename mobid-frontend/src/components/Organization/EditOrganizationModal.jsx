// src/components/Organization/EditOrganizationModal.jsx
import React, { useEffect, useState } from "react";
import { FaTimes } from "react-icons/fa";
import Select from "react-select";
import { getUsersForOrganization } from "../../api/organizationApi";
import "./Organization.css";

const EditOrganizationModal = ({ organization, onSubmit, onClose }) => {
  const [newName, setNewName] = useState(organization.name);
  const [membersOptions, setMembersOptions] = useState([]);
  const [selectedOwner, setSelectedOwner] = useState(null);
  const [loadingMembers, setLoadingMembers] = useState(false);
  const [error, setError] = useState("");

  // Initializăm selectedOwner doar cu numele, nu cu "nume | id"
  useEffect(() => {
    const [usernameOnly] = organization.ownerName.split("|").map(s => s.trim());
    setSelectedOwner({
      value: organization.ownerId,
      label: usernameOnly,
      name: usernameOnly,
      id: organization.ownerId
    });
  }, [organization]);

  useEffect(() => {
    const fetchMembers = async () => {
      setLoadingMembers(true);
      try {
        const members = await getUsersForOrganization(organization.id);
        setMembersOptions(
          members.map(m => ({
            value: m.userId,
            label: m.userName,
            name: m.userName,
            id: m.userId
          }))
        );
      } catch {
        setError("Nu am putut încărca membrii organizației.");
      } finally {
        setLoadingMembers(false);
      }
    };
    fetchMembers();
  }, [organization.id]);

  const handleSubmit = e => {
    e.preventDefault();
    setError("");
    if (!selectedOwner) {
      setError("Trebuie să selectezi un proprietar.");
      return;
    }
    onSubmit({
      organizationId: organization.id,
      name: newName.trim() === "" ? null : newName,
      ownerId: selectedOwner.value
    });
  };

  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <button className="modal-close" onClick={onClose}><FaTimes/></button>
        <h3>Actualizează Organizația</h3>
        {error && <p className="error">{error}</p>}
        <form onSubmit={handleSubmit} className="edit-org-form">
          <label htmlFor="orgName">Nume Organizație</label>
          <input
            id="orgName"
            type="text"
            value={newName}
            onChange={e => setNewName(e.target.value)}
          />

          <label htmlFor="orgOwnerSelect">Proprietar (membru organizație)</label>
          <Select
            className="org-select"
            classNamePrefix="org-select"
            inputId="orgOwnerSelect"
            options={membersOptions}
            isLoading={loadingMembers}
            value={selectedOwner}
            onChange={setSelectedOwner}
            placeholder="Caută membru..."
            noOptionsMessage={() => "Nu s‑au găsit membri"}
            formatOptionLabel={({ name, id }) => (
              <div className="org-option">
                <div className="org-option-name"><strong>Name:</strong> {name}</div>
                <div className="org-option-id">Id: {id}</div>
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
};

export default EditOrganizationModal;
