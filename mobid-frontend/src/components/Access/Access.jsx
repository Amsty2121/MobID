// src/components/Access/Access.jsx
import React, { useEffect, useState } from "react";
import Select from "react-select";
import AccessQRCodes from "./AccessQRCodes";
import { getOrganizationsPaged } from "../../api/organizationApi";
import { getAccessesForOrganization } from "../../api/accessApi";
import AccessTable from "./TableAccess/AccessTable";
import "./Access.css";

export default function Access() {
  const [orgOptions, setOrgOptions] = useState([]);
  const [selectedOrg, setSelectedOrg] = useState(null);
  const [accesses, setAccesses] = useState([]);
  const [selectedAccess, setSelectedAccess] = useState(null);

  const [loadingOrgs, setLoadingOrgs] = useState(false);
  const [error, setError] = useState("");

  // 1️⃣ Încarcă organizațiile
  useEffect(() => {
    const fetchOrgs = async () => {
      setLoadingOrgs(true);
      try {
        const { items } = await getOrganizationsPaged({ pageIndex: 0, pageSize: 1000 });
        setOrgOptions(
          (items || []).map(o => ({
            value: o.id,
            label: o.name,
            name: o.name,
            id: o.id
          }))
        );
      } catch {
        setError("Eroare la încărcarea organizațiilor.");
      } finally {
        setLoadingOrgs(false);
      }
    };
    fetchOrgs();
  }, []);

  // 2️⃣ Când selectăm o organizație, încarcă accesele pentru QR
  useEffect(() => {
    if (!selectedOrg) {
      setAccesses([]);
      setSelectedAccess(null);
      return;
    }
    const fetchAccesses = async () => {
      try {
        const data = await getAccessesForOrganization(selectedOrg.value);
        setAccesses(data);
      } catch {
        setError("Eroare la încărcarea acceselor.");
      }
    };
    fetchAccesses();
  }, [selectedOrg]);

  return (
    <div className="access-page">
      {error && <p className="error">{error}</p>}

      {/* Select Organizație */}
      <div className="org-select-wrapper">
        <Select
          className="org-select"
          classNamePrefix="org-select"
          options={orgOptions}
          isLoading={loadingOrgs}
          value={selectedOrg}
          onChange={opt => {
            setSelectedOrg(opt);
            setSelectedAccess(null);
          }}
          placeholder="Selectează organizație..."
          noOptionsMessage={() => "Nu s-au găsit organizații"}
          formatOptionLabel={({ name, id }) => (
            <div className="org-option">
              <div className="org-option-name"><strong>Name:</strong> {name}</div>
              <div className="org-option-id"><strong>Id:</strong> {id}</div>
            </div>
          )}
        />
      </div>

      {/* Tabel Accese */}
      {selectedOrg && (
        <AccessTable
          organizationId={selectedOrg.value}
          organizationName={selectedOrg.name}
        />
      )}

      {/* Select + tabel QR-Codes */}
      {accesses.length > 0 && (
        <div className="org-select-wrapper" style={{ marginTop: "2rem" }}>
          <Select
            className="org-select"
            classNamePrefix="org-select"
            options={accesses.map(a => ({
              value: a.id,
              label: `${a.name} — ${a.id.substring(0, 8)}`,
              id: a.id,
              name: a.name
            }))}
            value={selectedAccess}
            onChange={setSelectedAccess}
            placeholder="Selectează acces..."
            noOptionsMessage={() => "Niciun acces"}
            formatOptionLabel={({ name, id }) => (
              <div className="org-option">
                <div className="org-option-name"><strong>Name:</strong> {name}</div>
                <div className="org-option-id">Id: {id.substring(0,8)}…</div>
              </div>
            )}
          />
        </div>
      )}

      {/* Componentă QR-Codes */}
      {selectedAccess && (
        <AccessQRCodes access={selectedAccess} />
      )}
    </div>
  );
}
