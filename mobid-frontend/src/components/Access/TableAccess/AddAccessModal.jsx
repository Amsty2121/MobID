// src/components/Access/AddAccessModal.jsx
import React, { useEffect, useState } from "react";
import Select from "react-select";
import { getAllAccessTypes, createAccess } from "../../../api/accessApi";
import "../Access.css";

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
  const [step,               setStep]               = useState(Steps.SELECT_TYPE);
  const [types,              setTypes]              = useState([]);
  const [selectedType,       setSelectedType]       = useState(null);
  const [loadingTypes,       setLoadingTypes]       = useState(false);
  const [error,              setError]              = useState("");

  // câmpuri comune
  const [name,               setName]               = useState("");
  const [description,        setDescription]        = useState("");
  const [expirationDate,     setExpirationDate]     = useState("");
  const [restrictToMembers,  setRestrictToMembers]  = useState(false);
  const [scanMode,           setScanMode]           = useState(scanModeOptions[0]);

  // câmpuri dinamice
  const [maxUses,                  setMaxUses]                  = useState("");
  const [maxUsersPerPass,          setMaxUsersPerPass]          = useState("");
  const [monthlyLimit,             setMonthlyLimit]             = useState("");
  const [subscriptionPeriod,       setSubscriptionPeriod]       = useState(periodOptions[0]);
  const [usesPerPeriod,            setUsesPerPeriod]            = useState("");

  useEffect(() => {
    (async () => {
      setLoadingTypes(true);
      try {
        const data = await getAllAccessTypes();
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

  const toPreview = () => {
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
      name,
      description,
      accessTypeId: selectedType.value,
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
      <div className="modal-content preview-modal" onClick={e => e.stopPropagation()}>
        {step !== Steps.PREVIEW && (
          <button className="modal-close" onClick={onClose}>×</button>
        )}

        <h3 className={step === Steps.PREVIEW ? "preview-header" : ""}>
          {step === Steps.SELECT_TYPE
            ? "Alege Tip Acces"
            : step === Steps.CONFIGURE
              ? `Configurează “${selectedType.label}”`
              : `Previzualizează “${selectedType.label}”`}
        </h3>

        {error && <p className="error">{error}</p>}

        {/* Pasul 1: nume + select type */}
        {step === Steps.SELECT_TYPE && (
          <div className="add-org-form">
            <label htmlFor="accessName">Nume Acces</label>
            <input
              id="accessName"
              type="text"
              value={name}
              onChange={e => setName(e.target.value)}
              placeholder="Introdu numele accesului..."
            />

            <label htmlFor="accessTypeSelect">Alege Tip Acces</label>
            <Select
              inputId="accessTypeSelect"
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
          </div>
        )}

        {/* Pasul 2: configure */}
        {step === Steps.CONFIGURE && (
          <div className="add-org-form">
            <div className="access-name-subtitle">{name}</div>

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

            <label className="checkbox-container">
              <span>Restricționează la membri organizație</span>
              <input
                type="checkbox"
                checked={restrictToMembers}
                onChange={e => setRestrictToMembers(e.target.checked)}
              />
            </label>

            <label>Mod scanare</label>
            <Select
              options={scanModeOptions}
              value={scanMode}
              onChange={setScanMode}
              className="org-select"
              classNamePrefix="org-select"
            />

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
              {[
                ["Tip Acces", selectedType.label],
                ["Nume Acces", name],
                ["Expirare", expirationDate || "(niciuna)"],
                ["Restricționat", restrictToMembers ? "Da" : "Nu"],
                ["Mod Scanare", scanMode.label],
                ...(selectedType.name === "OneUse"
                  ? [["Max utilizatori/scanare", maxUsersPerPass]]
                  : []),
                ...(selectedType.name === "MultiUse"
                  ? [
                      ["Max utilizări", maxUses],
                      ["Max utilizatori/scanare", maxUsersPerPass]
                    ]
                  : []),
                ...(selectedType.name === "Subscription"
                  ? [
                      ["Limită/ lună", monthlyLimit],
                      ["Perioadă", subscriptionPeriod.label],
                      ["Utilizări/ perioadă", usesPerPeriod || "(nelimitat)"]
                    ]
                  : []),
                ["Descriere", description || "(niciuna)"]
              ].map(([label, val]) => (
                <div className="preview-row" key={label}>
                  <span className="preview-label">{label}:</span>
                  <span className="preview-value">{val}</span>
                </div>
              ))}
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
