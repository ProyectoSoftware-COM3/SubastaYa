// src/utils/auctionStatus.js
 
export const AUCTION_STATUS = {
  SCHEDULED: 'Scheduled',
  ACTIVE: 'Active',
  FINISHED: 'Finished',
  UNSOLD: 'Unsold',
};
 
const STATUS_PRESENTATION = {
  Scheduled: { label: 'Próxima', style: 'bg-blue-900/40 border-blue-500/50 text-blue-300' },
  Active: { label: 'Activa', style: 'bg-green-900/40 border-green-500/50 text-green-300' },
  Finished: { label: 'Finalizada', style: 'bg-gray-800 border-gray-600 text-gray-300' },
  Unsold: { label: 'Desierta', style: 'bg-red-900/40 border-red-500/50 text-red-300' },
};
 
export function getStatusPresentation(status) {
  return STATUS_PRESENTATION[status] ?? { label: status ?? 'Sin estado', style: 'bg-yellow-900/40 border-[#d4af37]/50 text-[#d4af37]' };
}
 
// Opciones del filtro de estado del catálogo (Módulo 1). El backend recibe un único estado por consulta,
// por eso Finalizadas (con ganador) y Desiertas (sin ofertas) son opciones separadas.
export const STATUS_FILTER_OPTIONS = [
  { value: '', label: 'Todas' },
  { value: AUCTION_STATUS.ACTIVE, label: 'Activas' },
  { value: AUCTION_STATUS.SCHEDULED, label: 'Próximas' },
  { value: AUCTION_STATUS.FINISHED, label: 'Finalizadas' },
  { value: AUCTION_STATUS.UNSOLD, label: 'Desiertas' },
];
 
export const SORT_OPTIONS = [
  { value: 'LeastTimeRemaining', label: 'Menor tiempo restante' },
  { value: 'HighestBid', label: 'Mayor puja' },
];
 
// Una subasta puede figurar Active con la fecha de fin ya vencida durante los segundos
// que tarda el worker del backend en cerrarla; en ese lapso no se debe permitir ofertar.
export function isBiddingOpen(auction, endTimestamp, now) {
  return auction?.status === AUCTION_STATUS.ACTIVE && endTimestamp !== null && endTimestamp > now;
}
