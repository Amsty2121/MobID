// src/components/User/EditUserRolesModal.jsx
import React, { useEffect, useState } from "react";
import { FaTimes } from "react-icons/fa";
import {
  getAllRoles,
  assignRoleToUser,
  removeRoleFromUser,
  getUserRoles
} from "../../api/userApi";
import "./User.css";

const EditUserRolesModal = ({ user, onClose }) => {
  const [allRoles, setAllRoles] = useState([]);
  const [userRoles, setUserRoles] = useState([]);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchRoles = async () => {
      try {
        const rolesData = await getAllRoles();
        setAllRoles(rolesData);
      } catch (err) {
        console.error(err);
        setError("Eroare la preluarea rolurilor.");
      }
    };
    fetchRoles();
  }, []);

  useEffect(() => {
    const fetchUserRoles = async () => {
      try {
        const roles = await getUserRoles(user.id);
        setUserRoles(roles);
      } catch (err) {
        console.error(err);
        setError("Eroare la preluarea rolurilor utilizatorului.");
      }
    };
    fetchUserRoles();
  }, [user.id]);

  const handleAssignRole = async (roleId) => {
    try {
      await assignRoleToUser(user.id, roleId);
      const updatedRoles = await getUserRoles(user.id);
      setUserRoles(updatedRoles);
    } catch (err) {
      console.error(err);
      setError("Eroare la atribuirea rolului.");
    }
  };

  const handleRemoveRole = async (roleName) => {
    try {
      const roleObj = allRoles.find((r) => r.name === roleName);
      if (!roleObj) return;
      await removeRoleFromUser(user.id, roleObj.id);
      const updatedRoles = await getUserRoles(user.id);
      setUserRoles(updatedRoles);
    } catch (err) {
      console.error(err);
      setError("Eroare la eliminarea rolului.");
    }
  };

  const availableRoles = allRoles.filter((r) => !userRoles.includes(r.name));

  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <button className="modal-close" onClick={onClose}>
          <FaTimes />
        </button>
        <h3>Editează Rolurile pentru {user.username}</h3>
        {error && <p className="error">{error}</p>}

        <div>
          <h4>Roluri Atribuite:</h4>
          {userRoles.length === 0 ? (
            <p>Niciun rol atribuit.</p>
          ) : (
            <ul className="role-list">
              {userRoles.map((roleName, index) => (
                <li key={index} className="role-list-item">
                  {roleName}
                  <button
                    className="remove-role-btn"
                    onClick={() => handleRemoveRole(roleName)}
                  >
                    Elimină
                  </button>
                </li>
              ))}
            </ul>
          )}
        </div>

        <div style={{ marginTop: "1rem" }}>
          <h4>Roluri Disponibile:</h4>
          {availableRoles.length === 0 ? (
            <p>Toate rolurile sunt deja atribuite.</p>
          ) : (
            <ul className="role-list">
              {availableRoles.map((role) => (
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
        </div>
      </div>
    </div>
  );
};

export default EditUserRolesModal;
