// src/pages/ActiveAuctionsPage.jsx
import { useState } from 'react';
import Navbar from '../components/layout/Navbar';
import AuctionCard from '../components/common/AuctionCard';
import Spinner from '../components/common/Spinner';
import ErrorMessage from '../components/common/ErrorMessage';
import Pagination from '../components/common/Pagination';
import { getAuctions } from '../services/auctionService';
import { useAsyncData } from '../hooks/useAsyncData';
import { AUCTION_STATUS, SORT_OPTIONS } from '../utils/auctionStatus';
import { CATALOG_PAGE_SIZE } from '../config';
 
export default function ActiveAuctionsPage() {
  const [page, setPage] = useState(1);
  const [sort, setSort] = useState(SORT_OPTIONS[0].value);
 
  
  const auctions = useAsyncData(
    () => getAuctions({ status: AUCTION_STATUS.ACTIVE, sort, page, pageSize: CATALOG_PAGE_SIZE }),
    `${sort}-${page}`
  );
 
  const result = auctions.data;
  const items = result?.items ?? [];
 
  return (
    <div className="min-h-screen bg-[#050505] text-white">
      <Navbar />
      <main className="max-w-7xl mx-auto px-4 sm:px-8 py-8 sm:py-12">
        <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-8">
          <h1 className="text-3xl font-serif font-bold text-[#d4af37]">Subastas Activas en Curso</h1>
          <div className="flex items-center gap-2">
            <label htmlFor="active-sort" className="text-sm text-gray-400">Ordenar por</label>
            <select
              id="active-sort"
              value={sort}
              onChange={(e) => { setSort(e.target.value); setPage(1); }}
              className="px-2 py-2 bg-black border border-gray-700 rounded text-sm text-white focus:outline-none focus:border-[#d4af37]"
            >
              {SORT_OPTIONS.map((option) => (
                <option key={option.value} value={option.value}>{option.label}</option>
              ))}
            </select>
          </div>
        </div>
 
        {auctions.isLoading && !result ? (
          <Spinner label="Cargando subastas..." className="py-12" />
        ) : auctions.error ? (
          <ErrorMessage message={auctions.error.message} onRetry={auctions.reload} />
        ) : items.length > 0 ? (
          <>
            <div className={`grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6 transition-opacity ${auctions.isLoading ? 'opacity-50' : ''}`}>
              {items.map((auction) => (
                <AuctionCard key={auction.id} auction={auction} />
              ))}
            </div>
            <Pagination
              page={page}
              pageSize={CATALOG_PAGE_SIZE}
              totalCount={result.totalCount}
              onPageChange={setPage}
              disabled={auctions.isLoading}
            />
          </>
        ) : (
          <div className="p-8 border border-dashed border-gray-800 rounded-lg text-center">
            <p className="text-gray-500">No hay subastas activas en este momento.</p>
          </div>
        )}
      </main>
    </div>
  );
}
