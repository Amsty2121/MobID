// src/components/Access/Access.jsx
import React, { useEffect, useState } from "react";
import Select from "react-select";
import { FaTrash } from "react-icons/fa";
import GenericTable from "../GenericTable/GenericTable";
import AddAccessModal from "./AddAccessModal";
import AccessDetailsModal from "./AccessDetailsModal";
import AccessQRCodes from "./AccessQRCodes";
import { getOrganizationsPaged } from "../../api/organizationApi";
import { getAccessesForOrganization, deactivateAccess } from "../../api/accessApi";
import "./Access.css";

export default function Access() {
  const [orgOptions, setOrgOptions] = useState([]);
  const [selectedOrg, setSelectedOrg] = useState(null);
  const [accesses, setAccesses] = useState([]);
  const [selectedAccess, setSelectedAccess] = useState(null);

  const [loadingOrgs, setLoadingOrgs] = useState(false);
  const [loadingAccesses, setLoadingAccesses] = useState(false);
  const [error, setError] = useState("");

  const [showAddAccess, setShowAddAccess] = useState(false);
  const [showDetails, setShowDetails] = useState(false);
  const [accessToView, setAccessToView] = useState(null);

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

  // 2️⃣ Când selectăm o organizație, încarcă accesele
  useEffect(() => {
    if (!selectedOrg) {
      setAccesses([]);
      setSelectedAccess(null);
      return;
    }
    const fetchAccesses = async () => {
      setLoadingAccesses(true);
      try {
        const data = await getAccessesForOrganization(selectedOrg.value);
        setAccesses(data);
      } catch {
        setError("Eroare la încărcarea acceselor.");
      } finally {
        setLoadingAccesses(false);
      }
    };
    fetchAccesses();
  }, [selectedOrg]);

  // Dezactivează un acces
  const handleDeactivate = async row => {
    await deactivateAccess(row.id);
    const fresh = await getAccessesForOrganization(selectedOrg.value);
    setAccesses(fresh);
    // dacă era deschisă detalierea pentru acest acces, o închidem
    if (accessToView?.id === row.id) {
      setAccessToView(null);
      setShowDetails(false);
    }
  };

  const columns = [
    { header: "ID", accessor: "id" },
    { header: "Nume", accessor: "name" },
    { header: "Tip", accessor: "accessType" },
    { header: "Activ", accessor: "isActive" },
    { header: "Expiră", accessor: "expirationDateTime" },
  ];

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
            setAccessToView(null);
            setShowDetails(false);
          }}
          placeholder="Selectează organizație..."
          noOptionsMessage={() => "Nu s-au găsit organizații"}
          formatOptionLabel={({ name, id }) => (
            <div className="org-option">
              <div className="org-option-name"><strong>Name:</strong> {name}</div>
              <div className="org-option-id">Id: {id}</div>
            </div>
          )}
        />
      </div>

      {/* Tabel Accese */}
      {selectedOrg && (
        <>
          <h2 className="access-heading">
            Accese pentru „{selectedOrg.name}”
          </h2>
          {loadingAccesses
            ? <p>Se încarcă accesele...</p>
            : (
              <GenericTable
                title=""
                columns={columns}
                filterColumns={["name", "accessType"]}
                data={accesses}
                onAdd={() => setShowAddAccess(true)}
                showAddOption
                showEditOption={false}
                showDeleteOption
                onDelete={handleDeactivate}
                onRowClick={row => {
                  setAccessToView(row);
                  setShowDetails(true);
                }}
                currentPage={0}
                totalCount={accesses.length}
                pageSize={accesses.length}
                onPageChange={() => {}}
                onPageSizeChange={() => {}}
              />
            )
          }
        </>
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

      {/* Modal Adaugă Acces */}
      {selectedOrg && showAddAccess && (
        <AddAccessModal
          organizationId={selectedOrg.value}
          onSuccess={async () => {
            setShowAddAccess(false);
            const fresh = await getAccessesForOrganization(selectedOrg.value);
            setAccesses(fresh);
          }}
          onClose={() => setShowAddAccess(false)}
        />
      )}

      {/* Modal Detalii Acces */}
      {showDetails && accessToView && (
        <AccessDetailsModal
          access={accessToView}
          onClose={() => setShowDetails(false)}
        />
      )}

      {/* Componentă QR-Codes */}
      {selectedAccess && (
        <AccessQRCodes access={selectedAccess} />
      )}
    </div>
  );
}
