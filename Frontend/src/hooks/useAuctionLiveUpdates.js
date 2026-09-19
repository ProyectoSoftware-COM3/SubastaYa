// src/hooks/useAuctionLiveUpdates.js
import { useEffect, useRef, useState } from 'react';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { AUCTION_HUB_URL, POLLING_INTERVAL_MS } from '../config';
 
export const LIVE_MODE = {
  CONNECTING: 'connecting',
  LIVE: 'live',
  POLLING: 'polling',
};
 

export function useAuctionLiveUpdates(auctionId, onChange) {
  const [mode, setMode] = useState(LIVE_MODE.CONNECTING);
  const onChangeRef = useRef(onChange);
 
  useEffect(() => {
    onChangeRef.current = onChange;
  });
 
  useEffect(() => {
    if (!auctionId) return undefined;
 
    let disposed = false;
    let pollingId = null;
 
    const notifyChange = (reason) => {
      if (!disposed) onChangeRef.current(reason);
    };
 
    const startPolling = () => {
      if (disposed || pollingId) return;
      setMode(LIVE_MODE.POLLING);
      pollingId = setInterval(() => notifyChange('polling'), POLLING_INTERVAL_MS);
    };
 
    const stopPolling = () => {
      if (pollingId) {
        clearInterval(pollingId);
        pollingId = null;
      }
    };
 
    const connection = new HubConnectionBuilder()
      
      .withUrl(AUCTION_HUB_URL, { withCredentials: false })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.None)
      .build();
 
    connection.on('NewBid', () => notifyChange('NewBid'));
    connection.on('TimeExtended', () => notifyChange('TimeExtended'));
    connection.on('AuctionClosed', () => notifyChange('AuctionClosed'));
 
    connection.onreconnecting(() => startPolling());
    connection.onreconnected(() => {
      connection.invoke('JoinAuctionRoom', auctionId)
        .then(() => {
          if (disposed) return;
          stopPolling();
          setMode(LIVE_MODE.LIVE);
          notifyChange('reconnected');
        })
        .catch(() => startPolling());
    });
    connection.onclose(() => startPolling());
 
    connection.start()
      .then(() => connection.invoke('JoinAuctionRoom', auctionId))
      .then(() => {
        if (!disposed) setMode(LIVE_MODE.LIVE);
      })
      .catch(() => startPolling());
 
    return () => {
      disposed = true;
      stopPolling();
      connection.invoke('LeaveAuctionRoom', auctionId)
        .catch(() => {})
        .finally(() => connection.stop().catch(() => {}));
    };
  }, [auctionId]);
 
  return mode;
}
