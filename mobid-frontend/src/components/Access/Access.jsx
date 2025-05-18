import React, { useEffect, useState } from "react";
import Select from "react-select";
import { FaTrash } from "react-icons/fa";
import GenericTable from "../GenericTable/GenericTable";
import AddAccessModal from "./AddAccessModal";
import AccessDetailsModal from "./AccessDetailsModal";
import { getOrganizationsPaged } from "../../api/organizationApi";
import { getAccessesForOrganization, deactivateAccess } from "../../api/accessApi";
import "./Access.css";

export default function Access() {
  const [orgOptions, setOrgOptions] = useState([]);
  const [selectedOrg, setSelectedOrg] = useState(null);
  const [accesses, setAccesses] = useState([]);
  const [loadingOrgs, setLoadingOrgs] = useState(false);
  const [loadingAccesses, setLoadingAccesses] = useState(false);
  const [error, setError] = useState("");
  const [showAddAccess, setShowAddAccess] = useState(false);
  const [showDetails, setShowDetails] = useState(false);
  const [accessToView, setAccessToView] = useState(null);

  useEffect(() => {
    (async () => {
      setLoadingOrgs(true);
      try {
        const { items } = await getOrganizationsPaged({ pageIndex: 0, pageSize: 1000 });
        setOrgOptions(items.map(o => ({ value: o.id, label: o.name, name: o.name, id: o.id })));
      } catch {
        setError("Eroare la încărcarea organizațiilor.");
      } finally {
        setLoadingOrgs(false);
      }
    })();
  }, []);

  useEffect(() => {
    if (!selectedOrg) return setAccesses([]);
    (async () => {
      setLoadingAccesses(true);
      try {
        setAccesses(await getAccessesForOrganization(selectedOrg.value));
      } catch {
        setError("Eroare la încărcarea acceselor.");
      } finally {
        setLoadingAccesses(false);
      }
    })();
  }, [selectedOrg]);

  const handleDeactivate = async row => {
    await deactivateAccess(row.id);
    setAccesses(await getAccessesForOrganization(selectedOrg.value));
  };

  const handleRowClick = row => {
    setAccessToView(row);
    setShowDetails(true);
  };

  const columns = [
    { header: "ID", accessor: "id" },
    { header: "Tip", accessor: "accessType" },
    { header: "Activ", accessor: "isActive" },
    { header: "Expiră", accessor: "expirationDateTime" },
  ];

  return (
    <div className="access-page">
      {error && <p className="error">{error}</p>}

      <div className="org-select-wrapper">
        <Select
          className="org-select"
          classNamePrefix="org-select"
          options={orgOptions}
          isLoading={loadingOrgs}
          value={selectedOrg}
          onChange={setSelectedOrg}
          placeholder="Selectează organizație..."
          formatOptionLabel={({ name, id }) => (
            <div className="org-option">
              <div className="org-option-name"><strong>Name:</strong> {name}</div>
              <div className="org-option-id">Id: {id}</div>
            </div>
          )}
        />
      </div>

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
                filterColumns={["accessType"]}
                data={accesses}
                onAdd={() => setShowAddAccess(true)}
                showAddOption
                showEditOption={false}
                showDeleteOption
                onDelete={handleDeactivate}
                onRowClick={handleRowClick}
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

      {showAddAccess && (
        <AddAccessModal
          organizationId={selectedOrg.value}
          onSuccess={async () => {
            setShowAddAccess(false);
            setAccesses(await getAccessesForOrganization(selectedOrg.value));
          }}
          onClose={() => setShowAddAccess(false)}
        />
      )}

      {showDetails && (
        <AccessDetailsModal
          access={accessToView}
          onClose={() => setShowDetails(false)}
        />
      )}
    </div>
  );
}
