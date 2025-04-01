// src/components/Main/Main.jsx
import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { verifyToken } from "../../api/authApi";

function Main() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function checkToken() {
      const token = localStorage.getItem("jwtToken");
      if (!token) {
        // Nu există token, redirecționează spre login
        navigate("/login");
        return;
      }
      try {
        // Verifică tokenul
        await verifyToken();
        // Dacă totul este ok, oprește starea de loading
        setLoading(false);
      } catch (error) {
        // Dacă verificarea eșuează, redirecționează spre login
        navigate("/login");
      }
    }
    checkToken();
  }, [navigate]);

  if (loading) {
    return <div>Loading...</div>;
  }

  return (
    <div style={{ display: "flex", justifyContent: "center", alignItems: "center", height: "100vh" }}>
      {/* Afișează logo-ul sau orice alt element de welcome */}
      <img src="/logo.png" alt="Logo" style={{ maxWidth: "200px" }} />
    </div>
  );
}

export default Main;
