// src/components/Organization/OrganizationPartners.jsx
import React, { useState, useEffect, useRef } from "react";
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
  getSharedAccessesBetweenOrgs,
  shareAccessWithOrganization,
  revokeSharedAccess
} from "../../api/accessShareApi";
import { getAllOrganizations } from "../../api/organizationApi";
import "./Organization.css";

const not = (a, b) => a.filter(x => !b.some(y => y.id === x.id));
const intersection = (a, b) => a.filter(x => b.some(y => y.id === x.id));

export default function OrganizationPartners({
  organizationId,
  organizationName
}) {
  // ─── ORG-uri ─────────────────────────────────────────────
  const [allOrgs, setAllOrgs] = useState([]);
  const didFetchOrgs = useRef(false);

  useEffect(() => {
    if (didFetchOrgs.current) return;
    didFetchOrgs.current = true;

    (async () => {
      try {
        const orgs = await getAllOrganizations();
        setAllOrgs(
          orgs
            .filter(o => o.id !== organizationId)
            .map(o => ({
              value: o.id,
              label: o.name,
              id: o.id,
              name: o.name
            }))
        );
      } catch {
        // poți trata eroarea aici dacă vrei
      }
    })();
  }, [organizationId]);

  // ─── ACCESE PROPRII (left) ──────────────────────────────
  const [left, setLeft] = useState([]);
  const didFetchLeft = useRef(false);

  useEffect(() => {
    if (didFetchLeft.current) return;
    didFetchLeft.current = true;

    (async () => {
      try {
        const accesses = await getAccessesForOrganization(organizationId);
        setLeft(accesses);
      } catch {
        // tratament eroare
      }
    })();
  }, [organizationId]);

  // ─── PARTAJATE (right) ──────────────────────────────────
  const [right, setRight] = useState([]);
  const [targetOrg, setTargetOrg] = useState(null);

  useEffect(() => {
    if (!targetOrg) {
      setRight([]);
      return;
    }
    (async () => {
      try {
        const shares = await getSharedAccessesBetweenOrgs(
          organizationId,
          targetOrg.value
        );
        const sharedIds = shares.map(s => s.accessId);
        setRight(left.filter(a => sharedIds.includes(a.id)));
      } catch {
        setRight([]);
      }
    })();
  }, [organizationId, targetOrg, left]);

  // ─── Checked items for transfer ──────────────────────────
  const [checked, setChecked] = useState([]);
  const leftOnly    = not(left, right);
  const leftChecked = intersection(checked, leftOnly);
  const rightChecked= intersection(checked, right);

  const handleToggle = item => {
    setChecked(prev =>
      prev.some(x => x.id === item.id)
        ? prev.filter(x => x.id !== item.id)
        : [...prev, item]
    );
  };

  // ─── Share / Revoke ─────────────────────────────────────
  const handleShare = async items => {
    if (!targetOrg) return;
    for (const acc of items) {
      await shareAccessWithOrganization(
        organizationId, targetOrg.value, acc.id
      );
    }
    setChecked([]);
    // reîmprospătăm doar partea “right”
    const shares = await getSharedAccessesBetweenOrgs(
      organizationId, targetOrg.value
    );
    const sharedIds = shares.map(s => s.accessId);
    setRight(left.filter(a => sharedIds.includes(a.id)));
  };

  const handleRevoke = async items => {
    if (!targetOrg) return;
    for (const acc of items) {
      await revokeSharedAccess(
        organizationId, targetOrg.value, acc.id
      );
    }
    setChecked([]);
    const shares = await getSharedAccessesBetweenOrgs(
      organizationId, targetOrg.value
    );
    const sharedIds = shares.map(s => s.accessId);
    setRight(left.filter(a => sharedIds.includes(a.id)));
  };

  // ─── Helper pentru listă ───────────────────────────────
  const customList = (title, items) => {
    const allSelected     = items.length > 0 && intersection(checked, items).length === items.length;
    const partialSelected = intersection(checked, items).length > 0 && !allSelected;

    const handleToggleAll = () => {
      if (allSelected) {
        setChecked(prev => prev.filter(c => !items.includes(c)));
      } else {
        setChecked(prev => [...prev, ...not(items, prev)]);
      }
    };

    return (
      <Paper className="org-transfer-list">
        <Box
          className="org-transfer-header"
          sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", px: 1 }}
        >
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
              <ListItemText
                id={`transfer-item-${item.id}`}
                primary={item.name}
              />
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
            >≫</Button>
            <Button
              variant="outlined"
              size="small"
              onClick={() => handleShare(leftChecked)}
              disabled={!leftChecked.length}
            >›</Button>
            <Button
              variant="outlined"
              size="small"
              onClick={() => handleRevoke(rightChecked)}
              disabled={!rightChecked.length}
            >‹</Button>
            <Button
              variant="outlined"
              size="small"
              onClick={() => handleRevoke(right)}
              disabled={!right.length}
            >≪</Button>
          </Box>
          {customList("Partajate", right)}
        </Box>
      )}
    </Box>
  );
}
