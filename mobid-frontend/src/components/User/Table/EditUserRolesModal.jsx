/* src/components/User/Table/EditUserRolesModal.jsx */
import React, { useEffect, useState, useRef } from "react";
import { FaTimes } from "react-icons/fa";
import {
  assignRoleToUser,
  removeRoleFromUser,
  getUserRoles
} from "../../../api/userApi";
import { getAllRoles } from "../../../api/roleApi";
import "../../../styles/components/modal/index.css";

const EditUserRolesModal = ({ user, onClose }) => {
  const [allRoles, setAllRoles]   = useState([]);
  const [userRoles, setUserRoles] = useState([]);
  const [error, setError]         = useState("");
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
      } catch {
        setError("Eroare la preluarea rolurilor.");
      }
    })();
  }, [user.id]);

  const refreshUserRoles = async () => {
    try {
      const roles = await getUserRoles(user.id);
      setUserRoles(roles);
    } catch {
      setError("Eroare la reîmprospătarea rolurilor.");
    }
  };

  const handleAssignRole = async (roleId) => {
    try {
      await assignRoleToUser(user.id, roleId);
      await refreshUserRoles();
    } catch {
      setError("Eroare la atribuirea rolului.");
    }
  };

  const handleRemoveRole = async (roleId) => {
    try {
      await removeRoleFromUser(user.id, roleId);
      await refreshUserRoles();
    } catch {
      setError("Eroare la eliminarea rolului.");
    }
  };

  const availableRoles = allRoles.filter(r => !userRoles.includes(r.name));

  return (
    <div className="modal__overlay">
      <div className="modal__content">
        <button className="modal__close" onClick={onClose}>
          <FaTimes />
        </button>
        <h3 className="modal__title">Editează Rolurile pentru {user.username}</h3>
        {error && <p className="modal__error">{error}</p>}

        <div className="modal__section">
          <h4>Roluri Atribuite:</h4>
          {userRoles.length === 0 ? (
            <p>Niciun rol atribuit.</p>
          ) : (
            <ul className="modal__role-list">
              {userRoles.map((roleName, idx) => {
                const roleObj = allRoles.find(r => r.name === roleName);
                return (
                  <li key={idx} className="modal__role-item">
                    {roleName}
                    {roleObj && (
                      <button
                        className="modal__button--no"
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
        </div>

        <div className="modal__section">
          <h4>Roluri Disponibile:</h4>
          {availableRoles.length === 0 ? (
            <p>Toate rolurile sunt deja atribuite.</p>
          ) : (
            <ul className="modal__role-list">
              {availableRoles.map(role => (
                <li key={role.id} className="modal__role-item">
                  {role.name}
                  <button
                    className="modal__button--yes"
                    onClick={() => handleAssignRole(role.id)}
                  >
                    Adaugă
                  </button>
                </li>
              ))}
            </ul>
          )}
        </div>
      </div>
    </div>
  );
};

export default EditUserRolesModal;
