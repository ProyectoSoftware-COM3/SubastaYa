import { Link } from 'react-router-dom';

const LoginPage = () => {
  return (
    <div className="w-full max-w-md p-8 bg-[#0a0a0a] border border-gray-800 rounded-2xl shadow-2xl">
      <div className="text-center mb-8">
        <h2 className="text-3xl font-bold text-brand-gold mb-2">Bienvenido de nuevo</h2>
        <p className="text-gray-400 text-sm">Ingresá a tu cuenta de SubastaYa</p>
      </div>

      <form className="space-y-6">
        <div>
          <label className="block text-sm font-medium text-gray-300 mb-2">
            Correo Electrónico
          </label>
          <input
            type="email"
            className="w-full px-4 py-3 bg-[#050505] border border-gray-700 rounded-lg focus:outline-none focus:border-brand-gold focus:ring-1 focus:ring-brand-gold text-white transition-colors"
            placeholder="tu@email.com"
          />
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-300 mb-2">
            Contraseña
          </label>
          <input
            type="password"
            className="w-full px-4 py-3 bg-[#050505] border border-gray-700 rounded-lg focus:outline-none focus:border-brand-gold focus:ring-1 focus:ring-brand-gold text-white transition-colors"
            placeholder="••••••••"
          />
        </div>

        <div className="flex items-center justify-end">
          <a href="#" className="text-sm text-brand-gold hover:text-yellow-300 transition-colors">
            ¿Olvidaste tu contraseña?
          </a>
        </div>

        <button type="submit" className="btn-lor w-full flex items-center justify-between">
          <span>Iniciar Sesión</span>
          <svg 
            width="20" 
            height="20" 
            className="text-current shrink-0" 
            fill="none" 
            stroke="currentColor" 
            strokeWidth="2.5" 
            viewBox="0 0 24 24"
          >
            <path strokeLinecap="round" strokeLinejoin="round" d="M14 5l7 7m0 0l-7 7m7-7H3" />
          </svg>
        </button>
      </form>

      <p className="mt-8 text-center text-sm text-gray-400">
        ¿No tenés una cuenta?{' '}
        <Link to="/register" className="text-brand-gold hover:text-yellow-300 font-semibold transition-colors">
          Registrate acá
        </Link>
      </p>
    </div>
  );
};

export default LoginPage;