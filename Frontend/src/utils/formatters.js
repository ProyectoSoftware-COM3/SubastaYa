// src/utils/formatters.js
 

export function formatCurrency(value) {
  const number = Number(value ?? 0);
  return `$${number.toLocaleString('es-AR', { minimumFractionDigits: 0, maximumFractionDigits: 2 })}`;
}
