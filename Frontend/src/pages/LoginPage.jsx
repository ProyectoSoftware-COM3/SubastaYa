import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { loginApi } from '../api/authApi';
import Spinner from '../components/common/Spinner';

const inputClass = 'w-full px-4 py-2 bg-gray-900 border border-gray-700 rounded text-white focus:outline-none focus:border-[#d4af37] disabled:opacity-60';

function getLoginErrorMessage(error) {
  if (error.status === 401) return 'Correo o contraseña incorrectos.';
  return error.message;
}

export default function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const { loginContext } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (isSubmitting) return;

    setError('');
    setIsSubmitting(true);
    try {
      const data = await loginApi({ email, password });
      // POST /api/sessions devuelve un AuthDto plano: token, expiresAt, userId, name y email.
      loginContext({ userId: data.userId, name: data.name, email: data.email }, data.token);
      navigate('/');
    } catch (err) {
      setError(getLoginErrorMessage(err));
      setIsSubmitting(false);
    }
  };

  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-black px-4">
      <div className="w-full max-w-md p-8 border border-[#d4af37] rounded-lg shadow-lg bg-black">
        <h2 className="text-3xl font-serif font-bold text-[#d4af37] text-center mb-6">Iniciar Sesión</h2>
        {error && <p className="text-red-400 text-sm mb-4 text-center" role="alert">{error}</p>}
        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          <div>
            <label htmlFor="login-email" className="block text-sm text-gray-300 mb-1">Correo electrónico</label>
            <input
              id="login-email"
              type="email"
              autoComplete="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className={inputClass}
              disabled={isSubmitting}
              required
            />
          </div>
                   <div>
            <label htmlFor="login-password" className="block text-sm text-gray-300 mb-1">Contraseña</label>
            <div className="relative">
              <input
                id="login-password"
                type={showPassword ? 'text' : 'password'}
                autoComplete="current-password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className={`${inputClass} pr-11`}
                disabled={isSubmitting}
                required
              />
              <button
                type="button"
                onClick={() => setShowPassword((visible) => !visible)}
                className="absolute inset-y-0 right-0 px-3 text-gray-400 hover:text-[#d4af37] transition-colors"
                aria-label={showPassword ? 'Ocultar contraseña' : 'Mostrar contraseña'}
                aria-pressed={showPassword}
              >
                {showPassword ? (
                  <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" aria-hidden="true">
                    <path d="M3 3l18 18" />
                    <path d="M10.6 10.6a2 2 0 002.8 2.8" />
                    <path d="M9.4 5.2A9.5 9.5 0 0112 5c5 0 9 5 9 7a11 11 0 01-2.4 3.3M6.5 6.9C4.2 8.4 3 10.7 3 12c0 2 4 7 9 7 1.3 0 2.5-.3 3.6-.8" />
                  </svg>
                ) : (
                  <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" aria-hidden="true">
                    <path d="M3 12s4-7 9-7 9 7 9 7-4 7-9 7-9-7-9-7z" />
                    <circle cx="12" cy="12" r="3" />
                  </svg>
                )}
              </button>
            </div>
          </div>
          <button
            type="submit"
            disabled={isSubmitting}
            className="w-full mt-4 py-2 bg-[#d4af37] text-black font-bold rounded hover:bg-[#c49a2e] transition-colors disabled:opacity-60 disabled:cursor-not-allowed flex items-center justify-center gap-2"
          >
            {isSubmitting ? <><Spinner label="" size="sm" /> Ingresando...</> : 'Ingresar'}
          </button>
        </form>
        <p className="mt-4 text-center text-sm text-gray-400">
          ¿No tienes cuenta? <Link to="/register" className="text-[#d4af37] hover:underline">Regístrate aquí</Link>
        </p>
      </div>
    </div>
  );
}
