import { Routes, Route, Navigate } from 'react-router-dom';
import { useAuth } from './hooks/useAuth';
import HomePage from './pages/HomePage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import HowItWorksPage from './pages/HowItWorksPage';
import ActiveAuctionsPage from './pages/ActiveAuctionsPage';
import UpcomingAuctionsPage from './pages/UpcomingAuctionsPage';

 
function App() {
  const { isAuthenticated } = useAuth();
 
  return (
    <div className="min-h-screen bg-black text-white font-sans">
      <Routes>
        <Route 
          path="/" 
          element={isAuthenticated ? <HomePage /> : <Navigate to="/login" />} 
        />
        
        <Route 
          path="/login" 
          element={!isAuthenticated ? <LoginPage /> : <Navigate to="/" />} 
        />
        
        <Route 
          path="/register" 
          element={!isAuthenticated ? <RegisterPage /> : <Navigate to="/" />} 
        />
        
        <Route 
          path="/how-it-works" 
          element={isAuthenticated ? <HowItWorksPage /> : <Navigate to="/login" />} 
        />
 
        
        <Route 
          path="/active-auctions" 
          element={isAuthenticated ? <ActiveAuctionsPage /> : <Navigate to="/login" />} 
        />
        
        <Route 
          path="/upcoming-auctions" 
          element={isAuthenticated ? <UpcomingAuctionsPage /> : <Navigate to="/login" />} 
        />
       
 
        {/* Cualquier ruta inexistente vuelve al inicio en lugar de mostrar una pantalla en blanco. */}
        <Route path="*" element={<Navigate to="/" />} />
      </Routes>
    </div>
  );
}
 
export default App;
