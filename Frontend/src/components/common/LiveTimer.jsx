// src/components/common/LiveTimer.jsx
import { formatRemaining } from '../../utils/dates';
 
const CRITICAL_THRESHOLD_MS = 60000;
 
export default function LiveTimer({ targetTimestamp, now, label = 'QUEDAN', expiredText = 'FINALIZADO', size = 'sm' }) {
  if (targetTimestamp === null || targetTimestamp === undefined) return null;
 
  const remaining = targetTimestamp - now;
  const isExpired = remaining <= 0;
  // Consigna Módulo 3: en el último minuto el temporizador cambia de color para alertar a los postores.
  const isCritical = !isExpired && remaining <= CRITICAL_THRESHOLD_MS;
 
  const colorClass = isExpired
    ? 'bg-gray-900 border-gray-700 text-gray-400'
    : isCritical
      ? 'bg-red-500/20 border-red-500 text-red-400 animate-pulse'
      : 'bg-[#1a1a1a] border-[#d4af37]/30 text-[#d4af37]';
 
  const sizeClass = size === 'lg' ? 'text-lg px-4 py-3' : 'text-xs px-2 py-1.5';
 
  return (
    <div className={`font-bold rounded text-center border transition-colors duration-300 ${sizeClass} ${colorClass}`}>
      {isExpired ? expiredText : `${label}: ${formatRemaining(remaining)}`}
      {isCritical && size === 'lg' && <span className="block text-xs font-semibold mt-1">¡Último minuto!</span>}
    </div>
  );
}
