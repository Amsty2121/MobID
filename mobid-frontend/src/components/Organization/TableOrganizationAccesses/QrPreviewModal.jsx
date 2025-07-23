// src/components/Organization/TableOrganizationAccesses/QrPreviewModal.jsx

import React from "react";
import { QRCodeSVG } from "qrcode.react";
import "../../../styles/components/modal/index.css";

export default function QrPreviewModal({ qr, onClose, onDownload }) {
  if (!qr) return null;

  return (
    <div className="qr-modal-overlay" onClick={onClose}>
      <div className="qr-modal-content" onClick={e => e.stopPropagation()}>
        <button className="modal-close" onClick={onClose}>×</button>
        <h3 className="qr-modal-title">{qr.name}</h3>
        <div className="qr-design-frame">
          <div className="qr-code-wrap">
            <QRCodeSVG
              id="qr-code-svg"
              value={qr.qrEncodedText}
              size={200}
              includeMargin={false}
              bgColor="#fff"
              fgColor="var(--color-tuna-light)"
            />
          </div>
          <div className="qr-footer">
            <span className="qr-footer-text">Scan me to get access</span>
          </div>
        </div>
        <button className="download-btn" onClick={onDownload}>
          Download QR as SVG
        </button>
      </div>
    </div>
  );
}
