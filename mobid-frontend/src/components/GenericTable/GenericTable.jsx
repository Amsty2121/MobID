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
  onRowClick,           // nou
  currentPage,
  totalCount,
  pageSize,
  onPageChange,
  onPageSizeChange,
}) => {
  const [searchTerm, setSearchTerm] = useState("");
  const [filteredData, setFilteredData] = useState(data || []);

  useEffect(() => {
    if (!data) return setFilteredData([]);
    const lower = searchTerm.toLowerCase();
    setFilteredData(
      data.filter(row =>
        filterColumns.some(col => {
          const cell = row[col];
          return cell && cell.toString().toLowerCase().includes(lower);
        })
      )
    );
  }, [searchTerm, data, filterColumns]);

  const totalPages = Math.ceil(totalCount / pageSize);

  return (
    <div className="generic-table-page">
      <h2 className="role-heading">{title}</h2>
      <div className="role-table-container">
        <div className="filter-plus-row">
          <div className="filter-container">
            <input
              type="text"
              placeholder="Filtrează..."
              value={searchTerm}
              onChange={e => setSearchTerm(e.target.value)}
            />
          </div>
          {showAddOption && (
            <div className="add-role-icon" onClick={onAdd} title="Adaugă">
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
                {columns.map((col, i) => (
                  <th key={i}>{col.header}</th>
                ))}
                {(showEditOption || showDeleteOption) && (
                  <th className="actions-col">Acțiuni</th>
                )}
              </tr>
            </thead>
            <tbody>
              {filteredData.map((row, i) => (
                <tr
                  key={i}
                  className={onRowClick ? "clickable-row" : ""}
                  onClick={() => onRowClick && onRowClick(row)}
                >
                  {columns.map((col, j) => (
                    <td key={j}>{row[col.accessor]}</td>
                  ))}
                  {(showEditOption || showDeleteOption) && (
                    <td className="actions-col" onClick={e => e.stopPropagation()}>
                      {showEditOption && (
                        <button
                          className="icon-btn"
                          onClick={() => onEdit && onEdit(row)}
                          title="Editează"
                        >
                          <FaEdit />
                        </button>
                      )}
                      {showDeleteOption && (
                        <button
                          className="icon-btn"
                          onClick={() => onDelete && onDelete(row)}
                          title="Șterge"
                        >
                          <FaTrash />
                        </button>
                      )}
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        )}

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
              onChange={(e) => onPageSizeChange(Number(e.target.value))}>
              {[10, 20, 50, 5000].map((sz) => (
                <option key={sz} value={sz}>
                  {sz === 5000 ? "ALL" : sz}
                </option>
              ))}
            </select>
            <span>Total: {totalCount}</span>
          </div>
        </div>
      </div>
    </div>
  );
};

export default GenericTable;
