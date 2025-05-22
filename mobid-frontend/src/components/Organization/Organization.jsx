// src/components/Organization/Organization.jsx
import React, { useEffect, useState } from "react";
import {
  getOrganizationsPaged,
  deleteOrganization,
  updateOrganization
} from "../../api/organizationApi";
import Select from "react-select";
import GenericTable from "../GenericTable/GenericTable";
import AddOrganizationModal from "./AddOrganizationModal";
import EditOrganizationModal from "./EditOrganizationModal";
import DeleteOrganizationModal from "./DeleteOrganizationModal";
import OrganizationMembers from "./OrganizationMembers";
import "./Organization.css";

const Organization = () => {
  const [organizations, setOrganizations]     = useState([]);
  const [totalCount, setTotalCount]           = useState(0);
  const [currentPage, setCurrentPage]         = useState(0);
  const [limit, setLimit]                     = useState(10);
  const [loading, setLoading]                 = useState(false);
  const [error, setError]                     = useState("");

  const [showAddModal, setShowAddModal]       = useState(false);
  const [showEditModal, setShowEditModal]     = useState(false);
  const [orgToEdit, setOrgToEdit]             = useState(null);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [orgToDelete, setOrgToDelete]         = useState(null);

  // lifted selector state
  const [selectedOrg, setSelectedOrg]         = useState(null);

  const fetchOrganizations = async () => {
    setLoading(true);
    setError("");
    try {
      const data = await getOrganizationsPaged({ pageIndex: currentPage, pageSize: limit });
      setOrganizations(data.items || []);
      setTotalCount(data.total || 0);
    } catch {
      setError("Eroare la preluarea organizațiilor.");
    } finally {
      setLoading(false);
    }
  };

  // ❌ was useEffect(fetchOrganizations, [currentPage, limit]);
  // ✅ now wrap in a plain callback so we don't return a Promise
  useEffect(() => {
    fetchOrganizations();
  }, [currentPage, limit]);

  const handleEdit = org => {
    setOrgToEdit(org);
    setShowEditModal(true);
  };
  const submitEdit = async upd => {
    try {
      await updateOrganization(upd);
      setShowEditModal(false);
      fetchOrganizations();
    } catch {
      setError("Eroare la actualizarea organizației.");
    }
  };

  const handleDelete = org => {
    setOrgToDelete(org);
    setShowDeleteModal(true);
  };
  const confirmDelete = async () => {
    try {
      await deleteOrganization(orgToDelete.id);
      setShowDeleteModal(false);
      fetchOrganizations();
    } catch {
      setError("Eroare la ștergerea organizației.");
    }
  };

  const orgOptions = organizations.map(o => ({
    value: o.id,
    label: o.name,
    name:  o.name,
    id:    o.id
  }));

  const columns = [
    { header: "ID", accessor: "id" },
    { header: "Nume Organizație", accessor: "name" },
    { header: "Proprietar", accessor: "ownerName" }
  ];
  const filterCols = ["name", "ownerName"];

  return (
    <>
      {loading && <p>Se încarcă...</p>}
      {error   && <p className="error">{error}</p>}

      <GenericTable
        title="Organizații"
        columns={columns}
        filterColumns={filterCols}
        data={organizations}
        onAdd={() => setShowAddModal(true)}
        showAddOption
        onEdit={handleEdit}
        showEditOption
        onDelete={handleDelete}
        showDeleteOption
        currentPage={currentPage}
        totalCount={totalCount}
        pageSize={limit}
        onPageChange={setCurrentPage}
        onPageSizeChange={size => { setLimit(size); setCurrentPage(0); }}
      />

      <div className="org-select-wrapper">
        <Select
          className="org-select"
          classNamePrefix="org-select"
          options={orgOptions}
          value={selectedOrg}
          onChange={setSelectedOrg}
          isLoading={loading}
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
        <OrganizationMembers
          organizationId={selectedOrg.value}
          organizationName={selectedOrg.name}
        />
      )}

      {showAddModal && (
        <AddOrganizationModal
          onSuccess={fetchOrganizations}
          onClose={() => setShowAddModal(false)}
        />
      )}
      {showEditModal && orgToEdit && (
        <EditOrganizationModal
          organization={orgToEdit}
          onSubmit={submitEdit}
          onClose={() => setShowEditModal(false)}
        />
      )}
      {showDeleteModal && orgToDelete && (
        <DeleteOrganizationModal
          organization={orgToDelete}
          onConfirm={confirmDelete}
          onCancel={() => setShowDeleteModal(false)}
        />
      )}
    </>
  );
};

export default Organization;
