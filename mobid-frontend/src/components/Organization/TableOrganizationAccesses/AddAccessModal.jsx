// src/components/Organization/TableOrganizationAccesses/AddAccessModal.jsx
import React, { useEffect, useState, useRef } from "react";
import Select from "react-select";
import TextField from "@mui/material/TextField";
import "../../../styles/components/modal/index.css";
import "../../../styles/components/access.css";
import { createAccess } from "../../../api/accessApi";
import { getAllAccessTypes } from "../../../api/accessTypeApi";

const Steps = {
  SELECT_TYPE: 0,
  CONFIGURE:   1,
  PREVIEW:     2,
};

export default function AddAccessModal({ organizationId, onSuccess, onClose }) {
  const [step, setStep]                   = useState(Steps.SELECT_TYPE);
  const [types, setTypes]                 = useState([]);
  const [loadingTypes, setLoadingTypes]   = useState(false);
  const [error, setError]                 = useState("");
  const didFetchRef                       = useRef(false);

  // Step-1 fields
  const [name, setName]                   = useState("");
  const [description, setDescription]     = useState("");
  const [selectedType, setSelectedType]   = useState(null);

  // Step-2 fields
  const [expirationDate, setExpirationDate]       = useState("");
  const [restrictToMembers, setRestrictToMembers] = useState(false);
  const [restrictToSharing, setRestrictToSharing] = useState(false);
  const [isMultiScan, setIsMultiScan]             = useState(false);

  const [totalUseLimit, setTotalUseLimit]         = useState("");
  const periodOptions = [
    { value: 1,  label: "1 lună" },
    { value: 3,  label: "3 luni" },
    { value: 6,  label: "6 luni" },
    { value: 12, label: "1 an" },
  ];
  const [subscriptionPeriod, setSubscriptionPeriod] = useState(periodOptions[0]);
  const [useLimitPerPeriod, setUseLimitPerPeriod]   = useState("");

  // Load access types once
  useEffect(() => {
    if (didFetchRef.current) return;
    didFetchRef.current = true;

    (async () => {
      setLoadingTypes(true);
      try {
        const data = await getAllAccessTypes();
        setTypes(data.map(t => ({
          value: t.id,
          label: t.name,
          code:  t.code
        })));
      } catch {
        setError("Eroare la încărcarea tipurilor de acces.");
      } finally {
        setLoadingTypes(false);
      }
    })();
  }, []);

  // Navigation handlers
  const goToConfigure = () => {
    if (!name.trim()) {
      setError("Completează numele accesului.");
      return;
    }
    if (!selectedType) {
      setError("Selectează un tip de acces.");
      return;
    }
    setError("");
    setStep(Steps.CONFIGURE);
  };

  const goToPreview = () => {
    if (selectedType.code === "LimitedUse" && (!totalUseLimit || totalUseLimit <= 0)) {
      setError("TotalUseLimit este necesar pentru LimitedUse.");
      return;
    }
    if (selectedType.code === "Subscription" && (!subscriptionPeriod || subscriptionPeriod.value <= 0)) {
      setError("SubscriptionPeriodMonths este necesar pentru Subscription.");
      return;
    }
    setError("");
    setStep(Steps.PREVIEW);
  };

  const goBack = () => {
    setError("");
    setStep(step - 1);
  };

  // Final submit
  const handleSubmit = async () => {
    const payload = {
      name,
      description:               description || null,
      organizationId,
      accessTypeId:              selectedType.value,
      expirationDateTime:        expirationDate || null,
      restrictToOrgMembers:      restrictToMembers,
      restrictToOrgSharing:      restrictToSharing,
      isMultiScan:               isMultiScan,
      ...(selectedType.code === "OneUse" && { totalUseLimit: 1 }),
      ...(selectedType.code === "LimitedUse" && {
        totalUseLimit: Number(totalUseLimit)
      }),
      ...(selectedType.code === "Subscription" && {
        subscriptionPeriodMonths: subscriptionPeriod.value,
        useLimitPerPeriod:        useLimitPerPeriod ? Number(useLimitPerPeriod) : null
      }),
    };

    try {
      await createAccess(payload);
      onSuccess();
      onClose();
    } catch (err) {
      setError("Eroare la crearea accesului: " + err.message);
    }
  };

  return (
    <div className="modal__overlay" onClick={onClose}>
      <div className="modal__content" onClick={e => e.stopPropagation()}>

        {/* Close on steps 1 & 2 */}
        {step !== Steps.PREVIEW && (
          <button className="modal__close" onClick={onClose}>×</button>
        )}

        <h3 className="modal__title">
          {step === Steps.SELECT_TYPE && "Add new Access"}
          {step === Steps.CONFIGURE   && `Config “${selectedType?.label}”`}
          {step === Steps.PREVIEW     && `Preview “${selectedType?.label}”`}
        </h3>

        {error && <p className="modal__error">{error}</p>}

        {/* ─── Step 1: General ───────────────────────────── */}
        {step === Steps.SELECT_TYPE && (
          <div className="modal__form">
            <TextField
              label="Nume Acces *"
              variant="outlined"
              value={name}
              onChange={e => setName(e.target.value)}
              required fullWidth margin="normal"
            />
            <TextField
              label="Description"
              variant="outlined"
              value={description}
              onChange={e => setDescription(e.target.value)}
              fullWidth margin="normal"
              multiline rows={3}
            />
            <label className="modal__section-title">Tip Acces *</label>
            <Select
              options={types}
              isLoading={loadingTypes}
              value={selectedType}
              onChange={setSelectedType}
              placeholder="Selectează tipul de acces..."
              className="modal__react-select"
              classNamePrefix="modal__react-select"
            />
            <div className="modal__actions">
              <button type="button" className="modal__button--yes" onClick={goToConfigure}>
                Next
              </button>
              <button type="button" className="modal__button--no" onClick={onClose}>
                Cancel
              </button>
            </div>
          </div>
        )}

        {/* ─── Step 2: Configuration ─────────────────────── */}
        {step === Steps.CONFIGURE && (
          <div className="modal__form">
            <TextField
              label="Expiration Date"
              type="date"
              variant="outlined"
              value={expirationDate}
              onChange={e => setExpirationDate(e.target.value)}
              fullWidth margin="normal"
              InputLabelProps={{ shrink: true }}
            />

            <label className="checkbox-container">
              <input
                type="checkbox"
                checked={restrictToMembers}
                onChange={e => setRestrictToMembers(e.target.checked)}
              />
              <span>Restricted to org mambers</span>
            </label>

            <label className="checkbox-container">
              <input
                type="checkbox"
                checked={restrictToSharing}
                onChange={e => setRestrictToSharing(e.target.checked)}
              />
              <span>Restricted Sharing</span>
            </label>

            {/* ← Checkbox instead of a select */}
            <label className="checkbox-container">
              <input
                type="checkbox"
                checked={isMultiScan}
                onChange={e => setIsMultiScan(e.target.checked)}
              />
              <span>MultiScan</span>
            </label>

            {selectedType.code === "LimitedUse" && (
              <TextField
                label="Total Use Limit *"
                type="number"
                variant="outlined"
                value={totalUseLimit}
                onChange={e => setTotalUseLimit(e.target.value)}
                fullWidth margin="normal"
                inputProps={{ min: 1 }}
                required
              />
            )}

            {selectedType.code === "Subscription" && (
              <>
                <label className="modal__section-title">Subscription Period *</label>
                <Select
                  options={periodOptions}
                  value={subscriptionPeriod}
                  onChange={setSubscriptionPeriod}
                  className="modal__react-select"
                  classNamePrefix="modal__react-select"
                />
                <TextField
                  label="Uses Per Period"
                  type="number"
                  variant="outlined"
                  value={useLimitPerPeriod}
                  onChange={e => setUseLimitPerPeriod(e.target.value)}
                  fullWidth margin="normal"
                  inputProps={{ min: 1 }}
                />
              </>
            )}

            <div className="modal__actions">
              <button type="button" className="modal__button--yes" onClick={goToPreview}>
                Preview
              </button>
              <button type="button" className="modal__button--no" onClick={goBack}>
                Back
              </button>
            </div>
          </div>
        )}

        {/* ─── Step 3: Preview ───────────────────────────── */}
        {step === Steps.PREVIEW && (
          <>
            <div className="modal__form">
              <div className="preview-row"><strong>Name:</strong> {name}</div>
              <div className="preview-row"><strong>Description:</strong> {description || "(niciuna)"}</div>
              <div className="preview-row"><strong>Type:</strong> {selectedType.label}</div>
              <div className="preview-row"><strong>Expires:</strong> {expirationDate || "Nelimitat"}</div>
              <div className="preview-row"><strong>Only for members:</strong> {restrictToMembers ? "Da" : "Nu"}</div>
              <div className="preview-row"><strong>Shareadble:</strong> {!restrictToSharing ? "Da" : "Nu"}</div>
              <div className="preview-row"><strong>Multi scan:</strong> {isMultiScan ? "Da" : "Nu"}</div>
              {selectedType.code === "LimitedUse" && (
                <div className="preview-row"><strong>Total Use Limit:</strong> {totalUseLimit}</div>
              )}
              {selectedType.code === "Subscription" && (
                <>
                  <div className="preview-row"><strong>Subscription Period:</strong> {subscriptionPeriod.label}</div>
                  <div className="preview-row"><strong>Uses Per Period:</strong> {useLimitPerPeriod || "Nelimitat"}</div>
                </>
              )}
            </div>
            <div className="modal__actions">
              <button type="button" className="modal__button--yes" onClick={handleSubmit}>
                Confirm and Save
              </button>
              <button type="button" className="modal__button--no" onClick={goBack}>
                Back
              </button>
            </div>
          </>
        )}

      </div>
    </div>
  );
}
