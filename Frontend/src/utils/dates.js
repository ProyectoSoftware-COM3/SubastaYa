// src/utils/dates.js
 
const TIMEZONE_SUFFIX = /([zZ]|[+-]\d{2}:?\d{2})$/;
 
// El backend guarda las fechas en UTC, pero las columnas datetime2 no conservan la zona horaria
// y la API puede devolverlas sin la "Z". Sin esta normalización, el navegador las tomaría como
// hora local y en Argentina todos los contadores quedarían corridos 3 horas.
export function parseApiDate(value) {
  if (!value) return null;
  if (value instanceof Date) return value;
 
  const text = String(value);
  const date = new Date(TIMEZONE_SUFFIX.test(text) ? text : `${text}Z`);
  return Number.isNaN(date.getTime()) ? null : date;
}
 
export function toTimestamp(value) {
  return parseApiDate(value)?.getTime() ?? null;
}
 
export function formatRemaining(milliseconds) {
  const totalSeconds = Math.max(0, Math.floor(milliseconds / 1000));
  const days = Math.floor(totalSeconds / 86400);
  const hours = Math.floor((totalSeconds % 86400) / 3600);
  const minutes = Math.floor((totalSeconds % 3600) / 60);
  const seconds = totalSeconds % 60;
  const pad = (number) => String(number).padStart(2, '0');
 
  if (days > 0) return `${days}d ${pad(hours)}h ${pad(minutes)}m`;
  return `${pad(hours)}h ${pad(minutes)}m ${pad(seconds)}s`;
}
