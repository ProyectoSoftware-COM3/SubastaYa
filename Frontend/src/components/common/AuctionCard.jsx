// src/components/common/AuctionCard.jsx
import { Link } from 'react-router-dom';
import LiveTimer from './LiveTimer';
import AuctionImage from './AuctionImage';
import StatusBadge from './StatusBadge';
import { useNow } from '../../hooks/useNow';
import { toTimestamp } from '../../utils/dates';
import { formatCurrency } from '../../utils/formatters';
import { AUCTION_STATUS, isBiddingOpen } from '../../utils/auctionStatus';
 
function getPriceLabel(status, bidCount) {
  if (status === AUCTION_STATUS.FINISHED) return 'Precio final';
  if (status === AUCTION_STATUS.UNSOLD) return 'Precio base (sin ofertas)';
  if (status === AUCTION_STATUS.SCHEDULED || bidCount === 0) return 'Precio base';
  return 'Oferta más alta';
}
 
function getFooterText(status) {
  if (status === AUCTION_STATUS.SCHEDULED) return 'Las pujas se habilitan al comenzar';
  if (status === AUCTION_STATUS.FINISHED) return 'Subasta finalizada';
  if (status === AUCTION_STATUS.UNSOLD) return 'Subasta desierta';
  return 'Cerrando subasta...';
}
 
export default function AuctionCard({ auction }) {
  const now = useNow(1000);
 
  if (!auction) return null;
 
  const { id, title, imageUrl, categoryName, currentPrice, bidCount = 0, endDate, status } = auction;
  const endTimestamp = toTimestamp(endDate);
  const canBid = isBiddingOpen(auction, endTimestamp, now);
 
  return (
    <article className="bg-[#0a0a0a] border border-gray-800 rounded-xl overflow-hidden flex flex-col justify-between hover:border-gray-700 transition-colors">
      <div>
        <div className="relative h-52 border-b border-gray-800 overflow-hidden">
          <AuctionImage src={imageUrl} alt={title} className="h-full w-full" />
          <StatusBadge status={status} className="absolute top-3 left-3 backdrop-blur" />
        </div>
 
        <div className="p-4">
          {status === AUCTION_STATUS.ACTIVE && (
            <div className="mb-3">
              <LiveTimer targetTimestamp={endTimestamp} now={now} />
            </div>
          )}
 
          {categoryName && (
            <span className="text-[10px] tracking-widest uppercase font-bold text-[#d4af37] block mb-1">
              {categoryName}
            </span>
          )}
 
          <h3 className="font-bold text-base text-white truncate" title={title}>{title}</h3>
        </div>
      </div>
 
      <div className="p-4 pt-0">
        <div className="mb-3 flex items-end justify-between gap-2">
          <div>
            <span className="text-[10px] text-gray-500 uppercase tracking-wider block">
              {getPriceLabel(status, bidCount)}
            </span>
            <span className="text-2xl font-bold text-[#d4af37]">{formatCurrency(currentPrice)}</span>
          </div>
          <span className="text-xs text-gray-400 whitespace-nowrap">
            {bidCount} {bidCount === 1 ? 'oferta' : 'ofertas'}
          </span>
        </div>
 
        {canBid ? (
          <Link
            to={`/auction/${id}`}
            className="block w-full py-2.5 font-bold rounded uppercase text-sm tracking-wide text-center bg-[#d4af37] text-black hover:bg-[#c49a2e] transition-colors"
          >
            Pujar
          </Link>
        ) : (
          <>
            <p className="text-[11px] text-gray-500 text-center mb-2">{getFooterText(status)}</p>
            <Link
              to={`/auction/${id}`}
              className="block w-full py-2.5 font-bold rounded uppercase text-sm tracking-wide text-center border border-gray-700 text-gray-300 hover:border-[#d4af37] hover:text-[#d4af37] transition-colors"
            >
              Ver detalle
            </Link>
          </>
        )}
      </div>
    </article>
  );
}
