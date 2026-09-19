// src/pages/HomePage.jsx
import { useState } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import Navbar from '../components/layout/Navbar';
import AuctionCard from '../components/common/AuctionCard';
import Spinner from '../components/common/Spinner';
import ErrorMessage from '../components/common/ErrorMessage';
import Pagination from '../components/common/Pagination';
import { getAuctions, getCategories } from '../services/auctionService';
import { useAsyncData } from '../hooks/useAsyncData';
import { STATUS_FILTER_OPTIONS, SORT_OPTIONS } from '../utils/auctionStatus';
import { CATALOG_PAGE_SIZE } from '../config';
 
const DEFAULT_SORT = SORT_OPTIONS[0].value;
 
const filterButtonClass = (isSelected) =>
  `text-left px-2 py-1.5 rounded transition-colors ${isSelected ? 'bg-[#d4af37] text-black font-bold' : 'text-gray-300 hover:text-white'}`;
 
function readFilters(searchParams) {
  return {
    status: searchParams.get('status') ?? '',
    categoryId: searchParams.get('categoryId') ?? '',
    minPrice: searchParams.get('minPrice') ?? '',
    maxPrice: searchParams.get('maxPrice') ?? '',
    sort: searchParams.get('sort') ?? DEFAULT_SORT,
    page: Math.max(1, Number(searchParams.get('page')) || 1),
  };
}
 
function validatePriceRange(minPrice, maxPrice) {
  const min = minPrice === '' ? null : Number(minPrice);
  const max = maxPrice === '' ? null : Number(maxPrice);
  if ((min !== null && min < 0) || (max !== null && max < 0)) return 'Los precios no pueden ser negativos.';
  if (min !== null && max !== null && max < min) return 'El precio máximo debe ser mayor o igual al mínimo.';
  return '';
}
 
