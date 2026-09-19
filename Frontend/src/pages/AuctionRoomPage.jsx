// src/pages/AuctionRoomPage.jsx
import { useCallback, useEffect, useRef, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import Navbar from '../components/layout/Navbar';
import LiveTimer from '../components/common/LiveTimer';
import AuctionImage from '../components/common/AuctionImage';
import StatusBadge from '../components/common/StatusBadge';
import Spinner from '../components/common/Spinner';
import ErrorMessage from '../components/common/ErrorMessage';
import { getAuctionById, placeBid } from '../services/auctionService';
import { getWalletBalance } from '../services/walletService';
import { useToast } from '../hooks/useToast';
import { useNow } from '../hooks/useNow';
import { useAuctionLiveUpdates, LIVE_MODE } from '../hooks/useAuctionLiveUpdates';
import { POLLING_INTERVAL_MS } from '../config';
import { AUCTION_STATUS, isBiddingOpen } from '../utils/auctionStatus';
import { toTimestamp } from '../utils/dates';
import { formatBidTime, formatCurrency, formatDateTime } from '../utils/formatters';
 

const EXTENSION_TOLERANCE_MS = 1000;
 

export default function AuctionRoomPage() {
  const { id } = useParams();
  return <AuctionRoom key={id} auctionId={id} />;
}
 
function LiveModeIndicator({ mode }) {
  const presentation = {
    [LIVE_MODE.LIVE]: { dot: 'bg-green-500', text: 'En vivo' },
    [LIVE_MODE.POLLING]: { dot: 'bg-yellow-500', text: 'Actualización automática cada 3 s' },
    [LIVE_MODE.CONNECTING]: { dot: 'bg-gray-500', text: 'Conectando...' },
  }[mode];
 
  return (
    <span className="inline-flex items-center gap-2 text-xs text-gray-400">
      <span className={`w-2 h-2 rounded-full ${presentation.dot} ${mode === LIVE_MODE.LIVE ? 'animate-pulse' : ''}`} />
      {presentation.text}
    </span>
  );
}
 

function LeadingIndicator({ isCurrentUserLeading, status }) {
  if (isCurrentUserLeading === null || isCurrentUserLeading === undefined) {
    return <p className="text-xs text-gray-500">Todavía no ofertaste en esta subasta.</p>;
  }
 
  if (status === AUCTION_STATUS.FINISHED) {
    return isCurrentUserLeading
      ? <span className="inline-block px-3 py-1 bg-green-900/80 border border-green-500 text-green-200 text-xs font-bold rounded">¡GANASTE ESTA SUBASTA!</span>
      : <span className="inline-block px-3 py-1 bg-gray-800 border border-gray-600 text-gray-300 text-xs font-bold rounded">NO GANASTE</span>;
  }
 
  return isCurrentUserLeading
    ? <span className="inline-block px-3 py-1 bg-green-900/80 border border-green-500 text-green-200 text-xs font-bold rounded">LIDERANDO</span>
    : <span className="inline-block px-3 py-1 bg-red-900/80 border border-red-500 text-red-200 text-xs font-bold rounded">SUPERADO</span>;
}
 
function ClosedBiddingMessage({ auction, startTimestamp, endTimestamp, now }) {
  if (auction.status === AUCTION_STATUS.SCHEDULED) {
    return (
      <div className="mt-6 p-4 bg-blue-950/40 border border-blue-500/40 rounded-lg text-center">
        <p className="text-sm font-bold text-blue-200">Las pujas todavía no están habilitadas</p>
        <p className="text-xs text-gray-400 mt-1">La subasta comienza el {formatDateTime(auction.startDate)}.</p>
        <div className="mt-3">
          <LiveTimer targetTimestamp={startTimestamp} now={now} label="COMIENZA EN" expiredText="ESPERANDO HABILITACIÓN" />
        </div>
      </div>
    );
  }
 
  if (auction.status === AUCTION_STATUS.FINISHED) {
    return (
      <div className="mt-6 p-4 bg-gray-900 border border-gray-700 rounded-lg text-center">
        <p className="text-sm font-bold text-gray-200">Subasta finalizada</p>
        <p className="text-xs text-gray-400 mt-1">Adjudicada por {formatCurrency(auction.currentPrice)}.</p>
      </div>
    );
  }
 
  if (auction.status === AUCTION_STATUS.UNSOLD) {
    return (
      <div className="mt-6 p-4 bg-red-950/30 border border-red-900/60 rounded-lg text-center">
        <p className="text-sm font-bold text-red-200">Subasta desierta</p>
        <p className="text-xs text-gray-400 mt-1">Venció sin ofertas.</p>
      </div>
    );
  }
 
  
  if (endTimestamp !== null && endTimestamp <= now) {
    return (
      <div className="mt-6 p-4 bg-gray-900 border border-gray-700 rounded-lg flex items-center justify-center gap-3">
        <Spinner label="El tiempo terminó. Cerrando la subasta..." size="sm" />
      </div>
    );
  }
 
  return null;
}
 
function BidHistory({ bids }) {
  
  const orderedBids = [...bids].sort((first, second) => (toTimestamp(second.placedAt) ?? 0) - (toTimestamp(first.placedAt) ?? 0));
 
  return (
    <div className="bg-[#0a0a0a] border border-gray-800 rounded-lg p-4">
      <h3 className="font-bold text-sm uppercase text-gray-400 mb-3">Historial de Ofertas ({bids.length})</h3>
      <div className="max-h-64 overflow-y-auto flex flex-col">
        {orderedBids.length > 0 ? (
          orderedBids.map((bid, position) => (
            <div key={bid.id} className={`grid grid-cols-3 items-center text-sm border-b border-gray-800 py-2 ${position === 0 ? 'text-white' : 'text-gray-400'}`}>
              <span className="truncate">
                {bid.bidderAlias}
                {position === 0 && <span className="ml-2 text-[10px] text-[#d4af37] font-bold uppercase">Más alta</span>}
              </span>
              <span className="text-center text-xs font-mono">{formatBidTime(bid.placedAt)}</span>
              <span className="text-right text-[#d4af37] font-bold">{formatCurrency(bid.amount)}</span>
            </div>
          ))
        ) : (
          <p className="text-gray-500 text-xs">Sin ofertas registradas todavía.</p>
        )}
      </div>
    </div>
  );
}
 
function AuctionRoom({ auctionId }) {
  const toast = useToast();
  const now = useNow(1000);
 
  const [auction, setAuction] = useState(null);
  const [loadError, setLoadError] = useState(null);
  const [wallet, setWallet] = useState(null);
  const [bidAmount, setBidAmount] = useState('');
  const [bidError, setBidError] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
 
  const lastAuctionRef = useRef(null);
  const isRefreshingRef = useRef(false);
  const hasPendingRefreshRef = useRef(false);
 
  const refreshWallet = useCallback(() => {
    getWalletBalance().then(setWallet).catch(() => setWallet(null));
  }, []);
 

  const applyAuctionUpdate = useCallback((next) => {
    const previous = lastAuctionRef.current;
 
    if (previous) {
      const previousEnd = toTimestamp(previous.endDate);
      const nextEnd = toTimestamp(next.endDate);
 
      if (next.status === AUCTION_STATUS.ACTIVE && previousEnd !== null && nextEnd !== null && nextEnd - previousEnd > EXTENSION_TOLERANCE_MS) {
        toast.warning(`Se registró una oferta en el último minuto. Nuevo cierre: ${formatDateTime(next.endDate)}.`, 'Tiempo extendido (+2 min)');
      }
 
      if (previous.isCurrentUserLeading === true && next.isCurrentUserLeading === false && next.status === AUCTION_STATUS.ACTIVE) {
        toast.error(`Otro usuario ofertó ${formatCurrency(next.currentPrice)}. Tu saldo retenido fue liberado.`, 'Te superaron');
      }
 
      if (previous.status === AUCTION_STATUS.ACTIVE && next.status !== AUCTION_STATUS.ACTIVE) {
        if (next.status === AUCTION_STATUS.UNSOLD) toast.info('La subasta terminó sin ofertas.', 'Subasta desierta');
        else if (next.isCurrentUserLeading) toast.success(`Ganaste la subasta por ${formatCurrency(next.currentPrice)}.`, '¡Felicitaciones!');
        else toast.info(`Se adjudicó por ${formatCurrency(next.currentPrice)}.`, 'Subasta finalizada');
      }
 
      if (previous.isCurrentUserLeading !== next.isCurrentUserLeading) refreshWallet();
    }
 
    lastAuctionRef.current = next;
    setAuction(next);
    setLoadError(null);
  }, [toast, refreshWallet]);
 
  const refreshAuction = useCallback(async () => {
    
    if (isRefreshingRef.current) {
      hasPendingRefreshRef.current = true;
      return;
    }
 
    isRefreshingRef.current = true;
    try {
      do {
        hasPendingRefreshRef.current = false;
        try {
          applyAuctionUpdate(await getAuctionById(auctionId));
        } catch (error) {
          if (!lastAuctionRef.current) setLoadError(error);
        }
      } while (hasPendingRefreshRef.current);
    } finally {
      isRefreshingRef.current = false;
    }
  }, [auctionId, applyAuctionUpdate]);
 
  useEffect(() => {
    let isCurrent = true;
    getAuctionById(auctionId)
      .then((data) => { if (isCurrent) applyAuctionUpdate(data); })
      .catch((error) => { if (isCurrent) setLoadError(error); });
    getWalletBalance()
      .then((data) => { if (isCurrent) setWallet(data); })
      .catch(() => {});
    return () => { isCurrent = false; };
   
  }, [auctionId]);
 
  const liveMode = useAuctionLiveUpdates(auctionId, refreshAuction);
 
  const endTimestamp = toTimestamp(auction?.endDate);
  const startTimestamp = toTimestamp(auction?.startDate);
  const canBid = isBiddingOpen(auction, endTimestamp, now);
 
  
  const hasExpiredLocally = auction?.status === AUCTION_STATUS.ACTIVE && endTimestamp !== null && endTimestamp <= now;
  useEffect(() => {
    if (hasExpiredLocally) refreshAuction();
  }, [hasExpiredLocally, refreshAuction]);
  
  const isWaitingActivation = auction?.status === AUCTION_STATUS.SCHEDULED && startTimestamp !== null && startTimestamp <= now;
  useEffect(() => {
    if (!isWaitingActivation) return undefined;

    const intervalId = setInterval(() => { refreshAuction(); }, POLLING_INTERVAL_MS);
    return () => clearInterval(intervalId);
  }, [isWaitingActivation, refreshAuction]);
 
  if (loadError && !auction) {
    return (
      <div className="min-h-screen bg-[#050505] text-white">
        <Navbar />
        <main className="max-w-3xl mx-auto px-4 sm:px-8 py-12">
          <ErrorMessage
            message={loadError.status === 404 ? 'La subasta no existe.' : loadError.message}
            onRetry={loadError.status === 404 ? undefined : refreshAuction}
          />
          <Link to="/" className="block text-center mt-6 text-sm text-[#d4af37] hover:underline">Volver al catálogo</Link>
        </main>
      </div>
    );
  }
 
  if (!auction) {
    return (
      <div className="min-h-screen bg-[#050505] text-white">
        <Navbar />
        <Spinner label="Cargando sala..." className="py-24" />
      </div>
    );
  }
 
  const suggestedNextBid = auction.suggestedNextBid ?? auction.currentPrice + auction.minIncrement;
  const availableBalance = wallet?.availableBalance ?? null;
 
  // Valida antes de llamar al backend las mismas reglas que PlaceBidCommandHandler (monto mínimo y saldo disponible).
  const validateBid = (amount) => {
    if (!Number.isFinite(amount) || amount <= 0) return 'Ingresá un monto válido.';
    if (amount < suggestedNextBid) return `La oferta mínima es ${formatCurrency(suggestedNextBid)}.`;
    if (availableBalance !== null && amount > availableBalance) return `Fondos insuficientes: tu saldo disponible es ${formatCurrency(availableBalance)}.`;
    return '';
  };
 
  const submitBid = async (amount) => {
    if (isSubmitting || !canBid) return;
 
  const validationError = validateBid(amount);
    setBidError(validationError);
    if (validationError) {
      if (validationError.startsWith('Fondos insuficientes')) {
        toast.error(validationError, 'Fondos insuficientes');
      }
      return;
    }
 
    setIsSubmitting(true);
    try {
      await placeBid(auctionId, amount);
      toast.success(`Tu oferta de ${formatCurrency(amount)} fue registrada. El monto quedó retenido en garantía.`, 'Puja confirmada');
      setBidAmount('');
      await refreshAuction();
      refreshWallet();
    } catch (error) {
      if (error.status === 422) {
        toast.error(error.message, 'Fondos insuficientes');
        refreshWallet();
      } else if (error.status === 409) {
        toast.warning('Otra oferta se registró al mismo tiempo. Actualizamos la sala: revisá el nuevo monto mínimo.', 'Conflicto de concurrencia');
        await refreshAuction();
      } else if (error.status === 400) {
        toast.error(error.message, 'Oferta rechazada');
        await refreshAuction();
      } else {
        toast.error(error.message, 'No se pudo registrar la oferta');
      }
    } finally {
      setIsSubmitting(false);
    }
  };
 
  const handleBidSubmit = (e) => {
    e.preventDefault();
    submitBid(Number(bidAmount));
  };
 
  return (
    <div className="min-h-screen bg-[#050505] text-white">
      <Navbar />
 
      <main className="max-w-6xl mx-auto px-4 sm:px-8 py-8 sm:py-12 grid grid-cols-1 md:grid-cols-2 gap-8 md:gap-12">
        <div className="flex flex-col gap-6">
          <div className="bg-[#0a0a0a] border border-gray-800 rounded-xl overflow-hidden h-80 sm:h-96">
            <AuctionImage src={auction.imageUrl} alt={auction.title} className="w-full h-full" fit="contain" />
          </div>
 
          <BidHistory bids={auction.bidHistory ?? []} />
        </div>
 
        <div className="flex flex-col gap-6">
          <div>
            <div className="flex flex-wrap items-center justify-between gap-3">
              <div className="flex items-center gap-3">
                <span className="text-xs uppercase tracking-widest text-[#d4af37] font-bold">
                  {auction.categoryName ?? 'Sin categoría'}
                </span>
                <StatusBadge status={auction.status} />
              </div>
              {auction.status === AUCTION_STATUS.ACTIVE && <LiveModeIndicator mode={liveMode} />}
            </div>
            <h1 className="text-3xl font-serif font-bold mt-2">{auction.title}</h1>
            <p className="text-gray-400 mt-2 whitespace-pre-line">{auction.description}</p>
          </div>
 
          {auction.status === AUCTION_STATUS.ACTIVE && (
            <LiveTimer targetTimestamp={endTimestamp} now={now} size="lg" expiredText="TIEMPO TERMINADO" />
          )}
 
          <div className="bg-[#0a0a0a] p-6 rounded-lg border border-gray-800">
            <div className="flex flex-wrap items-start justify-between gap-3">
              <div>
                <p className="text-sm text-gray-400">
                  {auction.bidHistory?.length ? 'Oferta más alta:' : 'Precio base:'}
                </p>
                <p className="text-4xl font-bold text-[#d4af37] my-2">{formatCurrency(auction.currentPrice)}</p>
              </div>
              <LeadingIndicator isCurrentUserLeading={auction.isCurrentUserLeading} status={auction.status} />
            </div>
 
            <div className="grid grid-cols-2 gap-2 text-xs text-gray-500 mt-2">
              <p>Incremento mínimo: <span className="text-gray-300">{formatCurrency(auction.minIncrement)}</span></p>
              <p className="text-right">Cierre: <span className="text-gray-300">{formatDateTime(auction.endDate)}</span></p>
            </div>
 
            {canBid ? (
              <div className="mt-6">
                <button
                  type="button"
                  onClick={() => submitBid(suggestedNextBid)}
                  disabled={isSubmitting}
                  className="w-full py-3 bg-[#d4af37] text-black font-bold rounded hover:bg-[#c49a2e] transition-colors disabled:opacity-60 disabled:cursor-not-allowed flex items-center justify-center gap-2"
                >
                  {isSubmitting ? <><Spinner label="" size="sm" /> Enviando oferta...</> : `OFERTAR ${formatCurrency(suggestedNextBid)} (sugerido)`}
                </button>
 
                <form onSubmit={handleBidSubmit} className="mt-4 flex flex-col sm:flex-row gap-3" noValidate>
                  <label htmlFor="custom-bid" className="sr-only">Monto personalizado</label>
                  <input
                    id="custom-bid"
                    type="number"
                    min={suggestedNextBid}
                    step="any"
                    placeholder={`Otro monto (mín. ${formatCurrency(suggestedNextBid)})`}
                    value={bidAmount}
                    onChange={(e) => { setBidAmount(e.target.value); setBidError(''); }}
                    disabled={isSubmitting}
                    className={`flex-1 p-3 bg-black border rounded text-white font-bold focus:outline-none ${bidError ? 'border-red-500' : 'border-gray-700 focus:border-[#d4af37]'}`}
                  />
                  <button
                    type="submit"
                    disabled={isSubmitting || bidAmount === ''}
                    className="px-6 py-3 border border-[#d4af37] text-[#d4af37] font-bold rounded hover:bg-[#d4af37] hover:text-black transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    OFERTAR
                  </button>
                </form>
 
                {bidError && <p className="text-red-400 text-xs mt-2" role="alert">{bidError}</p>}
 
                <p className="text-xs text-gray-500 mt-3">
                  {availableBalance !== null
                    ? <>Saldo disponible: <span className="text-gray-300 font-semibold">{formatCurrency(availableBalance)}</span> · </>
                    : null}
                  <Link to="/wallet" className="text-[#d4af37] hover:underline">Cargar saldo</Link>
                </p>
              </div>
            ) : (
              <ClosedBiddingMessage auction={auction} startTimestamp={startTimestamp} endTimestamp={endTimestamp} now={now} />
            )}
          </div>
        </div>
      </main>
    </div>
  );
}
