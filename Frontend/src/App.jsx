import { Routes, Route, Navigate } from 'react-router-dom';
import { useAuth } from './hooks/useAuth';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';

function App() {
  const { isAuthenticated, logout } = useAuth();

  return (
    <div className="min-h-screen bg-black text-white font-sans">
      <Routes>
        <Route 
          path="/" 
          element={
            isAuthenticated ? (
              <div className="min-h-screen flex flex-col items-center justify-center gap-4">
                <p className="text-xl">Sesión iniciada correctamente.</p>
                <button
                  onClick={logout}
                  className="px-4 py-2 rounded bg-[#d4af37] text-black font-semibold"
                >
                  Cerrar sesión
                </button>
              </div>
            ) : (
              <Navigate to="/login" />
            )
          } 
        />
        
        <Route 
          path="/login" 
          element={!isAuthenticated ? <LoginPage /> : <Navigate to="/" />} 
        />
        
        <Route 
          path="/register" 
          element={!isAuthenticated ? <RegisterPage /> : <Navigate to="/" />} 
        />

        {/* Cualquier ruta inexistente vuelve al inicio en lugar de mostrar una pantalla en blanco. */}
        <Route path="*" element={<Navigate to="/" />} />
      </Routes>
    </div>
  );
}

export default App;
