// src/hooks/useNow.js
import { useEffect, useState } from 'react';
 
// Hora actual que se actualiza sola; los contadores y las reglas que dependen del tiempo
// (por ejemplo, bloquear la puja al vencer) leen de acá en lugar de consultar la hora en cada render.
export function useNow(intervalMs = 1000) {
  const [now, setNow] = useState(() => Date.now());
 
  useEffect(() => {
    const intervalId = setInterval(() => setNow(Date.now()), intervalMs);
    return () => clearInterval(intervalId);
  }, [intervalMs]);
 
  return now;
}
