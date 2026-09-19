// src/context/AuthContext.jsx
import { useCallback, useEffect, useMemo, useState } from 'react';
import { AuthContext } from './contexts';
import { AUTH_EXPIRED_EVENT } from '../services/api';
import { decodeToken, getTokenExpiration, userFromClaims } from '../utils/jwt';
import { useToast } from '../hooks/useToast';

function clearStoredSession() {
  localStorage.removeItem('user');
  localStorage.removeItem('token');
}

function readStoredUser() {
  const savedUser = localStorage.getItem('user');
  if (!savedUser) return null;
  try {
    const parsed = JSON.parse(savedUser);
    return parsed && typeof parsed === 'object' ? parsed : null;
  } catch {
    return null;
  }
}

function readStoredSession() {
  const token = localStorage.getItem('token');
  const claims = decodeToken(token);
  const expiresAt = getTokenExpiration(claims);

  if (!token || !claims || (expiresAt && expiresAt <= Date.now())) {
    clearStoredSession();
    return { token: null, user: null };
  }

  return { token, user: readStoredUser() ?? userFromClaims(claims) };
}

export function AuthProvider({ children }) {
  const [session, setSession] = useState(readStoredSession);
  const toast = useToast();

  const loginContext = useCallback((userData, authToken) => {
    localStorage.setItem('user', JSON.stringify(userData));
    localStorage.setItem('token', authToken);
    setSession({ token: authToken, user: userData });
  }, []);

  const logout = useCallback(() => {
    clearStoredSession();
    setSession({ token: null, user: null });
  }, []);

  useEffect(() => {
    if (!session.token) return undefined;

    const expireSession = () => {
      logout();
      toast.warning('Tu sesión venció. Iniciá sesión nuevamente.', 'Sesión expirada');
    };

    window.addEventListener(AUTH_EXPIRED_EVENT, expireSession);

    const expiresAt = getTokenExpiration(decodeToken(session.token));
    const timeoutId = expiresAt ? setTimeout(expireSession, Math.max(0, expiresAt - Date.now())) : null;

    return () => {
      window.removeEventListener(AUTH_EXPIRED_EVENT, expireSession);
      if (timeoutId) clearTimeout(timeoutId);
    };
  }, [session.token, logout, toast]);

  const value = useMemo(() => ({
    user: session.user,
    token: session.token,
    isAuthenticated: Boolean(session.token),
    loginContext,
    logout,
  }), [session, loginContext, logout]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
