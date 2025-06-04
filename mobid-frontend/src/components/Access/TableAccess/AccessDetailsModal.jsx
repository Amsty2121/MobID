// src/components/Access/AccessDetailsModal.jsx
import React from "react";
import "../../../styles/components/modal/index.css";
import "../../../styles/components/access.css";

export default function AccessDetailsModal({ access, onClose }) {
  const rows = [
    { label: "Nume Acces",    value: access.name },
    { label: "Tip Acces",     value: access.accessType },
    {
      label: "Expirare",
      value: access.expirationDateTime
        ? new Date(access.expirationDateTime).toLocaleString()
        : "(niciuna)"
    },
    {
      label: "Restricționat",
      value: access.restrictToOrganizationMembers ? "Da" : "Nu"
    },
    { label: "Mod Scanare",   value: access.scanModeLabel || access.scanMode },
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
      ? [
          {
            label: "Durata abonament",
            value: `${access.subscriptionPeriodMonths} luni`
          }
        ]
      : []),
    ...(access.usesPerPeriod != null
      ? [{ label: "Utilizări/ perioadă", value: access.usesPerPeriod }]
      : []),
    {
      label: "Descriere",
      value: access.description || "(niciuna)"
    }
  ];

  return (
    <div className="modal__overlay" onClick={onClose}>
      <div
        className="modal__content modal__preview"
        onClick={e => e.stopPropagation()}
      >
        <button className="modal__close" onClick={onClose}>×</button>
        <h3 className="modal__title">Detalii Acces “{access.name}”</h3>
        <div className="modal__form">
          {rows.map(({ label, value }) => (
            <div key={label} className="modal__section">
              <span className="modal__section-title">{label}:</span>
              <span className="modal__message">{value}</span>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
