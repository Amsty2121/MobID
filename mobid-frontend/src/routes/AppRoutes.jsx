// src/routes/AppRoutes.jsx
import React from "react";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Login from "../components/Auth/Login";
import MainLayout from "../components/Layout/MainLayout";
import Role from "../components/Role/Role"; // importăm componenta Role
import User from "../components/User/User";
import Organization from "../components/Organization/Organization";
import Access from "../components/Access/Access";

const AppRoutes = () => (
  <BrowserRouter>
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route element={<MainLayout />}>
        <Route path="/" element={<div>Dashboard (Template)</div>} />
        <Route path="/users" element={<User />} />
        <Route path="/roles" element={<Role />} />
        <Route path="/organizations" element={<Organization />} />
        <Route path="/accesses" element={<Access />} />
        <Route path="/qrcodes" element={<div>Coduri QR (Template)</div>} />
        <Route path="/scans" element={<div>Scanări (Template)</div>} />
      </Route>
    </Routes>
  </BrowserRouter>
);

export default AppRoutes;
