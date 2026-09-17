// src/services/auctionService.js
import { fetchWithAuth, buildQuery } from './api';
import { CATALOG_PAGE_SIZE } from '../config';
 
// Los filtros se resuelven en el backend (GET /api/auctions acepta status, categoryId, minPrice,
// maxPrice, sort, page y pageSize) para no descargar todo el listado y filtrarlo en el navegador.
export async function getAuctions({ status, categoryId, minPrice, maxPrice, sort, page = 1, pageSize = CATALOG_PAGE_SIZE } = {}) {
  const data = await fetchWithAuth(`/auctions${buildQuery({ status, categoryId, minPrice, maxPrice, sort, page, pageSize })}`);
  return {
    items: Array.isArray(data?.items) ? data.items : [],
    page: data?.page ?? page,
    pageSize: data?.pageSize ?? pageSize,
    totalCount: data?.totalCount ?? 0,
  };
}
 
export async function getCategories() {
  const data = await fetchWithAuth('/categories');
  return Array.isArray(data) ? data : [];
}
