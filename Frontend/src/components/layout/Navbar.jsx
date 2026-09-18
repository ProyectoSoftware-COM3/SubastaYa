// src/components/layout/Navbar.jsx
import { useState, useRef, useEffect } from 'react';
import { Link, NavLink } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';
 
const TOP_NAV_LINKS = [
  { to: '/', label: 'Catálogo', end: true },
  { to: '/active-auctions', label: 'Subastas Activas' },
  { to: '/upcoming-auctions', label: 'Próximas Subastas' },
  { to: '/how-it-works', label: 'Cómo Funciona' },
];
 
const linkClass = ({ isActive }) =>
  `nav-btn text-sm ${isActive ? 'font-bold text-white' : ''}`;
 
export default function Navbar() {
  const { isAuthenticated, user, logout } = useAuth();
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const [isAccountOpen, setIsAccountOpen] = useState(false);
  const accountRef = useRef(null);
 
  const userName = user?.name || user?.email?.split('@')[0] || 'Usuario';
  const closeMenu = () => setIsMenuOpen(false);
 
  // Cerrar el menú desplegable al hacer clic fuera
  useEffect(() => {
    const handleClickOutside = (event) => {
      if (accountRef.current && !accountRef.current.contains(event.target)) {
        setIsAccountOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);
 
  return (
     <nav className="sticky top-0 z-40 bg-[#0a0a0a] border-b border-gray-800">
      {/*FILA SUPERIOR (Logo y Mi Cuenta / Auth) */}
      <div className="max-w-7xl mx-auto flex items-center justify-between px-4 sm:px-8 py-3 border-b border-gray-900">
        
        {/* Espacio izquierdo para balancear el layout en desktop */}
        <div className="hidden sm:block w-36"></div>
 
        {/* Logo Centrado / Principal */}
        <Link to="/" className="flex items-center hover:opacity-80 transition-opacity mx-auto sm:mx-0">
          <img 
            src="/logo.png" 
            alt="SubastaYa" 
            className="h-20 sm:h-24 w-auto object-contain brightness-125 contrast-110 drop-shadow-[0_0_15px_rgba(212,175,55,0.4)] hover:drop-shadow-[0_0_20px_rgba(212,175,55,0.6)] transition-all" 
          />
        </Link>
 
        {/* Derecha: Saludo arriba y Botón con icono abajo (Apilados y limpios) */}
        <div className="flex items-center gap-4">
          {isAuthenticated ? (
            <div className="flex flex-col items-center gap-1 relative" ref={accountRef}>
              
              
              <span className="text-xs text-gray-400 uppercase tracking-wider font-medium">
                Hola, <span className="text-[#f1c40f] font-semibold">{userName}</span>
              </span>
 
              {/* Botón Mi Cuenta con Icono ABAJO */}
              <button
                type="button"
                onClick={() => setIsAccountOpen((prev) => !prev)}
                className="nav-btn flex items-center gap-2.5 cursor-pointer py-1.5 px-3 text-sm"
              >
                <div className="w-6 h-6 rounded-full border border-[#f1c40f] flex items-center justify-center bg-black/40 text-[#f1c40f] shadow-[0_0_8px_rgba(241,196,15,0.3)]">
                  <svg className="w-3.5 h-3.5" fill="currentColor" viewBox="0 0 24 24">
                    <path d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z" />
                  </svg>
                </div>
                <span>Mi Cuenta</span>
                <svg className={`w-3.5 h-3.5 transition-transform ${isAccountOpen ? 'rotate-180' : ''}`} fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 9l-7 7-7-7" />
                </svg>
              </button>
 
              {/* Menú Desplegable */}
              {isAccountOpen && (
                <div className="dropdown-menu">
                  <Link to="/activities" className="dropdown-item" onClick={() => setIsAccountOpen(false)}>
                    Mis Actividades
                  </Link>
                  <Link to="/wallet" className="dropdown-item" onClick={() => setIsAccountOpen(false)}>
                    Mi Billetera
                  </Link>
                  <Link to="/create-auction" className="dropdown-item" onClick={() => setIsAccountOpen(false)}>
                    Publicar Subasta
                  </Link>
                  <div className="border-t border-gray-800 my-1"></div>
                  <button
                    type="button"
                    onClick={() => { setIsAccountOpen(false); logout(); }}
                    className="dropdown-item text-red-400 hover:text-red-300 hover:bg-red-950/30"
                  >
                    Cerrar Sesión
                  </button>
                </div>
              )}
            </div>
          ) : (
            <Link to="/login" className="nav-btn">
              Iniciar Sesión
            </Link>
          )}
 
          {/* Botón menú hamburguesa móvil */}
          <button
            type="button"
            onClick={() => setIsMenuOpen((open) => !open)}
            className="lg:hidden p-2 border border-gray-700 rounded text-gray-300 hover:text-[#ffd277] hover:border-[#ffd277] transition-colors"
            aria-expanded={isMenuOpen}
            aria-controls="mobile-menu"
            aria-label={isMenuOpen ? 'Cerrar menú' : 'Abrir menú'}
          >
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" aria-hidden="true">
              {isMenuOpen ? <path d="M6 6l12 12M18 6L6 18" /> : <path d="M4 7h16M4 12h16M4 17h16" />}
            </svg>
          </button>
        </div>
      </div>
 
      {/*  FILA INFERIOR (Segundo panel de navegación) */}
      <div className="hidden lg:flex items-center justify-center gap-8 py-3 bg-[#050505] border-t border-gray-900">
        {TOP_NAV_LINKS.map((link) => (
          <NavLink key={link.to} to={link.to} end={link.end} className={linkClass}>
            {link.label}
          </NavLink>
        ))}
      </div>
 
      {/* Menú Desplegable de Celular */}
      {isMenuOpen && (
        <div id="mobile-menu" className="lg:hidden border-t border-gray-800 px-4 py-4 flex flex-col gap-2 text-sm bg-[#050505]">
          {TOP_NAV_LINKS.map((link) => (
            <NavLink key={link.to} to={link.to} end={link.end} className={linkClass} onClick={closeMenu}>
              {link.label}
            </NavLink>
          ))}
          
          {isAuthenticated && (
            <div className="flex flex-col gap-2 border-t border-gray-800 pt-4 mt-2">
              <span className="text-xs text-gray-400 uppercase tracking-wider px-3">
                Hola, <span className="text-[#f1c40f] font-semibold">{userName}</span>
              </span>
              <NavLink to="/activities" className="nav-btn justify-start px-3 py-2" onClick={closeMenu}>
                Mis Actividades
              </NavLink>
              <NavLink to="/wallet" className="nav-btn justify-start px-3 py-2" onClick={closeMenu}>
                Mi Billetera
              </NavLink>
              <NavLink to="/create-auction" className="nav-btn justify-start px-3 py-2" onClick={closeMenu}>
                Publicar Subasta
              </NavLink>
              <button
                type="button"
                onClick={() => { closeMenu(); logout(); }}
                className="self-start px-3 py-2 text-red-400 hover:text-red-300 font-semibold cursor-pointer"
              >
                Cerrar Sesión
              </button>
            </div>
          )}
        </div>
      )}
    </nav>
  );
}
