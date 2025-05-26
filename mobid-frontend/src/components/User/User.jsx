// src/components/User/User.jsx
import React from "react";
import UserTable from "./Table/UserTable";
import "../../styles/components/user.css";

export default function User() {
  return (
    <div className="user-page">
      <UserTable />
    </div>
  );
}
