// src/components/Access/Access.jsx
import React, { useEffect, useState } from "react";
import Select from "react-select";
import GenericTable from "../GenericTable/GenericTable";
import {
  getOrganizationsPaged
} from "../../api/organizationApi";
import {
  getAccessesForOrganization,
  deactivateAccess
} from "../../api/accessApi";
import AddAccessModal from "./AddAccessModal";
import AccessQRCodeList from "./AccessQRCodeList";
import { FaTrash } from "react-icons/fa";
import "./Access.css";

export default function Access() {
  // organizații
  const [orgOptions, setOrgOptions] = useState([]);
  const [selectedOrg, setSelectedOrg] = useState(null);

  // accese
  const [accesses, setAccesses] = useState([]);
  const [selectedAccess, setSelectedAccess] = useState(null);

  // loading / error
  const [loadingOrgs, setLoadingOrgs] = useState(false);
  const [loadingAccesses, setLoadingAccesses] = useState(false);
  const [error, setError] = useState("");

  // modal
  const [showAddAccess, setShowAddAccess] = useState(false);

  // 1) Încarcă organizațiile o singură dată
  useEffect(() => {
    (async () => {
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
    })();
  }, []);

  // 2) Când se schimbă organizația, încarcă accesele ei
  useEffect(() => {
    if (!selectedOrg) {
      setAccesses([]);
      return;
    }
    (async () => {
      setLoadingAccesses(true);
      try {
        const data = await getAccessesForOrganization(selectedOrg.value);
        setAccesses(data);
      } catch {
        setError("Eroare la încărcarea acceselor.");
      } finally {
        setLoadingAccesses(false);
      }
    })();
  }, [selectedOrg]);

  // Dezactivează un acces și reîncarcă lista
  const handleDeactivateAccess = async row => {
    await deactivateAccess(row.id);
    const data = await getAccessesForOrganization(selectedOrg.value);
    setAccesses(data);
    // dacă tocmai ai dezactivat accesul selectat, resetează-l
    if (selectedAccess?.value === row.id) {
      setSelectedAccess(null);
    }
  };

  return (
    <div className="access-page">
      {error && <p className="error">{error}</p>}

      {/* Selector organizație */}
      <div className="access-select-wrapper">
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
          formatOptionLabel={({ name, id }) => (
            <div className="org-option">
              <div className="org-option-name"><strong>Name:</strong> {name}</div>
              <div className="org-option-id">Id: {id.substring(0, 8)}…</div>
            </div>
          )}
        />
      </div>

      {/* Tabel accese */}
      {selectedOrg && (
        <>
          <h2 className="access-heading">
            Accese pentru «{selectedOrg.name || selectedOrg.label}»
          </h2>
          <GenericTable
            title=""
            columns={[
              { header: "ID", accessor: "id" },
              { header: "Tip", accessor: "accessType" },
              { header: "Expiră", accessor: "expirationDateTime" },
              { header: "Activ", accessor: "isActive" },
              { header: "Acțiuni", accessor: "actions" }
            ]}
            filterColumns={["accessType"]}
            data={accesses.map(a => ({
              ...a,
              actions: (
                <button
                  className="icon-btn"
                  onClick={() => handleDeactivateAccess(a)}
                  title="Dezactivează"
                >
                  <FaTrash />
                </button>
              )
            }))}
            onAdd={() => setShowAddAccess(true)}
            showAddOption
            showEditOption={false}
            showDeleteOption={false}
            currentPage={0}
            totalCount={accesses.length}
            pageSize={accesses.length}
            onPageChange={() => {}}
            onPageSizeChange={() => {}}
          />

          {/* Selector acces */}
          {accesses.length > 0 && (
            <div className="access-select-wrapper">
              <Select
                className="org-select"
                classNamePrefix="org-select"
                options={accesses.map(a => ({
                  value: a.id,
                  label: `${a.accessType} — ${a.id.substring(0, 8)}`,
                  accessType: a.accessType,
                  id: a.id
                }))}
                value={selectedAccess}
                onChange={setSelectedAccess}
                placeholder="Selectează acces..."
                formatOptionLabel={({ accessType, id }) => (
                  <div className="org-option">
                    <div className="org-option-name"><strong>Tip:</strong> {accessType}</div>
                    <div className="org-option-id">Id: {id.substring(0, 8)}…</div>
                  </div>
                )}
              />
            </div>
          )}
        </>
      )}

      {/* Modal de adăugat acces */}
      {showAddAccess && selectedOrg && (
        <AddAccessModal
          organizationId={selectedOrg.value}
          onSuccess={async () => {
            setShowAddAccess(false);
            const data = await getAccessesForOrganization(selectedOrg.value);
            setAccesses(data);
          }}
          onClose={() => setShowAddAccess(false)}
        />
      )}

      {/* Lista codurilor QR */}
      {selectedAccess && (
        <AccessQRCodeList access={selectedAccess} />
      )}
    </div>
  );
}
