// src/components/Access/AddAccessModal.jsx
import React, { useEffect, useState } from "react";
import Select from "react-select";
import { getAccessTypes, createAccess } from "../../api/accessApi";
import "./Access.css";

const Steps = {
  SELECT_TYPE: 0,
  CONFIGURE:   1,
  PREVIEW:     2,
};

const scanModeOptions = [
  { value: "SingleScan", label: "Single Scan" },
  { value: "MultiScan",  label: "Multi Scan"  },
];
const periodOptions = [
  { value: 1,  label: "1 lună" },
  { value: 3,  label: "3 luni" },
  { value: 6,  label: "6 luni" },
  { value: 12, label: "1 an" },
];

export default function AddAccessModal({ organizationId, onSuccess, onClose }) {
  const [step, setStep]               = useState(Steps.SELECT_TYPE);
  const [types, setTypes]             = useState([]);
  const [selectedType, setSelectedType] = useState(null);
  const [loadingTypes, setLoadingTypes] = useState(false);
  const [error, setError]             = useState("");

  // câmpuri comune
  const [expirationDate, setExpirationDate]       = useState("");
  const [description, setDescription]             = useState("");
  const [restrictToMembers, setRestrictToMembers] = useState(false);
  const [scanMode, setScanMode]                   = useState(scanModeOptions[0]);

  // câmpuri dinamice
  const [maxUses, setMaxUses]                       = useState("");
  const [maxUsersPerPass, setMaxUsersPerPass]       = useState("");
  const [monthlyLimit, setMonthlyLimit]             = useState("");
  const [subscriptionPeriod, setSubscriptionPeriod] = useState(periodOptions[0]);
  const [usesPerPeriod, setUsesPerPeriod]           = useState("");

  useEffect(() => {
    (async () => {
      setLoadingTypes(true);
      try {
        const data = await getAccessTypes();
        setTypes(data.map(t => ({
          value: t.id,
          label: t.name,
          name:  t.name
        })));
      } catch {
        setError("Eroare la încărcarea tipurilor de acces.");
      } finally {
        setLoadingTypes(false);
      }
    })();
  }, []);

  const next = () => {
    if (!selectedType) {
      setError("Selectează un tip de acces.");
      return;
    }
    setError("");
    setStep(Steps.CONFIGURE);
  };

  const toPreview = () => {
    // validări dynamice înainte de preview
    if (selectedType.name === "OneUse" && !maxUsersPerPass) {
      setError("Completează numărul maxim de utilizatori / scanare.");
      return;
    }
    if (selectedType.name === "MultiUse" && (!maxUses || !maxUsersPerPass)) {
      setError("Completează max utilizări și max utilizatori / scanare.");
      return;
    }
    if (selectedType.name === "Subscription" && (!monthlyLimit || !subscriptionPeriod)) {
      setError("Completează limita lunară și durata abonament.");
      return;
    }
    if (description.length > 200) {
      setError("Descrierea nu poate depăși 200 caractere.");
      return;
    }
    setError("");
    setStep(Steps.PREVIEW);
  };

  const back = () => {
    setError("");
    setStep(step - 1);
  };

  const handleSubmit = async () => {
    const payload = {
      organizationId,
      accessTypeId: selectedType.value,
      description,
      expirationDate: expirationDate || null,
      restrictToOrganizationMembers: restrictToMembers,
      scanMode: scanMode.value,
      ...(selectedType.name === "OneUse" && {
        maxUsersPerPass: Number(maxUsersPerPass),
      }),
      ...(selectedType.name === "MultiUse" && {
        maxUses:         Number(maxUses),
        maxUsersPerPass: Number(maxUsersPerPass),
      }),
      ...(selectedType.name === "Subscription" && {
        monthlyLimit:             Number(monthlyLimit),
        subscriptionPeriodMonths: subscriptionPeriod.value,
        usesPerPeriod:            usesPerPeriod ? Number(usesPerPeriod) : null,
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
    <div className="modal-overlay" onClick={onClose}>
      {/* ocolim închiderea când dăm click în interior */}
      <div className="modal-content preview-modal" onClick={e => e.stopPropagation()}>
        {step !== Steps.PREVIEW && (
          <button className="modal-close" onClick={onClose}>×</button>
        )}

        {/* Titlu dinamic */}
        <h3 className={step === Steps.PREVIEW ? "preview-header" : ""}>
          {step === Steps.SELECT_TYPE
            ? "Alege Tip Acces"
            : step === Steps.CONFIGURE
              ? `Configurează “${selectedType.label}”`
              : `Previzualizează “${selectedType.label}”`}
        </h3>

        {error && <p className="error">{error}</p>}

        {/* Pasul 1: select type */}
        {step === Steps.SELECT_TYPE && (
          <>
            <Select
              options={types}
              isLoading={loadingTypes}
              value={selectedType}
              onChange={setSelectedType}
              placeholder="Selectează tipul de acces..."
              className="org-select"
              classNamePrefix="org-select"
              noOptionsMessage={() => "Niciun tip găsit"}
            />
            <div className="form-actions">
              <button type="button" onClick={next}>Următor</button>
              <button type="button" onClick={onClose}>Anulează</button>
            </div>
          </>
        )}

        {/* Pasul 2: configure */}
        {step === Steps.CONFIGURE && (
          <div className="add-org-form">
            <label>Descriere (max 200 caractere)</label>
            <textarea
              maxLength={200}
              value={description}
              onChange={e => setDescription(e.target.value)}
            />

            <label>Data expirării (opțional)</label>
            <input
              type="date"
              value={expirationDate}
              onChange={e => setExpirationDate(e.target.value)}
            />

            <label>
              <input
                type="checkbox"
                checked={restrictToMembers}
                onChange={e => setRestrictToMembers(e.target.checked)}
              />
              {' '}Restricționează la membri organizație
            </label>

            <label>Mod scanare</label>
            <Select
              options={scanModeOptions}
              value={scanMode}
              onChange={setScanMode}
              className="org-select"
              classNamePrefix="org-select"
            />

            {/* câmpuri dinamice */}
            {selectedType.name === "OneUse" && (
              <>
                <label>Max utilizatori / scanare</label>
                <input
                  type="number"
                  min="1"
                  value={maxUsersPerPass}
                  onChange={e => setMaxUsersPerPass(e.target.value)}
                />
              </>
            )}
            {selectedType.name === "MultiUse" && (
              <>
                <label>Max utilizări totale</label>
                <input
                  type="number"
                  min="1"
                  value={maxUses}
                  onChange={e => setMaxUses(e.target.value)}
                />
                <label>Max utilizatori / scanare</label>
                <input
                  type="number"
                  min="1"
                  value={maxUsersPerPass}
                  onChange={e => setMaxUsersPerPass(e.target.value)}
                />
              </>
            )}
            {selectedType.name === "Subscription" && (
              <>
                <label>Limită utilizări / lună</label>
                <input
                  type="number"
                  min="1"
                  value={monthlyLimit}
                  onChange={e => setMonthlyLimit(e.target.value)}
                />
                <label>Durata abonament (luni)</label>
                <Select
                  options={periodOptions}
                  value={subscriptionPeriod}
                  onChange={setSubscriptionPeriod}
                  className="org-select"
                  classNamePrefix="org-select"
                />
                <label>Utilizări permise / perioadă</label>
                <input
                  type="number"
                  min="1"
                  value={usesPerPeriod}
                  onChange={e => setUsesPerPeriod(e.target.value)}
                />
              </>
            )}

            <div className="form-actions">
              <button type="button" onClick={back}>Înapoi</button>
              <button type="button" onClick={toPreview}>Previzualizare</button>
            </div>
          </div>
        )}

        {/* Pasul 3: preview */}
        {step === Steps.PREVIEW && (
          <>
            <div className="preview-body">
              <div className="preview-row">
                <span className="preview-label">Tip Acces:</span>
                <span className="preview-value">{selectedType.label}</span>
              </div>
              <div className="preview-row">
                <span className="preview-label">Expirare:</span>
                <span className="preview-value">{expirationDate || "(niciuna)"}</span>
              </div>
              <div className="preview-row">
                <span className="preview-label">Restricționat:</span>
                <span className="preview-value">{restrictToMembers ? "Da" : "Nu"}</span>
              </div>
              <div className="preview-row">
                <span className="preview-label">Mod Scanare:</span>
                <span className="preview-value">{scanMode.label}</span>
              </div>
              {selectedType.name === "OneUse" && (
                <div className="preview-row">
                  <span className="preview-label">Max utilizatori / scanare:</span>
                  <span className="preview-value">{maxUsersPerPass}</span>
                </div>
              )}
              {selectedType.name === "MultiUse" && (
                <>
                  <div className="preview-row">
                    <span className="preview-label">Max utilizări:</span>
                    <span className="preview-value">{maxUses}</span>
                  </div>
                  <div className="preview-row">
                    <span className="preview-label">Max utilizatori / scanare:</span>
                    <span className="preview-value">{maxUsersPerPass}</span>
                  </div>
                </>
              )}
              {selectedType.name === "Subscription" && (
                <>
                  <div className="preview-row">
                    <span className="preview-label">Limită / lună:</span>
                    <span className="preview-value">{monthlyLimit}</span>
                  </div>
                  <div className="preview-row">
                    <span className="preview-label">Perioadă:</span>
                    <span className="preview-value">{subscriptionPeriod.label}</span>
                  </div>
                  <div className="preview-row">
                    <span className="preview-label">Utilizări / perioadă:</span>
                    <span className="preview-value">{usesPerPeriod || "(nelimitat)"}</span>
                  </div>
                </>
              )}
              <div className="preview-row">
                <span className="preview-label">Descriere:</span>
                <span className="preview-value">{description || "(niciuna)"}</span>
              </div>
            </div>

            <div className="form-actions">
              <button type="button" onClick={back}>Înapoi</button>
              <button type="button" onClick={handleSubmit}>Confirmă și Salvează</button>
            </div>
          </>
        )}
      </div>
    </div>
  );
}
