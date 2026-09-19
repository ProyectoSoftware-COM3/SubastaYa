import { useState } from 'react';

function isHttpUrl(value) {
  if (!value) return false;
  try {
    const url = new URL(value);
    return url.protocol === 'http:' || url.protocol === 'https:';
  } catch {
    return false;
  }
}


export default function AuctionImage({ src, alt, className = '', fit = 'cover' }) {
  const [failedSrc, setFailedSrc] = useState(null);
  const canShowImage = isHttpUrl(src) && failedSrc !== src;

  if (!canShowImage) {
    return (
      <div className={`bg-white/5 flex flex-col items-center justify-center gap-2 text-gray-600 ${className}`}>
        <svg width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" aria-hidden="true">
          <rect x="3" y="4" width="18" height="16" rx="2" />
          <circle cx="9" cy="10" r="2" />
          <path d="M21 16l-5-5-8 8" />
        </svg>
        <span className="text-[10px] uppercase tracking-widest">Imagen no disponible</span>
      </div>
    );
  }

  return (
    <img
      src={src}
      alt={alt || 'Imagen de subasta'}
      onError={() => setFailedSrc(src)}
      className={`${fit === 'contain' ? 'object-contain' : 'object-cover'} ${className}`}
    />
  );
}