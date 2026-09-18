// src/pages/WalletPage.jsx
import { useState } from 'react';
import { Link } from 'react-router-dom';
import Navbar from '../components/layout/Navbar';
import Spinner from '../components/common/Spinner';
import ErrorMessage from '../components/common/ErrorMessage';
import { getWalletBalance, depositFunds, getWalletMovements } from '../services/walletService';
import { useAsyncData } from '../hooks/useAsyncData';
import { useToast } from '../hooks/useToast';
import { formatCurrency, formatDateTime } from '../utils/formatters';
 
// Consigna Módulo 4: ingresos, retenciones por ofertas, liberaciones por superación y débitos por subastas ganadas.
const MOVEMENT_TYPES = {
  Deposit: { label: 'Ingreso', detail: 'Acreditación de saldo', effect: '+ Total y disponible', style: 'text-emerald-400 border-emerald-500/50 bg-emerald-950/40' },
  Hold: { label: 'Retención', detail: 'Oferta realizada', effect: 'Disponible → Retenido', style: 'text-red-300 border-red-500/50 bg-red-950/40' },
  Release: { label: 'Liberación', detail: 'Tu oferta fue superada', effect: 'Retenido → Disponible', style: 'text-blue-300 border-blue-500/50 bg-blue-950/40' },
  Payment: { label: 'Débito', detail: 'Subasta ganada', effect: '− Total (sale del retenido)', style: 'text-orange-300 border-orange-500/50 bg-orange-950/40' },
  Collection: { label: 'Cobro', detail: 'Subasta vendida', effect: '+ Total y disponible', style: 'text-[#d4af37] border-[#d4af37]/50 bg-yellow-950/40' },
};
 
function BalanceValue({ isLoading, value, className }) {
  if (isLoading) return <Spinner label="" size="sm" className="justify-start py-2" />;
  return <span className={`text-3xl font-bold ${className}`}>{formatCurrency(value)}</span>;
}
 
function MovementsTable({ movements }) {
  if (movements.length === 0) {
    return <p className="text-gray-500 text-sm">Todavía no hay movimientos en tu billetera.</p>;
  }
 
  return (
    <div className="overflow-x-auto">
    <table className="w-full text-sm min-w-140">
        <thead>
          <tr className="text-left text-xs uppercase tracking-wider text-gray-500 border-b border-gray-800">
            <th className="py-2 pr-4 font-semibold">Fecha</th>
            <th className="py-2 pr-4 font-semibold">Tipo</th>
            <th className="py-2 pr-4 font-semibold">Detalle</th>
            <th className="py-2 pr-4 font-semibold text-right">Monto</th>
            <th className="py-2 font-semibold text-right">Subasta</th>
          </tr>
        </thead>
        <tbody>
          {movements.map((movement) => {
            const type = MOVEMENT_TYPES[movement.type] ?? { label: movement.type, detail: '', effect: '', style: 'text-gray-300 border-gray-600' };
            return (
              <tr key={movement.id} className="border-b border-gray-900">
                <td className="py-3 pr-4 text-gray-400 whitespace-nowrap">{formatDateTime(movement.occurredAt)}</td>
                <td className="py-3 pr-4">
                  <span className={`inline-block px-2 py-0.5 border rounded text-xs font-bold ${type.style}`}>{type.label}</span>
                </td>
                <td className="py-3 pr-4">
                  <span className="text-gray-200 block">{type.detail}</span>
                  <span className="text-xs text-gray-500">{type.effect}</span>
                </td>
                <td className="py-3 pr-4 text-right font-bold text-white whitespace-nowrap">{formatCurrency(movement.amount)}</td>
                <td className="py-3 text-right">
                  {movement.auctionId
                    ? <Link to={`/auction/${movement.auctionId}`} className="text-xs text-[#d4af37] hover:underline">Ver subasta</Link>
                    : <span className="text-xs text-gray-600">—</span>}
                </td>
              </tr>
            );
          })}
        </tbody>
      </table>
    </div>
  );
}
 
