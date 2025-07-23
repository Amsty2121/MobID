// src/components/Access/Access.jsx
import React, { useEffect, useState } from "react";
import AccessQRCodes from "./AccessQRCodes";
import AccessTable from "./TableAccess/AccessTable";
import "./Access.css";

export default function Access({ organizationId, organizationName }) {
  const [selectedAccess, setSelectedAccess] = useState(null);

  return (
    <div className="access-page">
      <AccessTable
        showAddOption
        showEditOption
        showDeleteOption
        onRowClick={row => setSelectedAccess(row)}
      />

      {selectedAccess && (
        <div style={{ marginTop: "2rem" }}>
          <AccessQRCodes access={selectedAccess} />
        </div>
      )}
    </div>
  );
}
