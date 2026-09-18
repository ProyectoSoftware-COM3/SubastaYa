// src/pages/MyActivitiesPage.jsx
import { useState } from 'react';
import { Link } from 'react-router-dom';
import Navbar from '../components/layout/Navbar';
import StatusBadge from '../components/common/StatusBadge';
import Spinner from '../components/common/Spinner';
import ErrorMessage from '../components/common/ErrorMessage';
import { getMyBids, getMyAuctions } from '../services/activityService';
import { useAsyncData } from '../hooks/useAsyncData';
import { AUCTION_STATUS } from '../utils/auctionStatus';
import { formatCurrency } from '../utils/formatters';
 
const TABS = [
  { id: 'bids', label: 'Mis Compras / Pujas' },
  { id: 'auctions', label: 'Mis Publicaciones' },
];
 

function getBidResult(bid) {
  if (bid.won) return { label: '¡GANASTE!', style: 'bg-green-900/80 border-green-500 text-green-200', detail: 'Subasta cerrada' };
  if (bid.isOpen && bid.isCurrentlyWinning) return { label: 'LIDERANDO', style: 'bg-yellow-900/80 border-yellow-500 text-yellow-200', detail: 'Subasta abierta' };
  if (bid.isOpen) return { label: 'SUPERADO', style: 'bg-red-900/80 border-red-500 text-red-200', detail: 'Subasta abierta: todavía podés ofertar' };
  return { label: 'NO GANASTE', style: 'bg-gray-800 border-gray-600 text-gray-300', detail: 'Subasta cerrada' };
}
 

function getAdjudication(auction) {
  switch (auction.status) {
    case AUCTION_STATUS.FINISHED:
      return <>Adjudicada · Recaudación: <span className="text-[#d4af37] font-bold">{formatCurrency(auction.revenue)}</span></>;
    case AUCTION_STATUS.ACTIVE:
      return 'En curso · sin adjudicar todavía';
    case AUCTION_STATUS.SCHEDULED:
      return 'Todavía no comenzó';
    case AUCTION_STATUS.UNSOLD:
      return 'Desierta · sin adjudicar (recaudación $0)';
    default:
      return '';
  }
}
 
function MyBidsTab() {
  const bids = useAsyncData(getMyBids, 'my-bids');
 
  if (bids.isLoading) return <Spinner label="Cargando tus pujas..." className="py-12" />;
  if (bids.error) return <ErrorMessage message={bids.error.message} onRetry={bids.reload} />;
 
  const items = bids.data ?? [];
  if (items.length === 0) {
    return (
      <div className="p-8 border border-dashed border-gray-800 rounded-lg text-center">
        <p className="text-gray-500">No tienes pujas registradas.</p>
        <Link to="/active-auctions" className="inline-block mt-4 text-sm text-[#d4af37] hover:underline">Ver subastas activas</Link>
      </div>
    );
  }
 
  return (
    <div className="flex flex-col gap-4">
      {items.map((bid) => {
        const result = getBidResult(bid);
        return (
          <Link
            key={bid.auctionId}
            to={`/auction/${bid.auctionId}`}
            className="bg-[#0a0a0a] border border-gray-800 p-5 rounded-lg flex flex-col sm:flex-row sm:justify-between sm:items-center gap-3 hover:border-gray-600 transition-colors"
          >
            <div>
              <h3 className="font-bold text-lg">{bid.auctionTitle}</h3>
              <p className="text-sm text-gray-400">Mi oferta más alta: <span className="text-[#d4af37] font-bold">{formatCurrency(bid.myLastBid)}</span></p>
              <p className="text-xs text-gray-500 mt-1">{result.detail}</p>
            </div>
            <span className={`self-start sm:self-center px-3 py-1 border text-xs font-bold rounded ${result.style}`}>{result.label}</span>
          </Link>
        );
      })}
    </div>
  );
}
 
function MyAuctionsTab() {
  const auctions = useAsyncData(getMyAuctions, 'my-auctions');
 
  if (auctions.isLoading) return <Spinner label="Cargando tus publicaciones..." className="py-12" />;
  if (auctions.error) return <ErrorMessage message={auctions.error.message} onRetry={auctions.reload} />;
 
  const items = auctions.data ?? [];
  if (items.length === 0) {
    return (
      <div className="p-8 border border-dashed border-gray-800 rounded-lg text-center">
        <p className="text-gray-500">No tienes productos puestos en venta.</p>
        <Link to="/create-auction" className="inline-block mt-4 text-sm text-[#d4af37] hover:underline">Publicar una subasta</Link>
      </div>
    );
  }
 
  return (
    <div className="flex flex-col gap-4">
      {items.map((auction) => (
        <Link
          key={auction.id}
          to={`/auction/${auction.id}`}
          className="bg-[#0a0a0a] border border-gray-800 p-5 rounded-lg flex flex-col sm:flex-row sm:justify-between sm:items-center gap-3 hover:border-gray-600 transition-colors"
        >
          <div>
            <h3 className="font-bold text-lg">{auction.title}</h3>
            <p className="text-sm text-gray-400">
              Ofertas recibidas: <span className="text-white font-semibold">{auction.bidCount}</span>
            </p>
            <p className="text-xs text-gray-400 mt-1">{getAdjudication(auction)}</p>
          </div>
          <StatusBadge status={auction.status} className="self-start sm:self-center" />
        </Link>
      ))}
    </div>
  );
}
 
export default function MyActivitiesPage() {
  const [tab, setTab] = useState('bids');
 
  return (
    <div className="min-h-screen bg-[#050505] text-white">
      <Navbar />
      <main className="max-w-5xl mx-auto px-4 sm:px-8 py-8 sm:py-12">
        <h1 className="text-3xl font-serif font-bold text-[#d4af37] mb-8">Mis Actividades</h1>
 
        <div className="flex gap-6 mb-8 border-b border-gray-800 pb-3" role="tablist">
          {TABS.map((item) => (
            <button
              key={item.id}
              type="button"
              role="tab"
              aria-selected={tab === item.id}
              onClick={() => setTab(item.id)}
              className={`pb-2 font-semibold transition-colors ${tab === item.id ? 'text-[#d4af37] border-b-2 border-[#d4af37]' : 'text-gray-400 hover:text-white'}`}
            >
              {item.label}
            </button>
          ))}
        </div>
 
        {tab === 'bids' ? <MyBidsTab /> : <MyAuctionsTab />}
      </main>
    </div>
  );
}
