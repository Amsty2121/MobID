// src/components/Auth/Login.jsx

import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { loginUser } from "../../api/authApi";
import lockIcon from "../../assets/lock.svg";
import "./Login.css";

// Maimuță cu ochii deschiși pentru parolă vizibilă
const MonkeyOpenIcon = "🐵";
// Maimuță cu ochii acoperiți pentru parolă ascunsă
const MonkeyClosedIcon = "🙈";

function Login() {
  const [login, setLogin] = useState("");
  const [password, setPassword] = useState("");
  const [inputType, setInputType] = useState("password");
  const [error, setError] = useState("");
  const navigate = useNavigate();

  const toggleVisibility = () => {
    setInputType(prev => (prev === "password" ? "text" : "password"));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");

    try {
      const data = await loginUser({ login, password });
      localStorage.setItem("jwtToken", data.token);
       localStorage.setItem("username", data.username);
      navigate("/");
    } catch (err) {
      setError("Incorrect username and/or password");
    }
  };

  return (
    <div className="wrapper">
      <div className="login-wrapper slideInDown">
        <div className="heading">
          <img src={lockIcon} alt="padlock" className="mat-icon" />
          <div className="text">
            <span className="title">Login</span>
            <span className="subtitle">
              Please enter your credentials to login
            </span>
          </div>
        </div>

        <form className="content" onSubmit={handleSubmit}>
          {error && <div className="login-error">{error}</div>}

          {/* Email */}
          <div className="control">
            <span className="label">Email</span>
            <input
              type="email"
              placeholder="example@mail.com"
              value={login}
              onChange={(e) => setLogin(e.target.value)}
              required
            />
          </div>

          {/* Password + toggle pe același rând */}
          <div className="control password-control">
            <span className="label">Password</span>
            <div className="password-row">
              <input
                className="pw-field"
                type={inputType}
                placeholder="********************"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
              />
              <button
                type="button"
                className="toggle-password"
                onClick={toggleVisibility}
                aria-label="Toggle password visibility"
              >
                {inputType === "password" ? MonkeyClosedIcon : MonkeyOpenIcon}
              </button>
            </div>
          </div>

          <button type="submit">Login</button>
          <button
            type="button"
            className="register-button"
            onClick={() => navigate("/register")}
          >
            Register
          </button>
        </form>
      </div>
    </div>
  );
}

export default Login;
