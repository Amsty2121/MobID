// src/components/Organization/Organization.jsx
import React, { useState, useCallback } from "react";
import OrganizationTable from "./TableOrganization/OrganizationTable";
import OrganizationMemberTable from "./TableOrganizationMembers/OrganizationMemberTable";
import OrganizationPartners from "./OrganizationPartners";
import AccessTable from "../Access/TableAccess/AccessTable";
import { TabContext, TabList, TabPanel } from "@mui/lab";
import { Tab } from "@mui/material";
import "./Organization.css";

export default function Organization() {
  const [selectedOrg, setSelectedOrg] = useState(null);
  const [tabValue, setTabValue]       = useState("members");

  const handleSelectOrg = useCallback(org => {
    setSelectedOrg(org);
    setTabValue("members");
  }, []);

  const handleTabChange = (_event, newValue) => {
    setTabValue(newValue);
  };

  return (
    <>
      <OrganizationTable onSelect={handleSelectOrg} />

      {selectedOrg && (
        <TabContext value={tabValue}>
          <div className="org-tabs-wrapper">
            <TabList onChange={handleTabChange} aria-label="Organization tabs">
              <Tab label="Membri" value="members" />
              <Tab label="Accese" value="accese" />
              <Tab label="Parteneriat" value="parteneriat" />
            </TabList>
          </div>

          <div className="org-tabs-content">
            <TabPanel value="members" sx={{ p: 0, pt: 2 }}>
              <OrganizationMemberTable
                key={`members-${selectedOrg.id}`}
                organizationId={selectedOrg.id}
                organizationName={selectedOrg.name}
              />
            </TabPanel>

            <TabPanel value="accese" sx={{ p: 0, pt: 2 }}>
              <AccessTable
                key={`access-${selectedOrg.id}`}
                organizationId={selectedOrg.id}
                organizationName={selectedOrg.name}
              />
            </TabPanel>

            <TabPanel value="parteneriat" sx={{ p: 0, pt: 2 }}>
              <OrganizationPartners
                key={`partners-${selectedOrg.id}`}
                organizationId={selectedOrg.id}
                organizationName={selectedOrg.name}
              />
            </TabPanel>
          </div>
        </TabContext>
      )}
    </>
  );
}
