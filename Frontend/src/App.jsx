import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';

function App() {
  return (
    <Router>
      <div className="min-h-screen bg-brand-black text-white flex flex-col">
        {/* Acá irá el Navbar más adelante */}
        
        <main className="flex-1 flex items-center justify-center p-8">
          <Routes>
            <Route 
              path="/" 
              element={
                <div className="text-center">
                  <h1 className="text-4xl text-brand-gold font-bold mb-4">Setup Inicial Completado 🚀</h1>
                  <p className="text-gray-400 mb-8">El enrutador, Tailwind y los estilos globales están listos.</p>
                  <button className="btn-lor">
                    <span>Botón de prueba</span>
                    <svg className="w-5 h-5" fill="none" stroke="currentColor" strokeWidth="2.5" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" d="M14 5l7 7m0 0l-7 7m7-7H3" />
                    </svg>
                  </button>
                </div>
              } 
            />
          </Routes>
        </main>
      </div>
    </Router>
  );
}

export default App;