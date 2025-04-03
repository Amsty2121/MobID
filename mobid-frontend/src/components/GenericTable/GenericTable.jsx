// src/components/GenericTable/GenericTable.jsx
import React, { useEffect, useState } from "react";
import { FaPlus, FaTrash, FaEdit } from "react-icons/fa";
import "./GenericTable.css";

const GenericTable = ({
  title,
  columns,
  filterColumns,
  data,
  onAdd,
  showAddOption,
  onDelete,
  showDeleteOption,
  onEdit,          
  showEditOption,  
  currentPage,
  totalCount,
  pageSize,
  onPageChange,
  onPageSizeChange,
}) => {
  const [searchTerm, setSearchTerm] = useState("");
  const [filteredData, setFilteredData] = useState(data || []);

  // Filtrare locală pe baza coloanelor specificate
  useEffect(() => {
    if (!data) {
      setFilteredData([]);
      return;
    }
    const lowerSearch = searchTerm.toLowerCase();
    const filtered = data.filter((row) =>
      filterColumns.some((col) => {
        const cell = row[col];
        return cell && cell.toString().toLowerCase().includes(lowerSearch);
      })
    );
    setFilteredData(filtered);
  }, [searchTerm, data, filterColumns]);

  const totalPages = Math.ceil(totalCount / pageSize);

  return (
    <div className="generic-table-page">
      {/* Titlu centrat */}
      <h2 className="role-heading">{title}</h2>

      {/* Container cu fundal diferit pentru filtrare și tabel */}
      <div className="role-table-container">
        {/* Rând pentru filtrare și, opțional, butonul de adăugare */}
        <div className="filter-plus-row">
          <div className="filter-container">
            <input
              type="text"
              placeholder="Filtrează după nume/descriere..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
          {showAddOption && (
            <div
              className="add-role-icon"
              title="Adaugă"
              onClick={onAdd}
            >
              <FaPlus />
            </div>
          )}
        </div>

        {filteredData.length === 0 ? (
          <p className="no-results">Nu există date pentru filtrul curent.</p>
        ) : (
          <table className="role-table">
            <thead>
              <tr>
                {columns.map((col, index) => (
                  <th key={index}>{col.header}</th>
                ))}
                {showEditOption && <th className="actions-col">Edit</th>}
                {showDeleteOption && <th className="actions-col">Acțiuni</th>}
              </tr>
            </thead>
            <tbody>
              {filteredData.map((row, index) => (
                <tr key={index}>
                  {columns.map((col, idx) => (
                    <td key={idx}>{row[col.accessor]}</td>
                  ))}
                  {showEditOption && (
                    <td className="actions-col">
                      <button
                        className="icon-btn"
                        onClick={() => onEdit && onEdit(row)}
                      >
                        <FaEdit />
                      </button>
                    </td>
                  )}
                  {showDeleteOption && (
                    <td className="actions-col">
                      <button
                        className="icon-btn"
                        onClick={() => onDelete(row)}
                      >
                        <FaTrash />
                      </button>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        )}

        {/* Paginare */}
        <div className="pagination">
          <div className="pagination-left">
            <button
              onClick={() => onPageChange(currentPage - 1)}
              disabled={currentPage === 0}
            >
              Anterior
            </button>
            <span>
              Pagina {currentPage + 1} din {totalPages || 1}
            </span>
            <button
              onClick={() => onPageChange(currentPage + 1)}
              disabled={currentPage + 1 >= totalPages}
            >
              Următoare
            </button>
          </div>
          <div className="pagination-right">
            <label htmlFor="pageSizeSelect">Pe pagină:</label>
            <select
              id="pageSizeSelect"
              value={pageSize}
              onChange={(e) => onPageSizeChange(Number(e.target.value))}
            >
              <option value="5">5</option>
              <option value="10">10</option>
              <option value="20">20</option>
            </select>
            <span>Total: {totalCount}</span>
          </div>
        </div>
      </div>
    </div>
  );
};

export default GenericTable;
