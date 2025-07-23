// src/components/Scan/Scan.jsx
import React from "react";
import ScanTable from "./TableScan/ScanTable";
import "../../styles/components/scan.css";

export default function Scan() {
  return (
    <div className="scan-page">
      <ScanTable />
    </div>
  );
}
