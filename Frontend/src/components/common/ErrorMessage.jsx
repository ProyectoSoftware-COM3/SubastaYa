// src/components/common/ErrorMessage.jsx
 
// Se muestra cuando una carga falla, para no confundir un error con una lista vacía.
export default function ErrorMessage({ message, onRetry }) {
  return (
    <div className="p-6 border border-red-900/60 bg-red-950/30 rounded-lg text-center" role="alert">
      <p className="text-red-300 text-sm">{message || 'No se pudieron cargar los datos.'}</p>
      {onRetry && (
        <button
          type="button"
          onClick={onRetry}
          className="mt-4 px-4 py-2 text-sm border border-red-400 text-red-200 rounded hover:bg-red-900/40 transition-colors"
        >
          Reintentar
        </button>
      )}
    </div>
  );
}
