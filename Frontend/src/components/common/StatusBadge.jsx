// src/components/common/StatusBadge.jsx
import { getStatusPresentation } from '../../utils/auctionStatus';
 
export default function StatusBadge({ status, className = '' }) {
  const { label, style } = getStatusPresentation(status);
 
  return (
    <span className={`inline-block px-2.5 py-0.5 border text-[10px] font-bold uppercase tracking-wider rounded ${style} ${className}`}>
      {label}
    </span>
  );
}
