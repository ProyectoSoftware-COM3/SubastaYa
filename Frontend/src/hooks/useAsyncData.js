// src/hooks/useAsyncData.js
import { useCallback, useEffect, useRef, useState } from 'react';
 
export function useAsyncData(loader, key = 'default') {
  const loaderRef = useRef(loader);
  const [reloadCount, setReloadCount] = useState(0);
  const [result, setResult] = useState({ requestKey: null, data: undefined, error: null });
 
  const requestKey = `${key}#${reloadCount}`;
 
  useEffect(() => {
    loaderRef.current = loader;
  });
 
  useEffect(() => {
    let isCurrent = true;
 
    loaderRef.current().then(
      (data) => {
        if (isCurrent) setResult({ requestKey, data, error: null });
      },
      (error) => {
        if (isCurrent) setResult((previous) => ({ requestKey, data: previous.data, error }));
      }
    );
 
    return () => {
      isCurrent = false;
    };
  }, [requestKey]);
 
  const reload = useCallback(() => setReloadCount((count) => count + 1), []);
 
  // Permite reemplazar los datos con la respuesta de una operación (por ejemplo, el saldo devuelto por un depósito).
  const setData = useCallback((data) => {
    setResult((previous) => ({ ...previous, data, error: null }));
  }, []);
 
  const isLoading = result.requestKey !== requestKey;
 
  return {
    data: result.data,
    error: isLoading ? null : result.error,
    isLoading,
    reload,
    setData,
  };
}
