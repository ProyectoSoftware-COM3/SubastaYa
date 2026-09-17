// src/config.js
const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL || 'https://localhost:7080').replace(/\/$/, '');

export const API_URL = `${API_BASE_URL}/api`;
export const AUCTION_HUB_URL = `${API_BASE_URL}/hubs/auctions`;

// Consigna 3.1: el short-polling es la alternativa mínima aceptable si WebSockets no está disponible.
export const POLLING_INTERVAL_MS = 3000;

export const CATALOG_PAGE_SIZE = 12;
