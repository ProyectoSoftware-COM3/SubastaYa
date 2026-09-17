// src/services/auctionService.js
import { fetchWithAuth, buildQuery } from './api';
import { CATALOG_PAGE_SIZE } from '../config';
 

export async function getAuctions({ status, categoryId, minPrice, maxPrice, sort, page = 1, pageSize = CATALOG_PAGE_SIZE } = {}) {
  const data = await fetchWithAuth(`/auctions${buildQuery({ status, categoryId, minPrice, maxPrice, sort, page, pageSize })}`);
  return {
    items: Array.isArray(data?.items) ? data.items : [],
    page: data?.page ?? page,
    pageSize: data?.pageSize ?? pageSize,
    totalCount: data?.totalCount ?? 0,
  };
}
 
//Se agrega getAuctionById (GET /api/auctions/{id}, lo usa Próximas Subastas).
export function getAuctionById(id) {
  return fetchWithAuth(`/auctions/${id}`);
}

 
export async function getCategories() {
  const data = await fetchWithAuth('/categories');
  return Array.isArray(data) ? data : [];
}
