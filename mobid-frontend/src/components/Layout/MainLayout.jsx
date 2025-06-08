// src/components/Layout/MainLayout.jsx

import React, { useState } from "react";
import { Outlet, useNavigate, Link } from "react-router-dom";
import { FaBars, FaTimes } from "react-icons/fa";
import "../../styles/components/layout.css"; // sau calea corectă
import logo from "../../assets/mobid_main.png";

function MainLayout() {
  const navigate = useNavigate();
  const [sidebarOpen, setSidebarOpen] = useState(true);
  const username = localStorage.getItem("username") || "User Name";

  return (
    <div className="layout-container">
      <header className="top-bar">
        <div className="orange-section">
          <Link to="/" className="brand-img-link">
            <img src={logo} alt="MobID Logo" />
          </Link>
        </div>
        <div className="white-section">
          <div className="white-left">
            <button
              className="hamburger-btn"
              onClick={() => setSidebarOpen(!sidebarOpen)}
              aria-label="Toggle sidebar"
            >
              {sidebarOpen ? <FaTimes /> : <FaBars />}
            </button>
          </div>
          <div className="white-right">
            <span className="user-name">{username}</span>
            <button
              className="logout-btn"
              onClick={() => {
                localStorage.removeItem("jwtToken");
                navigate("/login");
              }}
            >
              ⤴
            </button>
          </div>
        </div>
      </header>

      <div className="content-container">
        <aside className={`left-sidebar ${sidebarOpen ? "" : "hidden"}`}>
          <nav>
            <ul>
              <li><Link to="/roles">Roluri</Link></li>
              <li><Link to="/users">Utilizatori</Link></li>
              <li><Link to="/organizations">Organizații</Link></li>
              <li><Link to="/accesses">Accese</Link></li>
              <li><Link to="/qrcodes">Coduri QR</Link></li>
              <li><Link to="/scans">Scanări</Link></li>
            </ul>
          </nav>
        </aside>
        <main className="main-content">
          <Outlet />
        </main>
      </div>
    </div>
  );
}

export default MainLayout;
