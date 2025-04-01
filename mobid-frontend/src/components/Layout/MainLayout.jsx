// src/components/Layout/MainLayout.jsx
import React, { useState } from "react";
import { Outlet, useNavigate, Link } from "react-router-dom";
import { FaBars, FaTimes } from "react-icons/fa"; // folosim iconițe pentru meniu
import "./MainLayout.css";

function MainLayout() {
  const navigate = useNavigate();
  const [sidebarOpen, setSidebarOpen] = useState(true);

  // Exemplu: numele utilizatorului
  const username = localStorage.getItem("username") || "User Name";

  // Toggle sidebar
  const toggleSidebar = () => {
    setSidebarOpen((prev) => !prev);
  };

  const handleLogout = () => {
    localStorage.removeItem("jwtToken");
    navigate("/login");
  };

  return (
    <div className="layout-container">
      {/* Bara de meniu de sus */}
      <header className="top-bar">
        {/* Secțiunea oranj (220px) */}
        <div className="orange-section">
          <Link
            to="/"
            className="brand-text"
            style={{ textDecoration: "none", color: "#fff" }}
          >
            MobID
          </Link>
        </div>

        {/* Secțiunea albă (restul spațiului) */}
        <div className="white-section">
          <div className="white-left">
            <button
              className="hamburger-btn"
              onClick={toggleSidebar}
              aria-label="Toggle sidebar"
            >
              {sidebarOpen ? <FaTimes /> : <FaBars />}
            </button>
          </div>
          <div className="white-right">
            <span className="user-name">{username}</span>
            <button
              className="logout-btn"
              onClick={handleLogout}
              aria-label="Logout"
            >
              ⤴
            </button>
          </div>
        </div>
      </header>

      {/* Container pentru sidebar + conținut */}
      <div className="content-container">
        {/* Sidebar */}
        <aside className={`left-sidebar ${sidebarOpen ? "" : "hidden"}`}>
          <nav>
            <ul>
              <li>
                <Link to="/users">Utilizatori</Link>
              </li>
              <li>
                <Link to="/roles">Roluri</Link>
              </li>
              <li>
                <Link to="/organizations">Organizații</Link>
              </li>
              <li>
                <Link to="/accesses">Accese</Link>
              </li>
              <li>
                <Link to="/qrcodes">Coduri QR</Link>
              </li>
              <li>
                <Link to="/scans">Scanări</Link>
              </li>
            </ul>
          </nav>
        </aside>

        {/* Conținut principal */}
        <main className="main-content">
          <Outlet />
        </main>
      </div>
    </div>
  );
}

export default MainLayout;
