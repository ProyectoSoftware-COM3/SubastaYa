// src/components/common/Toast.jsx

const TOAST_STYLES = {
  success: 'bg-emerald-950/95 border-emerald-500 text-emerald-100',
  error: 'bg-red-950/95 border-red-500 text-red-100',
  warning: 'bg-amber-950/95 border-[#d4af37] text-amber-100',
  info: 'bg-[#111111]/95 border-gray-600 text-gray-100',
};

const TOAST_ICONS = {
  success: '✓',
  error: '✕',
  warning: '!',
  info: 'i',
};

export default function Toast({ toasts, onDismiss }) {
  return (
    <div
      className="fixed top-20 right-4 left-4 sm:left-auto sm:w-96 z-50 flex flex-col gap-3 pointer-events-none"
      aria-live="polite"
      aria-atomic="false"
    >
      {toasts.map((toast) => (
        <div
          key={toast.id}
          role={toast.type === 'error' ? 'alert' : 'status'}
          className={`pointer-events-auto border rounded-lg shadow-2xl px-4 py-3 flex gap-3 items-start ${TOAST_STYLES[toast.type] ?? TOAST_STYLES.info}`}
        >
          <span className="w-6 h-6 shrink-0 rounded-full border border-current flex items-center justify-center text-xs font-bold">
            {TOAST_ICONS[toast.type] ?? TOAST_ICONS.info}
          </span>
          <div className="flex-1 min-w-0">
            {toast.title && <p className="font-bold text-sm">{toast.title}</p>}
            <p className="text-sm leading-snug">{toast.message}</p>
          </div>
          <button
            type="button"
            onClick={() => onDismiss(toast.id)}
            className="text-lg leading-none opacity-70 hover:opacity-100"
            aria-label="Cerrar notificación"
          >
            ×
          </button>
        </div>
      ))}
    </div>
  );
}
