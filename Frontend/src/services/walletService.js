// src/services/walletService.js
import { fetchWithAuth } from './api';
 
export function getWalletBalance() {
  return fetchWithAuth('/wallet/balance');
}
 
export function depositFunds(amount) {
  return fetchWithAuth('/wallet/deposit', {
    method: 'POST',
    body: JSON.stringify(Number(amount)),
  });
}
 
export async function getWalletMovements() {
  const data = await fetchWithAuth('/wallet/movements');
  return Array.isArray(data) ? data : [];
}
