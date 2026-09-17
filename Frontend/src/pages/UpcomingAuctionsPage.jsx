// src/pages/UpcomingAuctionsPage.jsx
import { Link } from 'react-router-dom';
import Navbar from '../components/layout/Navbar';
import AuctionImage from '../components/common/AuctionImage';
import LiveTimer from '../components/common/LiveTimer';
import Spinner from '../components/common/Spinner';
import ErrorMessage from '../components/common/ErrorMessage';
import { getAuctions, getAuctionById } from '../services/auctionService';
import { useAsyncData } from '../hooks/useAsyncData';
import { useNow } from '../hooks/useNow';
import { AUCTION_STATUS } from '../utils/auctionStatus';
import { toTimestamp } from '../utils/dates';
import { formatCurrency, formatDateTime } from '../utils/formatters';
 
const MAX_UPCOMING = 50;
 

async function loadUpcomingAuctions() {
  const { items } = await getAuctions({ status: AUCTION_STATUS.SCHEDULED, pageSize: MAX_UPCOMING });
  return Promise.all(
    items.map((item) => getAuctionById(item.id).then((detail) => ({ ...item, ...detail })).catch(() => item))
  );
}
 
export default function UpcomingAuctionsPage() {
  const upcoming = useAsyncData(loadUpcomingAuctions, 'upcoming');
  const now = useNow(1000);
  const items = upcoming.data ?? [];
 
  return (
    <div className="min-h-screen bg-[#050505] text-white">
      <Navbar />
      <main className="max-w-5xl mx-auto px-4 sm:px-8 py-8 sm:py-12">
        <h1 className="text-3xl font-serif font-bold text-[#d4af37] mb-8">Próximas Subastas</h1>
 
        {upcoming.isLoading && !upcoming.data ? (
          <Spinner label="Cargando próximas subastas..." className="py-12" />
        ) : upcoming.error ? (
          <ErrorMessage message={upcoming.error.message} onRetry={upcoming.reload} />
        ) : items.length > 0 ? (
          <div className="flex flex-col gap-4">
            {items.map((item) => {
              const startTimestamp = toTimestamp(item.startDate);
 
              return (
                <article
                  key={item.id}
                  className="bg-[#0a0a0a] border border-gray-800 p-5 rounded-lg flex flex-col sm:flex-row sm:items-center justify-between gap-6 hover:border-gray-700 transition-colors"
                >
                  <div className="flex items-center gap-5 min-w-0">
                    <div className="w-24 h-24 rounded-md overflow-hidden shrink-0 border border-gray-800">
                      <AuctionImage src={item.imageUrl} alt={item.title} className="w-full h-full" />
                    </div>
 
                    <div className="min-w-0">
                      {item.categoryName && (
                        <span className="text-[10px] tracking-wider uppercase font-bold text-[#d4af37]">{item.categoryName}</span>
                      )}
                      <h3 className="font-bold text-lg text-white mt-0.5">{item.title}</h3>
                      {item.description && <p className="text-xs text-gray-400 line-clamp-1 mt-1">{item.description}</p>}
 
                      <div className="flex flex-wrap gap-4 mt-2 text-xs">
                        <p className="text-yellow-500/90 font-medium">
                          Inicio: <span className="text-gray-200">{formatDateTime(item.startDate)}</span>
                        </p>
                        <p className="text-gray-400">
                          Cierre: <span className="text-gray-200">{formatDateTime(item.endDate)}</span>
                        </p>
                      </div>
                    </div>
                  </div>
 
                  <div className="sm:text-right shrink-0 flex flex-col gap-2 sm:items-end">
                    <div>
                      <span className="text-[11px] text-gray-500 uppercase tracking-wider block">Precio base</span>
                      <span className="text-xl font-bold text-[#d4af37]">{formatCurrency(item.basePrice ?? item.currentPrice)}</span>
                    </div>
                    <LiveTimer targetTimestamp={startTimestamp} now={now} label="COMIENZA EN" expiredText="POR COMENZAR" />
                    <Link to={`/auction/${item.id}`} className="text-xs text-gray-300 hover:text-[#d4af37] underline">
                      Ver detalle
                    </Link>
                  </div>
                </article>
              );
            })}
          </div>
        ) : (
          <div className="p-8 border border-dashed border-gray-800 rounded-lg text-center">
            <p className="text-gray-500">No hay subastas programadas por el momento.</p>
          </div>
        )}
      </main>
    </div>
  );
}
