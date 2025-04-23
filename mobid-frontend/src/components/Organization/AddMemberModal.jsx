// src/components/Organization/AddMemberModal.jsx
import React, { useEffect, useState } from "react";
import Select from "react-select";
import { getUsersPaged } from "../../api/userApi";
import {
  getUsersForOrganization,
  addUserToOrganization
} from "../../api/organizationApi";
import "./Organization.css";

const roleOptions = [
  { value: "Owner",  label: "Owner"  },
  { value: "Admin",  label: "Admin"  },
  { value: "Member", label: "Member" },
];

export default function AddMemberModal({ organizationId, onSuccess, onClose }) {
  const [allUsers, setAllUsers] = useState([]);
  const [existingMemberIds, setExistingMemberIds] = useState(new Set());
  const [selectedUser, setSelectedUser] = useState(null);
  const [selectedRole, setSelectedRole] = useState(roleOptions[2]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // load existing members so we can exclude them
  useEffect(() => {
    (async () => {
      try {
        const members = await getUsersForOrganization(organizationId);
        setExistingMemberIds(new Set(members.map(m => m.userId)));
      } catch {
        setError("Nu am putut încărca membrii existenți.");
      }
    })();
  }, [organizationId]);

  // load all users
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

  // filter out existing members
  const userOptions = allUsers
    .filter(u => !existingMemberIds.has(u.id))
    .map(u => ({
      value: u.id,
      name: u.username,
      id: u.id,
      label: u.username,
    }));

  const handleSubmit = async e => {
    e.preventDefault();
    setError("");
    if (!selectedUser) {
      setError("Alege mai întâi un utilizator.");
      return;
    }
    try {
      await addUserToOrganization(organizationId, {
        userId: selectedUser.value,
        role:   selectedRole.value
      });
      onSuccess();
    } catch (err) {
      setError("Nu am putut adăuga membrul: " + err.message);
    }
  };

  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <button className="modal-close" onClick={onClose}>×</button>
        <h3>Adaugă Membru</h3>
        {(error || loading) && (
          <p className="error">
            {error || "Se încarcă datele..."}
          </p>
        )}
        <form onSubmit={handleSubmit} className="edit-org-form">
          <label htmlFor="memberSelect">Utilizator</label>
          <Select
            inputId="memberSelect"
            className="org-select"
            classNamePrefix="org-select"
            options={userOptions}
            isLoading={loading}
            value={selectedUser}
            onChange={setSelectedUser}
            placeholder="Caută utilizator..."
            noOptionsMessage={() => "Niciun utilizator disponibil"}
            formatOptionLabel={({ name, id }) => (
              <div className="org-option">
                <div className="org-option-name"><strong>Name:</strong> {name}</div>
                <div className="org-option-id">Id: {id}</div>
              </div>
            )}
          />

          <label htmlFor="roleSelect" style={{ marginTop: "1rem" }}>
            Rol în organizație
          </label>
          <Select
            inputId="roleSelect"
            className="org-select"
            classNamePrefix="org-select"
            options={roleOptions}
            value={selectedRole}
            onChange={setSelectedRole}
            placeholder="Selectează rol..."
          />

          <div className="form-actions">
            <button type="submit">Adaugă</button>
            <button type="button" onClick={onClose}>Anulează</button>
          </div>
        </form>
      </div>
    </div>
  );
}
