// src/utils/formatters.js

import { parseApiDate } from './dates';

 
// Un único formato de moneda para toda la app.
export function formatCurrency(value) {
  const number = Number(value ?? 0);
  return `$${number.toLocaleString('es-AR', { minimumFractionDigits: 0, maximumFractionDigits: 2 })}`;
}
 

export function formatDateTime(value) {
  const date = parseApiDate(value);
  if (!date) return 'A confirmar';
  return `${date.toLocaleDateString('es-AR', { day: '2-digit', month: '2-digit', year: 'numeric' })} ${date.toLocaleTimeString('es-AR', { hour: '2-digit', minute: '2-digit', hour12: false })} hs`;
}

