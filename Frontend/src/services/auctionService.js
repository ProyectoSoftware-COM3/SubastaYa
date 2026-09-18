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
 
export function getAuctionById(id) {
  return fetchWithAuth(`/auctions/${id}`);
}
 
export async function getCategories() {
  const data = await fetchWithAuth('/categories');
  return Array.isArray(data) ? data : [];
}
 
export function createAuction(auctionData) {
  return fetchWithAuth('/auctions', {
    method: 'POST',
    body: JSON.stringify(auctionData),
  });
}
 

export function placeBid(auctionId, amount) {
  return fetchWithAuth(`/auctions/${auctionId}/bids`, {
    method: 'POST',
    body: JSON.stringify(Number(amount)),
  });
}

