// src/components/common/Spinner.jsx

export default function Spinner({ label = 'Cargando...', size = 'md', className = '' }) {
  const sizeClass = size === 'sm' ? 'w-4 h-4 border-2' : 'w-8 h-8 border-4';

  return (
    <div className={`flex items-center justify-center gap-3 text-gray-400 ${className}`} role="status">
      <span className={`${sizeClass} border-[#d4af37]/30 border-t-[#d4af37] rounded-full animate-spin`} />
      {label && <span className="text-sm">{label}</span>}
    </div>
  );
}