export default function HomePage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const filters = readFilters(searchParams);
 
  const [priceInputs, setPriceInputs] = useState({ minPrice: filters.minPrice, maxPrice: filters.maxPrice });
  const [priceError, setPriceError] = useState('');
 
  const auctions = useAsyncData(() => getAuctions({ ...filters, pageSize: CATALOG_PAGE_SIZE }), searchParams.toString());
  const categories = useAsyncData(getCategories, 'categories');
 
  const updateFilters = (changes, { resetPage = true } = {}) => {
    const next = { ...filters, ...changes };
    if (resetPage) next.page = 1;
 
    const params = {};
    Object.entries(next).forEach(([key, value]) => {
      const isDefault = (key === 'sort' && value === DEFAULT_SORT) || (key === 'page' && value === 1);
      if (value !== '' && value !== null && !isDefault) params[key] = String(value);
    });
    setSearchParams(params);
  };
 
  const applyPriceRange = (e) => {
    e.preventDefault();
    const error = validatePriceRange(priceInputs.minPrice, priceInputs.maxPrice);
    setPriceError(error);
    if (!error) updateFilters(priceInputs);
  };
 
  const clearFilters = () => {
    setPriceInputs({ minPrice: '', maxPrice: '' });
    setPriceError('');
    setSearchParams({});
  };
 
  const page = auctions.data;
  const items = page?.items ?? [];
 
  return (
    <div className="min-h-screen bg-[#050505]">
      <Navbar />
 
      <main className="max-w-7xl mx-auto px-4 sm:px-8 py-8 sm:py-12">
        <div className="relative rounded-2xl overflow-hidden border border-gray-800 mb-12 shadow-2xl">
          
          <div className="absolute inset-0 z-0">
            <img 
              src="/hero-luxury.png" 
              alt="Fondo SubastaYa" 
              className="w-full h-full object-cover object-center filter brightness-75 scale-105"
            />
            <div className="absolute inset-0 bg-gradient-to-r from-[#07080a] via-[#07080a]/90 to-[#07080a]/50"></div>
          </div>
 
          <div className="relative z-10 p-8 sm:p-14 max-w-2xl flex flex-col gap-6">
            <h1 className="text-4xl sm:text-5xl lg:text-6xl font-serif font-bold text-[#d4af37] leading-tight drop-shadow-md">
              Descubre Tesoros Exclusivos.<br />
              <span className="text-white">Puja y Gana.</span>
            </h1>
 
            <p className="text-gray-200 text-base sm:text-lg drop-shadow">
              Subastas en tiempo real con saldo en garantía.<br />
              Tu próxima adquisición está a un clic.
            </p>
 
            <div className="pt-2">
              <Link to="/active-auctions" className="btn-lor">
                <span>Ver Subastas Activas</span>
                <svg className="w-5 h-5" fill="none" stroke="currentColor" strokeWidth="2.5" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M14 5l7 7m0 0l-7 7m7-7H3" />
                </svg>
              </Link>
            </div>
          </div>
        </div>
 
        {/*  SECCIÓN DE CATÁLOGO (Filtros + Grilla)*/}
        <div className="flex flex-col md:flex-row gap-8">
          <aside className="w-full md:w-64 flex-shrink-0">
            <div className="border border-gray-800 rounded-lg bg-[#0a0a0a] p-4 flex flex-col gap-6 md:sticky md:top-4">
              <section>
                <h3 className="text-white font-bold border-b border-gray-800 pb-2 mb-2">Estado</h3>
                <div className="flex flex-col gap-1 text-sm">
                  {STATUS_FILTER_OPTIONS.map((option) => (
                    <button
                      key={option.value || 'all'}
                      type="button"
                      onClick={() => updateFilters({ status: option.value })}
                      className={filterButtonClass(filters.status === option.value)}
                    >
                      {option.label}
                    </button>
                  ))}
                </div>
              </section>
 
              <section>
                <h3 className="text-white font-bold border-b border-gray-800 pb-2 mb-2">Categoría</h3>
                {categories.isLoading ? (
                  <Spinner label="" size="sm" className="justify-start py-2" />
                ) : categories.error ? (
                  <button type="button" onClick={categories.reload} className="text-xs text-red-300 underline">
                    No se pudieron cargar. Reintentar
                  </button>
                ) : (
                  <div className="flex flex-col gap-1 text-sm">
                    <button type="button" onClick={() => updateFilters({ categoryId: '' })} className={filterButtonClass(filters.categoryId === '')}>
                      Todas las categorías
                    </button>
                    {(categories.data ?? []).map((category) => (
                      <button
                        key={category.id}
                        type="button"
                        onClick={() => updateFilters({ categoryId: category.id })}
                        className={filterButtonClass(filters.categoryId === category.id)}
                      >
                        {category.name}
                      </button>
                    ))}
                  </div>
                )}
              </section>
 
              <section>
                <h3 className="text-white font-bold border-b border-gray-800 pb-2 mb-2">Rango de precios</h3>
                <form onSubmit={applyPriceRange} className="flex flex-col gap-2" noValidate>
                  <div className="flex gap-2">
                    <input
                      type="number"
                      min="0"
                      placeholder="Mín."
                      aria-label="Precio mínimo"
                      value={priceInputs.minPrice}
                      onChange={(e) => setPriceInputs((current) => ({ ...current, minPrice: e.target.value }))}
                      className="w-1/2 px-2 py-1.5 bg-black border border-gray-700 rounded text-sm text-white focus:outline-none focus:border-[#d4af37]"
                    />
                    <input
                      type="number"
                      min="0"
                      placeholder="Máx."
                      aria-label="Precio máximo"
                      value={priceInputs.maxPrice}
                      onChange={(e) => setPriceInputs((current) => ({ ...current, maxPrice: e.target.value }))}
                      className="w-1/2 px-2 py-1.5 bg-black border border-gray-700 rounded text-sm text-white focus:outline-none focus:border-[#d4af37]"
                    />
                  </div>
                  {priceError && <p className="text-red-400 text-xs">{priceError}</p>}
                  <button type="submit" className="py-1.5 text-sm border border-[#d4af37] text-[#d4af37] rounded hover:bg-[#d4af37] hover:text-black transition-colors">
                    Aplicar
                  </button>
                </form>
              </section>
 
              <section>
                <label htmlFor="catalog-sort" className="text-white font-bold border-b border-gray-800 pb-2 mb-2 block">Ordenar por</label>
                <select
                  id="catalog-sort"
                  value={filters.sort}
                  onChange={(e) => updateFilters({ sort: e.target.value })}
                  className="w-full px-2 py-2 bg-black border border-gray-700 rounded text-sm text-white focus:outline-none focus:border-[#d4af37]"
                >
                  {SORT_OPTIONS.map((option) => (
                    <option key={option.value} value={option.value}>{option.label}</option>
                  ))}
                </select>
              </section>
 
              <button type="button" onClick={clearFilters} className="text-xs text-gray-400 hover:text-white underline self-start">
                Limpiar filtros
              </button>
            </div>
          </aside>
 
          {/* Grilla de Tarjetas */}
          <div className="flex-1 min-w-0">
            {auctions.isLoading && !page ? (
              <Spinner label="Cargando subastas..." className="py-12" />
            ) : auctions.error ? (
              <ErrorMessage message={auctions.error.message} onRetry={auctions.reload} />
            ) : (
              <>
                <div className="flex items-center justify-between mb-4 text-sm text-gray-400">
                  <span>{page?.totalCount ?? 0} {page?.totalCount === 1 ? 'subasta encontrada' : 'subastas encontradas'}</span>
                  {auctions.isLoading && <Spinner label="Actualizando..." size="sm" />}
                </div>
 
                {items.length > 0 ? (
                  <div className={`grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6 transition-opacity ${auctions.isLoading ? 'opacity-50' : ''}`}>
                    {items.map((auction) => (
                      <AuctionCard key={auction.id} auction={auction} />
                    ))}
                  </div>
                ) : (
                  <div className="p-8 border border-dashed border-gray-800 rounded-lg text-center">
                    <p className="text-gray-500">No hay subastas que coincidan con los filtros.</p>
                  </div>
                )}
 
                <Pagination
                  page={filters.page}
                  pageSize={CATALOG_PAGE_SIZE}
                  totalCount={page?.totalCount ?? 0}
                  onPageChange={(nextPage) => updateFilters({ page: nextPage }, { resetPage: false })}
                  disabled={auctions.isLoading}
                />
              </>
            )}
          </div>
        </div>
      </main>
    </div>
  );
}
