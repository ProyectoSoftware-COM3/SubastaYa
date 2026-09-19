// src/services/api.js
import { API_URL } from '../config';

export const AUTH_EXPIRED_EVENT = 'auth:expired';

export class ApiError extends Error {
  constructor(message, status, fieldErrors = {}) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.fieldErrors = fieldErrors;
  }
}

const DEFAULT_MESSAGES = {
  400: 'Los datos enviados no son válidos.',
  401: 'Tu sesión no es válida. Iniciá sesión nuevamente.',
  403: 'No tenés permiso para realizar esta acción.',
  404: 'El recurso solicitado no existe.',
  409: 'Otro usuario modificó este recurso al mismo tiempo. Volvé a intentar.',
  422: 'La operación no se pudo completar.',
  500: 'Ocurrió un error inesperado en el servidor.',
};

function toCamelCase(field) {
  return field ? field.charAt(0).toLowerCase() + field.slice(1) : field;
}

function buildApiError(status, payload) {
  const fieldErrors = {};
  const validationMessages = [];

  if (Array.isArray(payload?.errors)) {
    payload.errors.forEach((validationError) => {
      const field = toCamelCase(validationError.field);
      if (field && !fieldErrors[field]) fieldErrors[field] = validationError.message;
      validationMessages.push(validationError.message);
    });
  }

  const message = payload?.error
    || (validationMessages.length > 0 ? validationMessages.join(' ') : null)
    || DEFAULT_MESSAGES[status]
    || `Error en la petición (${status}).`;

  return new ApiError(message, status, fieldErrors);
}

async function readBody(response) {
  const text = await response.text();
  if (!text) return null;
  try {
    return JSON.parse(text);
  } catch {
    return null;
  }
}

export function buildQuery(params) {
  const searchParams = new URLSearchParams();
  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== '') searchParams.append(key, value);
  });
  const query = searchParams.toString();
  return query ? `?${query}` : '';
}

export async function fetchWithAuth(endpoint, options = {}) {
  const token = localStorage.getItem('token');

  const headers = {
    'Content-Type': 'application/json',
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
    ...options.headers,
  };

  let response;
  try {
    response = await fetch(`${API_URL}${endpoint}`, { ...options, headers });
  } catch {
    throw new ApiError('No se pudo conectar con el servidor. Verificá que el backend esté levantado.', 0);
  }

  const payload = await readBody(response);

  if (!response.ok) {
    // Solo se considera sesión vencida si se envió un token: un 401 en el login significa credenciales inválidas.
    if (response.status === 401 && token) {
      window.dispatchEvent(new Event(AUTH_EXPIRED_EVENT));
    }
    throw buildApiError(response.status, payload);
  }

  return payload;
}
