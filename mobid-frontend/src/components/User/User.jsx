// src/components/User/User.jsx
import React, { useState, useCallback } from "react";
import UserTable from "./TableUser/UserTable";
import UserAccessTable from "./TableUserAccess/UserAccessTable";
import UserOrganizationTable from "./TableUserOrganization/UserOrganizationTable";
import { TabContext, TabList, TabPanel } from "@mui/lab";
import { Tab, Box } from "@mui/material";
import "../../styles/components/user.css";
import ScanTable from "../Scan/TableScan/ScanTable";

export default function User() {
  const [selectedUser, setSelectedUser] = useState(null);
  const [tabValue, setTabValue] = useState("accesses");

  const handleSelectUser = useCallback(user => {
    setSelectedUser(user);
    setTabValue("accesses"); // resetează tab-ul la fiecare select
  }, []);

  const handleTabChange = (_e, newValue) => {
    setTabValue(newValue);
  };

  return (
    <div className="user-page">
      <UserTable onSelect={handleSelectUser} />

      {selectedUser && (
        <TabContext value={tabValue}>
          <div className="user-tabs-wrapper">
            <TabList onChange={handleTabChange} aria-label="User tabs">
              <Tab label="Accesses" value="accesses" />
              <Tab label="Organizations" value="organizations" />
              <Tab label="Scans" value="scans" />
            </TabList>
          </div>

          <Box className="user-tabs-content">
            <TabPanel value="accesses" sx={{ p: 0, pt: 2 }}>
              <UserAccessTable
                key={`accesses-${selectedUser.id}`}
                userId={selectedUser.id}
                userName={selectedUser.username}
              />
            </TabPanel>

            <TabPanel value="organizations" sx={{ p: 0, pt: 2 }}>
              <UserOrganizationTable
                key={`organizations-${selectedUser.id}`}
                userId={selectedUser.id}
                userName={selectedUser.username}
              />
            </TabPanel>

            <TabPanel value="scans" sx={{ p: 0, pt: 2 }}>
              <ScanTable />
            </TabPanel>
          </Box>
        </TabContext>
      )}
    </div>
  );
}
