// src/components/Access/AccessDetailsModal.jsx
import React from "react";
import "../Access.css";

export default function AccessDetailsModal({ access, onClose }) {
  const rows = [
    { label: "Nume Acces",            value: access.name },
    { label: "Tip Acces",             value: access.accessType },
    { label: "Expirare",              value: access.expirationDateTime
        ? new Date(access.expirationDateTime).toLocaleString()
        : "(niciuna)" },
    { label: "Restricționat",         value: access.restrictToOrganizationMembers ? "Da" : "Nu" },
    { label: "Mod Scanare",           value: access.scanModeLabel || access.scanMode },
    ...(access.maxUsersPerPass != null
      ? [{ label: "Max utilizatori/scanare", value: access.maxUsersPerPass }]
      : []),
    ...(access.maxUses != null
      ? [{ label: "Max utilizări totale",     value: access.maxUses }]
      : []),
    ...(access.monthlyLimit != null
      ? [{ label: "Limită lunară",           value: access.monthlyLimit }]
      : []),
    ...(access.subscriptionPeriodMonths != null
      ? [{ label: "Durata abonament",         value: `${access.subscriptionPeriodMonths} luni` }]
      : []),
    ...(access.usesPerPeriod != null
      ? [{ label: "Utilizări/ perioadă",     value: access.usesPerPeriod }]
      : []),
    { label: "Descriere",              value: access.description || "(niciuna)" },
  ];

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content preview-modal" onClick={e => e.stopPropagation()}>
        <div className="preview-header">
          Detalii Acces “{access.name}”
        </div>
        <div className="preview-body">
          {rows.map(({ label, value }) => (
            <div key={label} className="preview-row">
              <span className="preview-label">{label}:</span>
              <span className="preview-value">{value}</span>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
