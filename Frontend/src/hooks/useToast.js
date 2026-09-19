// src/hooks/useToast.js
import { useContext } from 'react';
import { ToastContext } from '../context/contexts';

export function useToast() {
  const context = useContext(ToastContext);
  if (!context) {
    throw new Error('useToast debe usarse dentro de un ToastProvider');
  }
  return context;
}
