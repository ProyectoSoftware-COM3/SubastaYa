// src/services/activityService.js
import { fetchWithAuth } from './api';
 
export async function getMyBids() {
  const data = await fetchWithAuth('/my-activity/bids');
  return Array.isArray(data) ? data : [];
}
 
export async function getMyAuctions() {
  const data = await fetchWithAuth('/my-activity/auctions');
  return Array.isArray(data) ? data : [];
}
