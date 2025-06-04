// src/components/Access/AddAccessModal.jsx
import React, { useEffect, useState, useRef } from "react";
import Select from "react-select";
import TextField from "@mui/material/TextField";
import "../../../styles/components/modal/index.css";
import "../../../styles/components/access.css";
import { getAllAccessTypes, createAccess } from "../../../api/accessApi";

const Steps = {
  SELECT_TYPE: 0,
  CONFIGURE:   1,
  PREVIEW:     2,
};

export default function AddAccessModal({ organizationId, onSuccess, onClose }) {
  const [step, setStep] = useState(Steps.SELECT_TYPE);
  const [types, setTypes] = useState([]);
  const [loadingTypes, setLoadingTypes] = useState(false);
  const [error, setError] = useState("");
  const didFetchRef = useRef(false);

  // Pasul 1: detalii generale
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [selectedType, setSelectedType] = useState(null);

  // Pasul 2: configurare
  const [expirationDate, setExpirationDate] = useState("");
  const [restrictToMembers, setRestrictToMembers] = useState(false);

  const scanModeOptions = [
    { value: "SingleScan", label: "Single Scan" },
    { value: "MultiScan",  label: "Multi Scan"  },
  ];
  const [scanMode, setScanMode] = useState(scanModeOptions[0]);
  const [totalUseLimit, setTotalUseLimit] = useState("");
  const periodOptions = [
    { value: 1,  label: "1 lună" },
    { value: 3,  label: "3 luni" },
    { value: 6,  label: "6 luni" },
    { value: 12, label: "1 an" },
  ];
  const [subscriptionPeriod, setSubscriptionPeriod] = useState(periodOptions[0]);
  const [useLimitPerPeriod, setUseLimitPerPeriod] = useState("");

  // Încarcă tipurile
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
          name: t.name,
          isLimitedUse: t.isLimitedUse,
          isSubscription: t.isSubscription
        })));
      } catch {
        setError("Eroare la încărcarea tipurilor de acces.");
      } finally {
        setLoadingTypes(false);
      }
    })();
  }, []);

  // Navigare între pași
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
    if (selectedType.isLimitedUse && (!totalUseLimit || totalUseLimit <= 0)) {
      setError("TotalUseLimit (>0) este necesar pentru acest tip de acces.");
      return;
    }
    if (selectedType.isSubscription && (!subscriptionPeriod || subscriptionPeriod.value <= 0)) {
      setError("SubscriptionPeriod (>0) este necesar pentru acest tip de acces.");
      return;
    }
    setError("");
    setStep(Steps.PREVIEW);
  };

  const goBack = () => {
    setError("");
    setStep(step - 1);
  };

  // Trimite payload către server
  const handleSubmit = async () => {
    const payload = {
      name,
      description: description || null,
      organizationId,
      accessTypeId: selectedType.value,
      expirationDate: expirationDate || null,
      restrictToOrganizationMembers: restrictToMembers,
      scanMode: scanMode.value,
      ...(selectedType.name === "OneUse" && { totalUseLimit: 1 }),
      ...(selectedType.isLimitedUse && { totalUseLimit: Number(totalUseLimit) }),
      ...(selectedType.isSubscription && {
        subscriptionPeriodMonths: subscriptionPeriod.value,
        useLimitPerPeriod: useLimitPerPeriod ? Number(useLimitPerPeriod) : null
      })
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
        {step !== Steps.PREVIEW && (
          <button className="modal__close" onClick={onClose}>×</button>
        )}
        <h3 className="modal__title">
          {step === Steps.SELECT_TYPE && "Adaugă acces nou"}
          {step === Steps.CONFIGURE   && `Configurează “${selectedType?.label}”`}
          {step === Steps.PREVIEW     && `Previzualizează “${selectedType?.label}”`}
        </h3>

        {error && <p className="modal__error">{error}</p>}

        {/* Pasul 1 */}
        {step === Steps.SELECT_TYPE && (
          <div className="modal__form">
            <TextField
              label="Nume Acces *"
              variant="outlined"
              value={name}
              onChange={e => setName(e.target.value)}
              required
              fullWidth
              margin="normal"
            />
            <TextField
              label="Descriere"
              variant="outlined"
              value={description}
              onChange={e => setDescription(e.target.value)}
              fullWidth
              margin="normal"
              multiline
              rows={3}
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
                Următor
              </button>
              <button type="button" className="modal__button--no" onClick={onClose}>
                Anulează
              </button>
            </div>
          </div>
        )}

        {/* Pasul 2 */}
        {step === Steps.CONFIGURE && (
          <div className="modal__form">
            <TextField
              label="Data expirării"
              type="date"
              variant="outlined"
              value={expirationDate}
              onChange={e => setExpirationDate(e.target.value)}
              fullWidth
              margin="normal"
              InputLabelProps={{ shrink: true }}
            />
            <div className="checkbox-container">
              <input
                type="checkbox"
                checked={restrictToMembers}
                onChange={e => setRestrictToMembers(e.target.checked)}
              />
              <span>Restricționează doar la membri organizație</span>
            </div>
            <label className="modal__section-title">Mod Scanare</label>
            <Select
              options={scanModeOptions}
              value={scanMode}
              onChange={setScanMode}
              className="modal__react-select"
              classNamePrefix="modal__react-select"
            />
            {selectedType.isLimitedUse && (
              <TextField
                label="Total Use Limit *"
                type="number"
                variant="outlined"
                value={totalUseLimit}
                onChange={e => setTotalUseLimit(e.target.value)}
                fullWidth
                margin="normal"
                inputProps={{ min: 1 }}
              />
            )}
            {selectedType.isSubscription && (
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
                  fullWidth
                  margin="normal"
                  inputProps={{ min: 1 }}
                />
              </>
            )}
            <div className="modal__actions">
              <button type="button" className="modal__button--yes" onClick={goToPreview}>
                Previzualizare
              </button>
              <button type="button" className="modal__button--no" onClick={goBack}>
                Înapoi
              </button>
            </div>
          </div>
        )}

        {/* Pasul 3 */}
        {step === Steps.PREVIEW && (
          <>
            <div className="modal__form">
              <div className="preview-row"><strong>Nume:</strong> {name}</div>
              <div className="preview-row"><strong>Descriere:</strong> {description || "(niciuna)"}</div>
              <div className="preview-row"><strong>Tip:</strong> {selectedType.label}</div>
              {expirationDate && (
                <div className="preview-row"><strong>Expiră:</strong> {expirationDate}</div>
              )}
              <div className="preview-row"><strong>Restricționat:</strong> {restrictToMembers ? "Da" : "Nu"}</div>
              <div className="preview-row"><strong>Mod Scanare:</strong> {scanMode.label}</div>
              {selectedType.isLimitedUse && (
                <div className="preview-row"><strong>Total Use Limit:</strong> {totalUseLimit}</div>
              )}
              {selectedType.isSubscription && (
                <>
                  <div className="preview-row"><strong>Subscription Period:</strong> {subscriptionPeriod.label}</div>
                  <div className="preview-row"><strong>Uses Per Period:</strong> {useLimitPerPeriod || "Nelimitat"}</div>
                </>
              )}
            </div>
            <div className="modal__actions">
              <button type="button" className="modal__button--yes" onClick={handleSubmit}>
                Confirmă și Salvează
              </button>
              <button type="button" className="modal__button--no" onClick={goBack}>
                Înapoi
              </button>
            </div>
          </>
        )}
      </div>
    </div>
  );
}
