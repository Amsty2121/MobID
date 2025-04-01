import axios from "axios";

const api = axios.create({
  baseURL: "http://localhost:5287/api", // Adresa backendului
  headers: {
    "Content-Type": "application/json",
  },
});

// Interceptor pentru token JWT (dacă îl folosești)
api.interceptors.request.use((config) => {
  const token = localStorage.getItem("jwtToken");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default api;
