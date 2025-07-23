// src/components/Organization/TableOrganization/AddMemberModal.jsx
import React, { useEffect, useState } from "react";
import { FaTimes } from "react-icons/fa";
import Select from "react-select";
import { getUsersPaged } from "../../../api/userApi";
import {
  getUsersForOrganization,
  addUserToOrganization
} from "../../../api/organizationApi";
import "../../../styles/components/modal/index.css";

const roleOptions = [
  { value: "Owner",  label: "Owner"  },
  { value: "Admin",  label: "Admin"  },
  { value: "Member", label: "Member" },
];

export default function AddMemberModal({
  organizationId,
  onSuccess,
  onClose
}) {
  const [allUsers, setAllUsers]                   = useState([]);
  const [existingMemberIds, setExistingMemberIds] = useState(new Set());
  const [selectedUser, setSelectedUser]           = useState(null);
  const [selectedRole, setSelectedRole]           = useState(roleOptions[2]);
  const [loading, setLoading]                     = useState(true);
  const [error, setError]                         = useState("");

  // exclude existing members
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

  const userOptions = allUsers
    .filter(u => !existingMemberIds.has(u.id))
    .map(u => ({ value: u.id, label: u.username }));

  const handleSubmit = async e => {
    e.preventDefault();
    setError("");
    if (!selectedUser) {
      setError("Alege mai întâi un utilizator.");
      return;
    }
    setLoading(true);
    try {
      await addUserToOrganization(organizationId, {
        userId: selectedUser.value,
        role:   selectedRole.value
      });
      onSuccess();
      onClose();
    } catch (err) {
      setError("Nu am putut adăuga membrul: " + err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="modal__overlay">
      <div className="modal__content">
        <button className="modal__close" onClick={onClose}>
          <FaTimes />
        </button>
        <h3 className="modal__title">Add member</h3>

        {error && <p className="modal__error">{error}</p>}

        <form onSubmit={handleSubmit} className="modal__form">
          <Select
            className="modal__react-select"
            classNamePrefix="modal__react-select"
            options={userOptions}
            isLoading={loading}
            value={selectedUser}
            onChange={setSelectedUser}
            placeholder="Find User…"
            noOptionsMessage={() => "Niciun utilizator disponibil"}
            formatOptionLabel={({ label, value }) => (
              <div className="modal__react-select__option">
                <span className="modal__react-select__option-label">
                  {label}
                </span>
                <span className="modal__react-select__option-id">
                  Id: {value}
                </span>
              </div>
            )}
            menuPortalTarget={document.body}
            menuPosition="fixed"
            styles={{
              menuPortal: base => ({ ...base, zIndex: 9999 }),
              menuList:   base => ({
                ...base,
                maxHeight: '200px',
                overflowY: 'auto'
              })
            }}
          />

          <Select
            className="modal__react-select"
            classNamePrefix="modal__react-select"
            options={roleOptions}
            value={selectedRole}
            onChange={setSelectedRole}
            placeholder="Selectează rol…"
          />

          <div className="modal__actions">
            <button
              type="submit"
              className="modal__button--yes"
              disabled={loading}
            >
              {loading ? "Add…" : "Add"}
            </button>
            <button
              type="button"
              className="modal__button--no"
              onClick={onClose}
              disabled={loading}
            >
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
