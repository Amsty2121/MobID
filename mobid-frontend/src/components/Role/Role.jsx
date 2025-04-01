// src/components/Role/Role.jsx
import React, { useEffect, useState } from "react";
import {
  getRolesPaged,
  addRole,
  deleteRole,
} from "../../api/roleApi";
import { FaTrash, FaPlus, FaTimes } from "react-icons/fa";
import "./Role.css";

const Role = () => {
  // ========== State pentru paginare ==========
  const [roles, setRoles] = useState([]);
  const [currentPage, setCurrentPage] = useState(0); // PageIndex
  const [limit, setLimit] = useState(5);             // PageSize
  const [totalCount, setTotalCount] = useState(0);

  // ========== State pentru filtrare ==========
  const [searchTerm, setSearchTerm] = useState("");

  // ========== State pentru adăugare ==========
  const [showAddModal, setShowAddModal] = useState(false);
  const [newRoleName, setNewRoleName] = useState("");
  const [newRoleDescription, setNewRoleDescription] = useState("");

  // ========== Stări ajutătoare ==========
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  /**
   * Preia lista de roluri paginată de pe server.
   * pageIndex = currentPage
   * pageSize = limit
   */
  const fetchRoles = async () => {
    setLoading(true);
    setError("");
    try {
      // Apelăm getRolesPaged cu un obiect PagedRequest
      const data = await getRolesPaged({
        pageIndex: currentPage,
        pageSize: limit,
      });

      // data are structura: { pageIndex, pageSize, total, items }
      const list = data.items || [];
      setRoles(list);
      setTotalCount(data.total || 0);
    } catch (err) {
      console.error(err);
      setError("Eroare la preluarea rolurilor.");
    } finally {
      setLoading(false);
    }
  };

  // Efect: reîncărcăm lista de roluri când currentPage sau limit se schimbă
  useEffect(() => {
    fetchRoles();
    // eslint-disable-next-line
  }, [currentPage, limit]);

  // ========== Filtrare locală (opțional) ==========
  const filteredRoles = roles.filter((role) => {
    const term = searchTerm.toLowerCase();
    return (
      role.name.toLowerCase().includes(term) ||
      (role.description || "").toLowerCase().includes(term)
    );
  });

  // Daca vrei să folosești strict filtrarea locală, atunci se ignore server side filtering
  // (Poți face la fel un request server-side cu param. searchTerm)
  const paginatedRoles = filteredRoles;

  // ========== Adăugare rol ==========
  const handleAddRole = async (e) => {
    e.preventDefault();
    setError("");
    try {
      await addRole(newRoleName, newRoleDescription);
      setShowAddModal(false);
      setNewRoleName("");
      setNewRoleDescription("");
      setCurrentPage(0);
      fetchRoles();
    } catch (err) {
      console.error(err);
      setError("Eroare la adăugarea rolului.");
    }
  };

  // ========== Ștergere rol ==========
  const handleDeleteRole = async (roleId) => {
    if (!window.confirm("Sigur ștergi acest rol?")) return;
    setError("");
    try {
      await deleteRole(roleId);
      fetchRoles();
    } catch (err) {
      console.error(err);
      setError("Eroare la ștergerea rolului.");
    }
  };

  // ========== Paginare (Anterior / Următoare) ==========
  const totalPages = Math.ceil(totalCount / limit);

  const goToPreviousPage = () => {
    if (currentPage > 0) {
      setCurrentPage((prev) => prev - 1);
    }
  };

  const goToNextPage = () => {
    if (currentPage + 1 < totalPages) {
      setCurrentPage((prev) => prev + 1);
    }
  };

  return (
    <div className="role-page">
      {/* Titlu centrat, de culoare oranj */}
      <h2 className="role-heading">Roluri</h2>

      <div className="role-table-container">
        {/* Rând pentru filtrare (stânga) și plus (dreapta) */}
        <div className="filter-plus-row">
          <div className="filter-container">
            <input
              type="text"
              placeholder="Filtrează după nume/descriere..."
              value={searchTerm}
              onChange={(e) => {
                setSearchTerm(e.target.value);
                setCurrentPage(0); // reset la pagina 0 când se modifică filtrul
              }}
            />
          </div>
          <div
            className="add-role-icon"
            title="Adaugă rol nou"
            onClick={() => setShowAddModal(true)}
          >
            <FaPlus />
          </div>
        </div>

        {loading && <p>Se încarcă...</p>}
        {error && <p className="error">{error}</p>}
        {!loading && paginatedRoles.length === 0 && (
          <p className="no-results">Nu există roluri pentru filtrul curent.</p>
        )}

        {paginatedRoles.length > 0 && (
          <table className="role-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Nume</th>
                <th>Descriere</th>
                <th className="actions-col">Acțiuni</th>
              </tr>
            </thead>
            <tbody>
              {paginatedRoles.map((role) => (
                <tr key={role.id}>
                  <td>{role.id}</td>
                  <td>{role.name}</td>
                  <td>{role.description}</td>
                  <td className="actions-col">
                    <button
                      className="icon-btn"
                      onClick={() => handleDeleteRole(role.id)}
                    >
                      <FaTrash />
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}

        {/* Paginare */}
        <div className="pagination">
          <div className="pagination-left">
            <button onClick={goToPreviousPage} disabled={currentPage === 0}>
              Anterior
            </button>
            <span>
              Pagina {currentPage + 1} din {totalPages || 1}
            </span>
            <button
              onClick={goToNextPage}
              disabled={currentPage + 1 >= totalPages}
            >
              Următoare
            </button>
          </div>
          <div className="pagination-right">
            <label htmlFor="limitSelect">Pe pagină:</label>
            <select
              id="limitSelect"
              value={limit}
              onChange={(e) => {
                setLimit(Number(e.target.value));
                setCurrentPage(0);
              }}
            >
              <option value="5">5</option>
              <option value="10">10</option>
              <option value="20">20</option>
            </select>
            <span>Total: {totalCount}</span>
          </div>
        </div>
      </div>

      {/* Modal de adăugare rol */}
      {showAddModal && (
        <div className="modal-overlay">
          <div className="modal-content">
            <button
              className="modal-close"
              onClick={() => setShowAddModal(false)}
            >
              <FaTimes />
            </button>
            <h3>Adaugă Rol Nou</h3>
            <form onSubmit={handleAddRole} className="add-role-form">
              <label htmlFor="roleName">Nume Rol</label>
              <input
                id="roleName"
                type="text"
                value={newRoleName}
                onChange={(e) => setNewRoleName(e.target.value)}
                required
              />
              <label htmlFor="roleDesc">Descriere</label>
              <input
                id="roleDesc"
                type="text"
                value={newRoleDescription}
                onChange={(e) => setNewRoleDescription(e.target.value)}
              />
              <div className="form-actions">
                <button type="submit">Salvează</button>
                <button type="button" onClick={() => setShowAddModal(false)}>
                  Anulează
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default Role;
