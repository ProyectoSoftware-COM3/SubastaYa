// src/context/ToastContext.jsx
import { useCallback, useMemo, useRef, useState } from 'react';
import { ToastContext } from './contexts';
import Toast from '../components/common/Toast';

const MAX_VISIBLE_TOASTS = 4;
const DEFAULT_DURATION_MS = 5000;
const ERROR_DURATION_MS = 7000;

export function ToastProvider({ children }) {
  const [toasts, setToasts] = useState([]);
  const nextIdRef = useRef(0);

  const dismiss = useCallback((id) => {
    setToasts((current) => current.filter((toast) => toast.id !== id));
  }, []);

  const show = useCallback((type, message, title) => {
    nextIdRef.current += 1;
    const id = nextIdRef.current;
    setToasts((current) => [...current, { id, type, message, title }].slice(-MAX_VISIBLE_TOASTS));
    setTimeout(() => dismiss(id), type === 'error' ? ERROR_DURATION_MS : DEFAULT_DURATION_MS);
    return id;
  }, [dismiss]);

  const value = useMemo(() => ({
    success: (message, title) => show('success', message, title),
    error: (message, title) => show('error', message, title),
    warning: (message, title) => show('warning', message, title),
    info: (message, title) => show('info', message, title),
    dismiss,
  }), [show, dismiss]);

  return (
    <ToastContext.Provider value={value}>
      {children}
      <Toast toasts={toasts} onDismiss={dismiss} />
    </ToastContext.Provider>
  );
}
