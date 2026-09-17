// src/pages/HowItWorksPage.jsx
import Navbar from '../components/layout/Navbar';
 
export default function HowItWorksPage() {
  return (
    <div className="min-h-screen bg-[#050505]">
      <Navbar />
      
      <main className="max-w-4xl mx-auto px-4 sm:px-8 py-8 sm:py-12">
        <h1 className="text-3xl font-serif font-bold text-[#d4af37] mb-8">Cómo Funciona SubastaYa</h1>
 
        <div className="flex flex-col gap-6 text-gray-300">
          
          <div className="bg-[#0a0a0a] border border-gray-800 p-6 rounded-xl">
            <h2 className="text-xl font-bold text-white mb-2">1. Sistema de Pujas en Vivo</h2>
            <p>Cada artículo cuenta con un temporizador en tiempo real. Puedes realizar ofertas superando la puja actual respetando el incremento mínimo establecido por el vendedor. La sala se actualiza sola cuando otro usuario oferta y te indica si vas liderando o si te superaron.</p>
          </div>
 
          <div className="bg-[#0a0a0a] border border-gray-800 p-6 rounded-xl">
            <h2 className="text-xl font-bold text-white mb-2">2. Regla de Juego Limpio</h2>
            <p>Para evitar que otros usuarios ganen en el último milisegundo de forma desleal, si se efectúa una oferta válida en los <strong>últimos 60 segundos</strong>, la subasta se extiende automáticamente por <strong>2 minutos adicionales</strong>.</p>
          </div>
 
          <div className="bg-[#0a0a0a] border border-gray-800 p-6 rounded-xl">
            <h2 className="text-xl font-bold text-white mb-2">3. Billetera Virtual y Fondos Retenidos</h2>
            <p>Para garantizar transacciones seguras, cuando realizas la oferta más alta el monto ofertado queda retenido en garantía. Si otro usuario te supera, ese saldo vuelve a estar disponible automáticamente. Solo el <strong>Saldo Disponible</strong> (Saldo Total menos Saldo Retenido) puede usarse para nuevas pujas.</p>
          </div>
 
          <div className="bg-[#0a0a0a] border border-gray-800 p-6 rounded-xl">
            <h2 className="text-xl font-bold text-white mb-2">4. Cierre de la Subasta</h2>
            <p>Al vencer el tiempo, el sistema adjudica el artículo a la oferta más alta y transfiere el saldo retenido al vendedor. Si nadie ofertó, la subasta queda <strong>desierta</strong>.</p>
          </div>
 
        </div>
      </main>
    </div>
  );
}
