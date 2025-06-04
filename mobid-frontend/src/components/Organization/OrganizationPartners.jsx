// src/components/Organization/OrganizationPartners.jsx
import React, { useState, useEffect, useCallback } from "react";
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
  getSharedAccessesBetweenOrganizations,
  shareAccessWithOrganization,
  revokeSharedAccess
} from "../../api/accessShareApi";
import { getAllOrganizations } from "../../api/organizationApi";
import "./Organization.css";

const not = (a, b) => a.filter(x => !b.some(y => y.id === x.id));
const intersection = (a, b) => a.filter(x => b.some(y => y.id === x.id));

export default function OrganizationPartners({ organizationId, organizationName }) {
  const [allOrgs,   setAllOrgs]   = useState([]);
  const [targetOrg, setTargetOrg] = useState(null);
  const [left,      setLeft]      = useState([]); // toate accesele org curente
  const [right,     setRight]     = useState([]); // accesele deja partajate cu targetOrg
  const [checked,   setChecked]   = useState([]);

  // 1️⃣ Încarcă lista organizațiilor
  useEffect(() => {
    (async () => {
      const orgs = await getAllOrganizations();
      setAllOrgs(
        orgs
          .filter(o => o.id !== organizationId)
          .map(o => ({ value: o.id, label: o.name, id: o.id, name: o.name }))
      );
    })();
  }, [organizationId]);

  // 2️⃣ Funcție de (re)încărcare left + right
  const refreshData = useCallback(async () => {
    // left = toate accesele organizației curente
    const accesses = await getAccessesForOrganization(organizationId);
    setLeft(accesses);

    // right = share‐urile dintre cele două org
    if (targetOrg) {
      const shares = await getSharedAccessesBetweenOrganizations(
        organizationId,
        targetOrg.value
      );
      const sharedIds = shares.map(s => s.accessId);
      setRight(accesses.filter(a => sharedIds.includes(a.id)));
    } else {
      setRight([]);
    }

    // resetează selecțiile
    setChecked([]);
  }, [organizationId, targetOrg]);

  // 3️⃣ Rulează la schimbarea organizației curente sau a targetOrg
  useEffect(() => {
    refreshData();
  }, [refreshData]);

  const leftOnly  = not(left, right);
  const leftChecked  = intersection(checked, leftOnly);
  const rightChecked = intersection(checked, right);

  const handleToggle = item => {
    setChecked(prev =>
      prev.some(x => x.id === item.id)
        ? prev.filter(x => x.id !== item.id)
        : [...prev, item]
    );
  };

  const handleShare = async items => {
    if (!targetOrg) return;
    for (const acc of items) {
      await shareAccessWithOrganization(acc.id, organizationId, targetOrg.value);
    }
    await refreshData();
  };

  const handleRevoke = async items => {
    for (const acc of items) {
      await revokeSharedAccess(acc.id, organizationId, targetOrg.value);
    }
    await refreshData();
  };

  const customList = (title, items) => {
    const allSelected     = items.length > 0 && intersection(checked, items).length === items.length;
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
          {items.map(item => (
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
                  inputProps={{ "aria-labelledby": `transfer-item-${item.id}` }}
                />
              </ListItemIcon>
              <ListItemText id={`transfer-item-${item.id}`} primary={item.name} />
            </ListItem>
          ))}
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
          {customList("Disponibile", leftOnly)}
          <Box className="org-transfer-actions">
            <Button
              variant="outlined"
              size="small"
              onClick={() => handleShare(leftOnly)}
              disabled={!leftOnly.length}
            >
              &gt;&gt;
            </Button>
            <Button
              variant="outlined"
              size="small"
              onClick={() => handleShare(leftChecked)}
              disabled={!leftChecked.length}
            >
              &gt;
            </Button>
            <Button
              variant="outlined"
              size="small"
              onClick={() => handleRevoke(rightChecked)}
              disabled={!rightChecked.length}
            >
              &lt;
            </Button>
            <Button
              variant="outlined"
              size="small"
              onClick={() => handleRevoke(right)}
              disabled={!right.length}
            >
              &lt;&lt;
            </Button>
          </Box>
          {customList("Partajate", right)}
        </Box>
      )}
    </Box>
  );
}
