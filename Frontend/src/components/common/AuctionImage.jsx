import { useState } from 'react';
 
const getFallbackImage = (title, originalUrl) => {
  if (originalUrl && originalUrl.startsWith('http')) return originalUrl;
 
  const lower = title?.toLowerCase() || '';
  
  if (lower.includes('reloj')) return 'https://cdn.pixabay.com/photo/2018/02/24/20/39/clock-3179167_1280.jpg';
  if (lower.includes('teclado')) return 'https://images.unsplash.com/photo-1587829741301-dc798b83add3?q=80&w=600&auto=format&fit=crop';
  if (lower.includes('bicicleta')) return 'https://images.unsplash.com/photo-1485965120184-e220f721d03e?q=80&w=600&auto=format&fit=crop';
  if (lower.includes('figura')) return 'https://images.unsplash.com/photo-1607604276583-eef5d076aa5f?q=80&w=600&auto=format&fit=crop';
  if (lower.includes('notebook')) return 'https://images.unsplash.com/photo-1603302576837-37561b2e2302?q=80&w=600&auto=format&fit=crop';
  if (lower.includes('campera')) return 'https://images.unsplash.com/photo-1551028719-0c1d988641eb?q=80&w=600&auto=format&fit=crop';
  if (lower.includes('zapatilla')) return 'https://images.unsplash.com/photo-1542291026-7eec264c27ff?q=80&w=600&auto=format&fit=crop';
  if (lower.includes('silla')) return 'https://cdn20.pamono.com/p/g/1/3/1382340_fpvw77c8ua/silla-windsor-inglesa-antigua-de-olmo-imagen-3.jpg';
 
 
  return 'https://images.unsplash.com/photo-1589829085413-56de8ae18c73?q=80&w=600&auto=format&fit=crop';
};
 
export default function AuctionImage({ src, alt, className = '', fit = 'cover' }) {
  const [imgSrc, setImgSrc] = useState(getFallbackImage(alt, src));
 
  const handleError = () => {
    if (imgSrc !== getFallbackImage('', '')) {
      setImgSrc(getFallbackImage('', ''));
    }
  };
 
  return (
    <img
      src={imgSrc}
      alt={alt || 'Imagen de subasta'}
      onError={handleError}
      className={`object-${fit} ${className}`}
    />
  );
}
