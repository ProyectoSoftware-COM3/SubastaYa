// src/api/authApi.js
import { fetchWithAuth } from '../services/api';

export const loginApi = (credentials) =>
  fetchWithAuth('/sessions', { method: 'POST', body: JSON.stringify(credentials) });

export const registerApi = (userData) =>
  fetchWithAuth('/users', { method: 'POST', body: JSON.stringify(userData) });
