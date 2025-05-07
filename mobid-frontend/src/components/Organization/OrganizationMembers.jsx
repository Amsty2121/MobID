// src/components/Organization/OrganizationMembers.jsx
import React, { useEffect, useState } from "react";
import Select from "react-select";
import GenericTable from "../GenericTable/GenericTable";
import AddMemberModal from "./AddMemberModal";
import DeleteMemberModal from "./DeleteMemberModal";
import {
  getOrganizationsPaged,
  getUsersForOrganization,
  removeUserFromOrganization
} from "../../api/organizationApi";
import { FaTrash } from "react-icons/fa";
import "./Organization.css";

const OrganizationMembers = () => {
  const [orgs, setOrgs] = useState([]);
  const [members, setMembers] = useState([]);
  const [selectedOrg, setSelectedOrg] = useState(null);

  const [showAddMem, setShowAddMem] = useState(false);
  const [showDelMem, setShowDelMem] = useState(false);
  const [memToDelete, setMemToDelete] = useState(null);

  const [loadingOrgs, setLoadingOrgs] = useState(false);
  const [loadingMems, setLoadingMems] = useState(false);
  const [error, setError] = useState("");

  // load organizations once
  useEffect(() => {
    (async () => {
      setLoadingOrgs(true);
      try {
        const { items } = await getOrganizationsPaged({ pageIndex: 0, pageSize: 100 });
        setOrgs(items || []);
      } catch {
        setError("Eroare la încărcarea organizațiilor.");
      } finally {
        setLoadingOrgs(false);
      }
    })();
  }, []);

  // load members when selectedOrg changes
  useEffect(() => {
    if (!selectedOrg) return setMembers([]);
    (async () => {
      setLoadingMems(true);
      setError("");
      try {
        const data = await getUsersForOrganization(selectedOrg.value);
        setMembers(data);
      } catch {
        setError("Eroare la încărcarea membrilor.");
      } finally {
        setLoadingMems(false);
      }
    })();
  }, [selectedOrg]);

  const handleDelMem = row => {
    setMemToDelete(row);
    setShowDelMem(true);
  };
  const confirmDelMem = async () => {
    await removeUserFromOrganization(selectedOrg.value, memToDelete.userId);
    setShowDelMem(false);
    // reload
    const data = await getUsersForOrganization(selectedOrg.value);
    setMembers(data);
  };

  // build options with both name and id
  const orgOptions = orgs.map(o => ({
    value: o.id,
    name: o.name,
    id: o.id,
    label: o.name, // fallback if needed
  }));

  const memberColumns = [
    { header: "User ID",   accessor: "userId"   },
    { header: "User Name", accessor: "userName" },
    { header: "Rol",       accessor: "role"     },
    { header: "Acțiuni",   accessor: "actions"  }
  ];

  const membersWithActions = members.map(m => ({
    userId:   m.userId,
    userName: m.userName,
    role:     m.role,
    actions: (
      <button className="icon-btn" onClick={() => handleDelMem(m)} title="Exclude">
        <FaTrash />
      </button>
    )
  }));

  return (
    <div className="org-page">
      {error && <p className="error">{error}</p>}

      {/* organizații Select with Name + Id */}
      <div className="org-select-wrapper">
        <Select
          className="org-select"
          classNamePrefix="org-select"
          options={orgOptions}
          value={selectedOrg}
          onChange={setSelectedOrg}
          isLoading={loadingOrgs}
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

      {selectedOrg && (
        <>
          <h2 className="org-heading">
            Membri din „{selectedOrg.name}”
          </h2>
          <GenericTable
            title=""
            columns={memberColumns}
            filterColumns={["userName", "userId", "role"]}
            data={membersWithActions}
            onAdd={() => setShowAddMem(true)}
            showAddOption
            showDeleteOption={false}
            currentPage={0}
            totalCount={membersWithActions.length}
            pageSize={membersWithActions.length}
            onPageChange={() => {}}
            onPageSizeChange={() => {}}
          />
        </>
      )}

      {showAddMem && (
        <AddMemberModal
          organizationId={selectedOrg.value}
          onSuccess={async () => {
            const data = await getUsersForOrganization(selectedOrg.value);
            setMembers(data);
            setShowAddMem(false);
          }}
          onClose={() => setShowAddMem(false)}
        />
      )}
      {showDelMem && (
        <DeleteMemberModal
          member={memToDelete}
          onConfirm={confirmDelMem}
          onCancel={() => setShowDelMem(false)}
        />
      )}
    </div>
  );
};

export default OrganizationMembers;
