// src/routes/AppRoutes.jsx
import React from "react";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Login from "../components/Auth/Login";
import MainLayout from "../components/Layout/MainLayout";
import Role from "../components/Role/Role"; // importăm componenta Role

const AppRoutes = () => (
  <BrowserRouter>
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route element={<MainLayout />}>
        <Route path="/" element={<div>Dashboard (Template)</div>} />
        <Route path="/users" element={<div>Utilizatori (Template)</div>} />
        <Route path="/roles" element={<Role />} />
        <Route path="/organizations" element={<div>Organizații (Template)</div>} />
        <Route path="/accesses" element={<div>Accese (Template)</div>} />
        <Route path="/qrcodes" element={<div>Coduri QR (Template)</div>} />
        <Route path="/scans" element={<div>Scanări (Template)</div>} />
      </Route>
    </Routes>
  </BrowserRouter>
);

export default AppRoutes;
