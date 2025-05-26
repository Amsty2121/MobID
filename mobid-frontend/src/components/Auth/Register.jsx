// src/components/Auth/Register.jsx
import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { registerUser } from "../../api/authApi";
import lockIcon from "../../assets/lock.svg";
import "./Login.css"; // reutilizăm aceleași stiluri

// aceleași emoji pentru toggle
const MonkeyOpenIcon = "🐵";
const MonkeyClosedIcon = "🙈";

export default function Register() {
  const [email, setEmail] = useState("");
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [inputType, setInputType] = useState("password");
  const [error, setError] = useState("");
  const navigate = useNavigate();

  const toggleVisibility = () =>
    setInputType(prev => (prev === "password" ? "text" : "password"));

  const handleSubmit = async e => {
    e.preventDefault();
    setError("");
    try {
      await registerUser({ email, username, password });
      navigate("/login");
    } catch (err) {
      setError(err.response?.data?.message || "Eroare la înregistrare");
    }
  };

  return (
    <div className="wrapper">
      <div className="login-wrapper slideInDown">
        <div className="heading">
          <img src={lockIcon} alt="padlock" className="mat-icon" />
          <div className="text">
            <span className="title">Register</span>
            <span className="subtitle">
              Create a new account
            </span>
          </div>
        </div>

        <form className="content" onSubmit={handleSubmit}>
          {error && <div className="login-error">{error}</div>}

          <div className="control">
            <span className="label">Email</span>
            <input
              type="email"
              placeholder="example@mail.com"
              value={email}
              onChange={e => setEmail(e.target.value)}
              required
            />
          </div>

          <div className="control">
            <span className="label">Username</span>
            <input
              type="text"
              placeholder="username"
              value={username}
              onChange={e => setUsername(e.target.value)}
              required
            />
          </div>

          <div className="control password-control">
            <span className="label">Password</span>
            <div className="password-row">
              <input
                className="pw-field"
                type={inputType}
                placeholder="********"
                value={password}
                onChange={e => setPassword(e.target.value)}
                required
              />
              <button
                type="button"
                className="toggle-password"
                onClick={toggleVisibility}
                aria-label="Toggle password visibility"
              >
                {inputType === "password"
                  ? MonkeyClosedIcon
                  : MonkeyOpenIcon}
              </button>
            </div>
          </div>

          <button type="submit">Register</button>

          {/* link înapoi la login */}
          <button
            type="button"
            className="register-button"
            onClick={() => navigate("/login")}
          >
            Back to Login
          </button>
        </form>
      </div>
    </div>
  );
}
