// src/components/User/Table/EditUserRolesModal.jsx
import React, { useEffect, useState, useRef } from "react";
import { FaTimes } from "react-icons/fa";
import { assignRoleToUser, removeRoleFromUser, getUserRoles } from "../../../api/userApi";
import { getAllRoles } from "../../../api/roleApi";
import "../User.css";

const EditUserRolesModal = ({ user, onClose }) => {
  const [allRoles, setAllRoles]     = useState([]);
  const [userRoles, setUserRoles]   = useState([]);
  const [error, setError]           = useState("");
  const didFetchRef = useRef(false);

  useEffect(() => {
    if (didFetchRef.current) return;
    didFetchRef.current = true;

    (async () => {
      setError("");
      try {
        const [rolesData, rolesForUser] = await Promise.all([
          getAllRoles(),
          getUserRoles(user.id)
        ]);
        setAllRoles(rolesData);
        setUserRoles(rolesForUser);
      } catch (err) {
        console.error(err);
        setError("Eroare la preluarea rolurilor.");
      }
    })();
  }, [user.id]);

  const refreshUserRoles = async () => {
    try {
      const roles = await getUserRoles(user.id);
      setUserRoles(roles);
    } catch {
      setError("Eroare la reîmprospătarea rolurilor utilizatorului.");
    }
  };

  const handleAssignRole = async (roleId) => {
    try {
      await assignRoleToUser(user.id, roleId);
      await refreshUserRoles();
    } catch (err) {
      console.error(err);
      setError("Eroare la atribuirea rolului.");
    }
  };

  const handleRemoveRole = async (roleId) => {
    try {
      await removeRoleFromUser(user.id, roleId);
      await refreshUserRoles();
    } catch (err) {
      console.error(err);
      setError("Eroare la eliminarea rolului.");
    }
  };

  const availableRoles = allRoles.filter(r => !userRoles.includes(r.name));

  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <button className="modal-close" onClick={onClose}>
          <FaTimes />
        </button>
        <h3>Editează Rolurile pentru {user.username}</h3>
        {error && <p className="error">{error}</p>}

        <section>
          <h4>Roluri Atribuite:</h4>
          {userRoles.length === 0 ? (
            <p>Niciun rol atribuit.</p>
          ) : (
            <ul className="role-list">
              {userRoles.map((roleName, idx) => {
                const roleObj = allRoles.find(r => r.name === roleName);
                return (
                  <li key={idx} className="role-list-item">
                    {roleName}
                    {roleObj && (
                      <button
                        className="remove-role-btn"
                        onClick={() => handleRemoveRole(roleObj.id)}
                      >
                        Elimină
                      </button>
                    )}
                  </li>
                );
              })}
            </ul>
          )}
        </section>

        <section style={{ marginTop: "1rem" }}>
          <h4>Roluri Disponibile:</h4>
          {availableRoles.length === 0 ? (
            <p>Toate rolurile sunt deja atribuite.</p>
          ) : (
            <ul className="role-list">
              {availableRoles.map(role => (
                <li key={role.id} className="role-list-item">
                  {role.name}
                  <button
                    className="add-role-btn"
                    onClick={() => handleAssignRole(role.id)}
                  >
                    Adaugă
                  </button>
                </li>
              ))}
            </ul>
          )}
        </section>
      </div>
    </div>
  );
};

export default EditUserRolesModal;
