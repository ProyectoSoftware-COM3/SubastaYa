// src/components/common/Pagination.jsx
 
export default function Pagination({ page, pageSize, totalCount, onPageChange, disabled = false }) {
  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
  if (totalPages <= 1) return null;
 
  const buttonClass = 'px-4 py-2 text-sm border border-gray-700 rounded text-gray-300 hover:border-[#d4af37] hover:text-[#d4af37] transition-colors disabled:opacity-40 disabled:cursor-not-allowed disabled:hover:border-gray-700 disabled:hover:text-gray-300';
 
  return (
    <nav className="flex items-center justify-center gap-4 mt-8" aria-label="Paginación">
      <button type="button" className={buttonClass} onClick={() => onPageChange(page - 1)} disabled={disabled || page <= 1}>
        Anterior
      </button>
      <span className="text-sm text-gray-400">
        Página <span className="text-white font-semibold">{page}</span> de {totalPages}
      </span>
      <button type="button" className={buttonClass} onClick={() => onPageChange(page + 1)} disabled={disabled || page >= totalPages}>
        Siguiente
      </button>
    </nav>
  );
}
