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
            <input
              id="login-password"
              type="password"
              autoComplete="current-password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className={inputClass}
              disabled={isSubmitting}
              required
            />
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
