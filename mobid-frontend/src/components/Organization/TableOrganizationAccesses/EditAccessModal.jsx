// src/components/Organization/TableOrganizationAccesses/EditAccessModal.jsx
import React, { useState, useEffect } from "react";
import TextField from "@mui/material/TextField";
import { FaTimes } from "react-icons/fa";
import "../../../styles/components/modal/index.css";
import "../../../styles/components/access.css";
import { updateAccess } from "../../../api/accessApi";

export default function EditAccessModal({ access, onSuccess, onClose }) {
  const [name, setName]                           = useState("");
  const [description, setDescription]             = useState("");
  const [expirationDate, setExpirationDate]       = useState("");
  const [restrictToMembers, setRestrictToMembers] = useState(false);
  const [restrictToSharing, setRestrictToSharing] = useState(false);
  const [isMultiScan, setIsMultiScan]             = useState(false);
  const [totalUseLimit, setTotalUseLimit]         = useState("");
  const [subscriptionPeriod, setSubscriptionPeriod] = useState("");
  const [useLimitPerPeriod, setUseLimitPerPeriod] = useState("");
  const [error, setError]                         = useState("");

  useEffect(() => {
    if (!access) return;
    setName(access.name);
    setDescription(access.description || "");
    setExpirationDate(
      access.expirationDateTime
        ? access.expirationDateTime.split("T")[0]
        : ""
    );
    setRestrictToMembers(Boolean(access.restrictToOrgMembers));
    setRestrictToSharing(Boolean(access.restrictToOrgSharing));
    setIsMultiScan(Boolean(access.isMultiScan));
    setTotalUseLimit(access.totalUseLimit ?? "");
    setSubscriptionPeriod(
      access.subscriptionPeriodMonths?.toString() ?? ""
    );
    setUseLimitPerPeriod(access.useLimitPerPeriod?.toString() ?? "");
    setError("");
  }, [access]);

  const handleSubmit = async e => {
    e.preventDefault();
    setError("");
    try {
      const payload = {
        id: access.id,
        name: name.trim(),
        description: description || null,
        expirationDateTime: expirationDate || null,
        restrictToOrgMembers: restrictToMembers,
        restrictToOrgSharing: restrictToSharing,
        isMultiScan: isMultiScan,
        // numai dacă există în AccessCreateReq
        totalUseLimit: totalUseLimit
          ? Number(totalUseLimit)
          : null,
        subscriptionPeriodMonths: subscriptionPeriod
          ? Number(subscriptionPeriod)
          : null,
        useLimitPerPeriod: useLimitPerPeriod
          ? Number(useLimitPerPeriod)
          : null
      };
      await updateAccess(payload);
      onSuccess();
      onClose();
    } catch (err) {
      setError(err.response?.data?.message || err.message);
    }
  };

  // Detectăm tipul după câmpurile existente
  const isLimitedUse   = access.totalUseLimit != null;
  const isSubscription = access.subscriptionPeriodMonths != null;

  return (
    <div className="modal__overlay" onClick={onClose}>
      <div
        className="modal__content"
        onClick={e => e.stopPropagation()}
      >
        <button className="modal__close" onClick={onClose}>
          <FaTimes />
        </button>
        <h3 className="modal__title">Editează Acces “{access.name}”</h3>
        {error && <p className="modal__error">{error}</p>}
        <form onSubmit={handleSubmit} className="modal__form">
          <TextField
            label="Nume Acces"
            value={name}
            onChange={e => setName(e.target.value)}
            required
            variant="outlined"
            fullWidth
            margin="normal"
          />
          <TextField
            label="Descriere"
            value={description}
            onChange={e => setDescription(e.target.value)}
            variant="outlined"
            fullWidth
            margin="normal"
            multiline
            rows={3}
          />
          <TextField
            label="Data expirării"
            type="date"
            value={expirationDate}
            onChange={e => setExpirationDate(e.target.value)}
            variant="outlined"
            fullWidth
            margin="normal"
            InputLabelProps={{ shrink: true }}
          />
          <label className="checkbox-container">
              <input
                type="checkbox"
                checked={restrictToMembers}
                onChange={e => setRestrictToMembers(e.target.checked)}
              />
              <span>Restricționează doar la membri organizație</span>
          </label>
          <label className="checkbox-container">
              <input
                type="checkbox"
                checked={restrictToSharing}
                onChange={e => setRestrictToSharing(e.target.checked)}
              />
              <span>Permite partajarea accesului</span>
          </label>
          <label className="checkbox-container">
              <input
                type="checkbox"
                checked={isMultiScan}
                onChange={e => setIsMultiScan(e.target.checked)}
              />
              <span>Scanare multiplă</span>
          </label>

          {isLimitedUse && (
            <TextField
              label="Max Total Use Limit *"
              type="number"
              value={totalUseLimit}
              onChange={e => setTotalUseLimit(e.target.value)}
              variant="outlined"
              fullWidth
              margin="normal"
              inputProps={{ min: 1 }}
              required
            />
          )}
          {isSubscription && (
            <>
              <TextField
                label="Durata abonament (luni) *"
                type="number"
                value={subscriptionPeriod}
                onChange={e => setSubscriptionPeriod(e.target.value)}
                variant="outlined"
                fullWidth
                margin="normal"
                inputProps={{ min: 1 }}
                required
              />
              <TextField
                label="Utilizări per perioadă"
                type="number"
                value={useLimitPerPeriod}
                onChange={e => setUseLimitPerPeriod(e.target.value)}
                variant="outlined"
                fullWidth
                margin="normal"
                inputProps={{ min: 1 }}
              />
            </>
          )}

          <div className="modal__actions">
            <button type="submit" className="modal__button--yes">
              Salvează
            </button>
            <button
              type="button"
              className="modal__button--no"
              onClick={onClose}
            >
              Anulează
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
