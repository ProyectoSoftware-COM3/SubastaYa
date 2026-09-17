import { Routes, Route, Navigate } from 'react-router-dom';
import { useAuth } from './hooks/useAuth';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';

import HowItWorksPage from './pages/HowItWorksPage';

 
function App() {
  
  const { isAuthenticated } = useAuth();
  
 
  return (
    <div className="min-h-screen bg-black text-white font-sans">
      <Routes>
        
        <Route 
          path="/" 
          element={isAuthenticated ? <HowItWorksPage /> : <Navigate to="/login" />} 
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
        
 
        {/* Cualquier ruta inexistente vuelve al inicio en lugar de mostrar una pantalla en blanco. */}
        <Route path="*" element={<Navigate to="/" />} />
      </Routes>
    </div>
  );
}
 
export default App;
