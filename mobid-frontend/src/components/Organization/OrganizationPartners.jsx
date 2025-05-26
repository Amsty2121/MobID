// src/components/Organization/OrganizationPartners.jsx
import React, { useState, useEffect } from "react";
import Select from "react-select";
import {
  Box,
  Button,
  Checkbox,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  Paper,
  Typography
} from "@mui/material";
import { getAccessesForOrganization } from "../../api/accessApi";
import {
  getSharesForOrganization,
  shareAccessWithOrganization,
  revokeSharedAccess
} from "../../api/accessShareApi";
import { getAllOrganizations } from "../../api/organizationApi";
import "./Organization.css";

const not = (a, b) => a.filter(x => !b.some(y => y.id === x.id));
const intersection = (a, b) => a.filter(x => b.some(y => y.id === x.id));

export default function OrganizationPartners({ organizationId, organizationName }) {
  const [allOrgs, setAllOrgs] = useState([]);
  const [targetOrg, setTargetOrg] = useState(null);
  const [left, setLeft] = useState([]);
  const [right, setRight] = useState([]);
  const [checked, setChecked] = useState([]);

  useEffect(() => {
    (async () => {
      const accesses = await getAccessesForOrganization(organizationId);
      setLeft(accesses);
    })();
  }, [organizationId]);

  useEffect(() => {
    (async () => {
      const orgs = await getAllOrganizations();
      setAllOrgs(
        orgs
          .filter(o => o.id !== organizationId)
          .map(o => ({
            value: o.id,
            label: o.name,
            name: o.name,
            id: o.id
          }))
      );
    })();
  }, [organizationId]);

  useEffect(() => {
    if (!targetOrg) {
      setRight([]);
      return;
    }
    (async () => {
      const shares = await getSharesForOrganization(targetOrg.value);
      const sharedIds = shares.map(s => s.accessId);
      setRight(left.filter(a => sharedIds.includes(a.id)));
    })();
  }, [targetOrg, left]);

  const leftChecked = intersection(checked, not(left, right));
  const rightChecked = intersection(checked, right);

  const handleToggle = item => {
    setChecked(prev =>
      prev.some(x => x.id === item.id)
        ? prev.filter(x => x.id !== item.id)
        : [...prev, item]
    );
  };

  const refreshRight = async () => {
    const shares = await getSharesForOrganization(targetOrg.value);
    const sharedIds = shares.map(s => s.accessId);
    setRight(left.filter(a => sharedIds.includes(a.id)));
    setChecked([]);
  };

  const handleAllRight = async () => {
    if (!targetOrg) return;
    for (const acc of not(left, right)) {
      await shareAccessWithOrganization(acc.id, organizationId, targetOrg.value);
    }
    await refreshRight();
  };

  const handleCheckedRight = async () => {
    if (!targetOrg) return;
    for (const acc of leftChecked) {
      await shareAccessWithOrganization(acc.id, organizationId, targetOrg.value);
    }
    await refreshRight();
  };

  const handleCheckedLeft = async () => {
    for (const acc of rightChecked) {
      await revokeSharedAccess(acc.id, organizationId, targetOrg.value);
    }
    await refreshRight();
  };

  const handleRevokeAll = async () => {
    for (const acc of right) {
      await revokeSharedAccess(acc.id, organizationId, targetOrg.value);
    }
    await refreshRight();
  };

  const handleShareAllAccesses = async () => {
    if (!targetOrg) return;
    for (const acc of left) {
      await shareAccessWithOrganization(acc.id, organizationId, targetOrg.value);
    }
    await refreshRight();
  };

  const handleTakeAllAccesses = async () => {
    if (!targetOrg) return;
    const theirAccesses = await getAccessesForOrganization(targetOrg.value);
    for (const acc of theirAccesses) {
      await shareAccessWithOrganization(acc.id, targetOrg.value, organizationId);
    }
    await refreshRight();
  };

  const customList = (title, items) => {
    const allSelected = items.length > 0 && intersection(checked, items).length === items.length;
    const partialSelected = intersection(checked, items).length > 0 && !allSelected;

    const handleToggleAll = () => {
      if (allSelected) {
        setChecked(checked.filter(c => !items.includes(c)));
      } else {
        setChecked([...checked, ...not(items, checked)]);
      }
    };

    return (
      <Paper className="org-transfer-list">
        <Box className="org-transfer-header" sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", px: 1 }}>
          <Checkbox
            checked={allSelected}
            indeterminate={partialSelected}
            onChange={handleToggleAll}
            sx={{ color: "var(--color-primary)" }}
          />
          <Box sx={{ flexGrow: 1, textAlign: "left" }}>
            {title}
            <div className="org-transfer-count">
              {intersection(checked, items).length}/{items.length} selectate
            </div>
          </Box>
        </Box>
        <List dense component="div" role="list">
          {items.map(item => {
            const labelId = `transfer-item-${item.id}`;
            return (
              <ListItem
                key={item.id}
                role="listitem"
                component="li"
                onClick={() => handleToggle(item)}
                sx={{ cursor: "pointer" }}
              >
                <ListItemIcon>
                  <Checkbox
                    checked={checked.some(x => x.id === item.id)}
                    tabIndex={-1}
                    disableRipple
                    inputProps={{ "aria-labelledby": labelId }}
                  />
                </ListItemIcon>
                <ListItemText id={labelId} primary={item.name} />
              </ListItem>
            );
          })}
        </List>
      </Paper>
    );
  };

  return (
    <Box className="org-partners-container" sx={{ mb: 4, textAlign: "center", p: 1 }}>
      <Typography variant="h6" gutterBottom className="org-partners-title">
        <strong>Parteneriat pentru „{organizationName}”</strong>
      </Typography>

      <div className="org-select-wrapper">
        <Select
          className="org-select"
          classNamePrefix="org-select"
          options={allOrgs}
          value={targetOrg}
          onChange={setTargetOrg}
          placeholder="Selectează organizație…"
          formatOptionLabel={({ name, id }) => (
            <div className="org-option">
              <div className="org-option-name"><strong>Name:</strong> {name}</div>
              <div className="org-option-id"><strong>Id:</strong> {id}</div>
            </div>
          )}
        />
      </div>

      {targetOrg && (
        <Box className="org-transfer-container">
          {customList("Disponibile", not(left, right))}
          <Box className="org-transfer-actions">
            <Button variant="outlined" size="small" onClick={handleAllRight} disabled={!not(left, right).length}>
              &gt;&gt;
            </Button>
            <Button variant="outlined" size="small" onClick={handleCheckedRight} disabled={!leftChecked.length}>
              &gt;
            </Button>
            <Button variant="outlined" size="small" onClick={handleCheckedLeft} disabled={!rightChecked.length}>
              &lt;
            </Button>
            <Button variant="outlined" size="small" onClick={handleRevokeAll} disabled={!right.length}>
              &lt;&lt;
            </Button>
          </Box>
          {customList("Partajate", right)}
        </Box>
      )}
    </Box>
  );
}
