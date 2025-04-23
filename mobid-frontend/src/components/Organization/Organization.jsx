// src/components/Organization/Organization.jsx
import React, { useEffect, useState } from "react";
import {
  getOrganizationsPaged,
  deleteOrganization,
  updateOrganization
} from "../../api/organizationApi";
import GenericTable from "../GenericTable/GenericTable";
import AddOrganizationModal from "./AddOrganizationModal";
import EditOrganizationModal from "./EditOrganizationModal";
import DeleteOrganizationModal from "./DeleteOrganizationModal";
import OrganizationMembers from "./OrganizationMembers";
import "./Organization.css";

const Organization = () => {
  const [organizations, setOrganizations] = useState([]);
  const [totalCount, setTotalCount] = useState(0);
  const [currentPage, setCurrentPage] = useState(0);
  const [limit, setLimit] = useState(5);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const [showAddModal, setShowAddModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [orgToEdit, setOrgToEdit] = useState(null);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [orgToDelete, setOrgToDelete] = useState(null);

  // Select for members table
  const [selectedOrgId, setSelectedOrgId] = useState("");
  const [selectedOrgName, setSelectedOrgName] = useState("");

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

  useEffect(() => { fetchOrganizations(); }, [currentPage, limit]);

  const handleEdit = (org) => {
    setOrgToEdit(org);
    setShowEditModal(true);
  };
  const submitEdit = async (updateData) => {
    try {
      await updateOrganization(updateData);
      setShowEditModal(false);
      fetchOrganizations();
    } catch {
      setError("Eroare la actualizarea organizației.");
    }
  };

  const handleDelete = (org) => {
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

  // When user selects org from dropdown
  const handleSelectOrg = (e) => {
    const id = e.target.value;
    setSelectedOrgId(id);
    const org = organizations.find(o => o.id === id);
    setSelectedOrgName(org ? org.name : "");
  };

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

      {
        <OrganizationMembers
          organizationId={selectedOrgId}
          organizationName={selectedOrgName}
        />
      }

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
