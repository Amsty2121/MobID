// src/components/Organization/Organization.jsx
import React, { useEffect, useState } from "react";
import { getOrganizationsPaged, createOrganization, updateOrganization, deleteOrganization } from "../../api/organizationApi";
import GenericTable from "../GenericTable/GenericTable";
import AddOrganizationModal from "./AddOrganizationModal";
import EditOrganizationModal from "./EditOrganizationModal";
import DeleteOrganizationModal from "./DeleteOrganizationModal";
import "./Organization.css";

const Organization = () => {
  const [organizations, setOrganizations] = useState([]);
  const [totalCount, setTotalCount] = useState(0);
  const [currentPage, setCurrentPage] = useState(0); // PageIndex
  const [limit, setLimit] = useState(5);             // PageSize
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  // Modal de adăugare organizație
  const [showAddModal, setShowAddModal] = useState(false);

  // Modal de editare organizație
  const [showEditModal, setShowEditModal] = useState(false);
  const [orgToEdit, setOrgToEdit] = useState(null);

  // Modal de ștergere organizație
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [orgToDelete, setOrgToDelete] = useState(null);

  const fetchOrganizations = async () => {
    setLoading(true);
    setError("");
    try {
      const data = await getOrganizationsPaged({ pageIndex: currentPage, pageSize: limit });
      setOrganizations(data.items || []);
      setTotalCount(data.total || 0);
    } catch (err) {
      console.error(err);
      setError("Eroare la preluarea organizațiilor.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchOrganizations();
  }, [currentPage, limit]);

  const handleAdd = () => {
    setShowAddModal(true);
  };

  const handleEdit = (org) => {
    setOrgToEdit(org);
    setShowEditModal(true);
  };

  const handleDelete = (org) => {
    setOrgToDelete(org);
    setShowDeleteModal(true);
  };

  const submitEdit = async (updateData) => {
    try {
      await updateOrganization(updateData);
      setShowEditModal(false);
      setOrgToEdit(null);
      fetchOrganizations();
    } catch (err) {
      console.error(err);
      setError("Eroare la actualizarea organizației.");
    }
  };

  const confirmDelete = async () => {
    try {
      await deleteOrganization(orgToDelete.id);
      setShowDeleteModal(false);
      setOrgToDelete(null);
      fetchOrganizations();
    } catch (err) {
      console.error(err);
      setError("Eroare la ștergerea organizației.");
    }
  };

  const cancelDelete = () => {
    setShowDeleteModal(false);
    setOrgToDelete(null);
  };

  const columns = [
    { header: "ID", accessor: "id" },
    { header: "Nume Organizație", accessor: "name" },
    { header: "Proprietar", accessor: "ownerName" },
  ];

  const filterColumns = ["name", "ownerName"];

  return (
    <>
      {loading && <p>Se încarcă...</p>}
      {error && <p className="error">{error}</p>}
      <GenericTable
        title="Organizații"
        columns={columns}
        filterColumns={filterColumns}
        data={organizations}
        onAdd={handleAdd}
        showAddOption={true}
        onEdit={handleEdit}
        showEditOption={true}
        onDelete={handleDelete}
        showDeleteOption={true}
        currentPage={currentPage}
        totalCount={totalCount}
        pageSize={limit}
        onPageChange={(newPage) => setCurrentPage(newPage)}
        onPageSizeChange={(newSize) => {
          setLimit(newSize);
          setCurrentPage(0);
        }}
      />

      {showAddModal && (
        <AddOrganizationModal
          onClose={() => setShowAddModal(false)}
          handleAddOrg={async (e) => {
            e.preventDefault();
            try {
              // Presupunem că AddOrganizationModal va trimite datele sub forma:
              // { name, ownerId }
              // Iar createOrganization API le va primi corect
              const formData = new FormData(e.target);
              const name = formData.get("orgName");
              const ownerId = formData.get("orgOwner");
              await createOrganization({ name, ownerId });
              setShowAddModal(false);
              fetchOrganizations();
            } catch (err) {
              console.error(err);
              setError("Eroare la adăugarea organizației.");
            }
          }}
        />
      )}

      {showEditModal && orgToEdit && (
        <EditOrganizationModal
          organization={orgToEdit}
          onSubmit={submitEdit}
          onClose={() => {
            setShowEditModal(false);
            setOrgToEdit(null);
          }}
        />
      )}

      {showDeleteModal && orgToDelete && (
        <DeleteOrganizationModal
          organization={orgToDelete}
          onConfirm={confirmDelete}
          onCancel={cancelDelete}
        />
      )}
    </>
  );
};

export default Organization;