export default function WalletPage() {
  const toast = useToast();
  const balance = useAsyncData(getWalletBalance, 'balance');
  const movements = useAsyncData(getWalletMovements, 'movements');
 
  const [depositAmount, setDepositAmount] = useState('');
  const [depositError, setDepositError] = useState('');
  const [submitting, setSubmitting] = useState(false);
 
  const wallet = balance.data ?? { totalBalance: 0, heldBalance: 0, availableBalance: 0 };
  const isBalanceLoading = balance.isLoading && !balance.data;
 
  const handleDeposit = async (e) => {
    e.preventDefault();
    if (submitting) return;
 
    const amount = Number(depositAmount);
    if (depositAmount === '' || !Number.isFinite(amount) || amount <= 0) {
      setDepositError('Ingresá un monto mayor a cero.');
      return;
    }
 
    setDepositError('');
    setSubmitting(true);
    try {
      const updated = await depositFunds(amount);
      // El backend devuelve el saldo actualizado; si no llegara, se vuelve a consultar.
      if (updated) balance.setData(updated);
      else balance.reload();
      movements.reload();
      setDepositAmount('');
      toast.success(`Se acreditaron ${formatCurrency(amount)} en tu billetera.`, 'Depósito exitoso');
    } catch (err) {
      toast.error(err.message, 'No se pudo acreditar el saldo');
    } finally {
      setSubmitting(false);
    }
  };
 
  return (
    <div className="min-h-screen bg-[#050505] text-white">
      <Navbar />
      <main className="max-w-5xl mx-auto px-4 sm:px-8 py-8 sm:py-12">
        <h1 className="text-3xl font-serif font-bold text-[#d4af37] mb-8">Mi Billetera Virtual</h1>
 
        {balance.error && !balance.data ? (
          <div className="mb-10">
            <ErrorMessage message={balance.error.message} onRetry={balance.reload} />
          </div>
        ) : (
          <>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-3">
              <div className="bg-[#0a0a0a] border border-gray-800 p-6 rounded-xl">
                <span className="text-xs text-gray-400 uppercase tracking-wider block mb-2">Saldo Total</span>
                <BalanceValue isLoading={isBalanceLoading} value={wallet.totalBalance} className="text-white" />
              </div>
 
              <div className="bg-[#0a0a0a] border border-red-900/40 p-6 rounded-xl">
                <span className="text-xs text-red-400 uppercase tracking-wider block mb-2">Saldo Retenido (En Garantía)</span>
                <BalanceValue isLoading={isBalanceLoading} value={wallet.heldBalance} className="text-red-400" />
              </div>
 
              <div className="bg-[#0a0a0a] border border-[#d4af37]/40 p-6 rounded-xl">
                <span className="text-xs text-[#d4af37] uppercase tracking-wider block mb-2">Saldo Disponible</span>
                <BalanceValue isLoading={isBalanceLoading} value={wallet.availableBalance} className="text-[#d4af37]" />
              </div>
            </div>
            <p className="text-xs text-gray-500 mb-10">
              Saldo Disponible = Saldo Total − Saldo Retenido. Solo el disponible puede usarse para nuevas pujas.
            </p>
          </>
        )}
 
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <div className="bg-[#0a0a0a] border border-gray-800 p-6 rounded-xl h-fit">
            <h2 className="text-lg font-bold text-white mb-4">Acreditar Fondos</h2>
 
            <form onSubmit={handleDeposit} className="flex flex-col gap-4" noValidate>
              <div>
                <label htmlFor="deposit-amount" className="text-xs text-gray-400 block mb-1">Monto a cargar</label>
                <input
                  id="deposit-amount"
                  type="number"
                  min="1"
                  step="any"
                  value={depositAmount}
                  onChange={(e) => { setDepositAmount(e.target.value); setDepositError(''); }}
                  placeholder="Ej: 50000"
                  disabled={submitting}
                  className={`w-full bg-[#121212] border rounded px-4 py-2.5 text-white focus:outline-none ${depositError ? 'border-red-500' : 'border-gray-800 focus:border-[#d4af37]'}`}
                />
                {depositError && <p className="text-red-400 text-xs mt-1">{depositError}</p>}
              </div>
              <button
                type="submit"
                disabled={submitting}
                className="w-full py-3 bg-[#d4af37] text-black font-bold rounded hover:bg-[#c49a2e] transition-colors uppercase text-sm tracking-wide disabled:opacity-50 flex items-center justify-center gap-2"
              >
                {submitting ? <><Spinner label="" size="sm" /> Acreditando...</> : 'Cargar Fondos'}
              </button>
            </form>
          </div>
 
          <div className="lg:col-span-2 bg-[#0a0a0a] border border-gray-800 p-6 rounded-xl">
            <div className="flex items-center justify-between mb-4">
              <h2 className="text-lg font-bold text-white">Historial de Movimientos</h2>
              {movements.isLoading && movements.data && <Spinner label="" size="sm" />}
            </div>
 
            {movements.isLoading && !movements.data ? (
              <Spinner label="Cargando movimientos..." className="py-8" />
            ) : movements.error ? (
              <ErrorMessage message={movements.error.message} onRetry={movements.reload} />
            ) : (
              <MovementsTable movements={movements.data ?? []} />
            )}
          </div>
        </div>
      </main>
    </div>
  );
}
