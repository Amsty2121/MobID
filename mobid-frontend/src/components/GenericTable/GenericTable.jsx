// src/components/GenericTable/GenericTable.jsx
import React, { useMemo, useState } from "react";
import { FaPlus, FaTrash, FaEdit } from "react-icons/fa";
import "../../styles/components/generic-table.css";

export default function GenericTable({
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
  onRowClick,
  currentPage,
  totalCount,
  pageSize,
  onPageChange,
  onPageSizeChange,
}) {
  const [searchTerm, setSearchTerm] = useState("");
  const filteredData = useMemo(() => {
    const lower = searchTerm.toLowerCase();
    return (data || []).filter(row =>
      filterColumns.some(col =>
        row[col]?.toString().toLowerCase().includes(lower)
      )
    );
  }, [data, searchTerm, filterColumns]);

  const totalPages = Math.ceil(totalCount / pageSize);

  return (
    <div className="generic-table">
      <h2 className="generic-table__title">{title}</h2>
      <div className="generic-table__container">
        <div className="generic-table__filter-row">
          <div className="generic-table__filter">
            <input
              type="text"
              placeholder="Filtrează..."
              value={searchTerm}
              onChange={e => setSearchTerm(e.target.value)}
            />
          </div>
          {showAddOption && (
            <div
              className="generic-table__add-icon"
              onClick={onAdd}
              title="Adaugă"
            >
              <FaPlus />
            </div>
          )}
        </div>

        {filteredData.length === 0 ? (
          <p className="generic-table__message">Nu există date.</p>
        ) : (
          <table className="generic-table__table">
            <thead>
              <tr>
                {columns.map((col, i) => <th key={i}>{col.header}</th>)}
                {(showEditOption || showDeleteOption) && (
                  <th className="generic-table__actions-col">Acțiuni</th>
                )}
              </tr>
            </thead>
            <tbody>
              {filteredData.map((row, i) => (
                <tr
                  key={i}
                  className={onRowClick ? "generic-table__row--clickable" : ""}
                  onClick={() => onRowClick?.(row)}
                >
                  {columns.map((col, j) => (
                    <td key={j}>{row[col.accessor]}</td>
                  ))}
                  {(showEditOption || showDeleteOption) && (
                    <td
                      className="generic-table__actions-col"
                      onClick={e => e.stopPropagation()}
                    >
                      {showEditOption && (
                        <button
                          className="generic-table__icon-btn"
                          onClick={() => onEdit(row)}
                          title="Editează"
                        >
                          <FaEdit />
                        </button>
                      )}
                      {showDeleteOption && (
                        <button
                          className="generic-table__icon-btn"
                          onClick={() => onDelete(row)}
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

        <div className="generic-table__pagination">
          <div className="generic-table__pagination-left">
            <button onClick={() => onPageChange(currentPage - 1)}
                    disabled={currentPage === 0}>
              Anterior
            </button>
            <span>
              Pagina {currentPage + 1} din {totalPages || 1}
            </span>
            <button onClick={() => onPageChange(currentPage + 1)}
                    disabled={currentPage + 1 >= totalPages}>
              Următoare
            </button>
          </div>
          <div className="generic-table__pagination-right">
            <label htmlFor="pageSizeSelect">Pe pagină:</label>
            <select
              id="pageSizeSelect"
              value={pageSize}
              onChange={e => onPageSizeChange(Number(e.target.value))}
            >
              {[10,20,50,5000].map(sz => (
                <option key={sz} value={sz}>{sz===5000 ? "ALL": sz}</option>
              ))}
            </select>
            <span>Total: {totalCount}</span>
          </div>
        </div>
      </div>
    </div>
  );
}
