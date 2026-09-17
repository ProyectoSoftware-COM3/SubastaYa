import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { registerApi } from '../api/authApi';
import { useToast } from '../hooks/useToast';
import Spinner from '../components/common/Spinner';
import { hasErrors, isValidEmail } from '../utils/validation';

const MIN_PASSWORD_LENGTH = 6;
const MAX_NAME_LENGTH = 100;

const inputClass = (hasError) =>
  `w-full px-4 py-2 bg-gray-900 border rounded text-white focus:outline-none disabled:opacity-60 ${hasError ? 'border-red-500 focus:border-red-400' : 'border-gray-700 focus:border-[#d4af37]'}`;

function validateRegistration({ nombre, email, password }) {
  return {
    name: !nombre.trim() ? 'Ingresá tu nombre.' : nombre.length > MAX_NAME_LENGTH ? `El nombre no puede superar ${MAX_NAME_LENGTH} caracteres.` : '',
    email: !isValidEmail(email) ? 'Ingresá un correo electrónico válido.' : '',
    password: password.length < MIN_PASSWORD_LENGTH ? `La contraseña debe tener al menos ${MIN_PASSWORD_LENGTH} caracteres.` : '',
  };
}

export default function RegisterPage() {
  const [nombre, setNombre] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [errors, setErrors] = useState({});
  const [generalError, setGeneralError] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const navigate = useNavigate();
  const toast = useToast();

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (isSubmitting) return;

    const validationErrors = validateRegistration({ nombre, email, password });
    setErrors(validationErrors);
    setGeneralError('');
    if (hasErrors(validationErrors)) return;

    setIsSubmitting(true);
    try {
      await registerApi({ name: nombre.trim(), email, password });
      toast.success('Tu cuenta fue creada. Ya podés iniciar sesión.', 'Registro exitoso');
      navigate('/login');
    } catch (err) {
      if (err.status === 409) {
        setErrors({ email: err.message });
      } else if (hasErrors(err.fieldErrors ?? {})) {
        setErrors(err.fieldErrors);
      } else {
        setGeneralError(err.message);
      }
      setIsSubmitting(false);
    }
  };

  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-black px-4">
      <div className="w-full max-w-md p-8 border border-[#d4af37] rounded-lg shadow-lg bg-black">
        <h2 className="text-3xl font-serif font-bold text-[#d4af37] text-center mb-6">Registro</h2>
        {generalError && <p className="text-red-400 text-sm mb-4 text-center" role="alert">{generalError}</p>}
        <form onSubmit={handleSubmit} className="flex flex-col gap-4" noValidate>
          <div>
            <label htmlFor="register-name" className="block text-sm text-gray-300 mb-1">Nombre</label>
            <input
              id="register-name"
              type="text"
              autoComplete="name"
              value={nombre}
              onChange={(e) => setNombre(e.target.value)}
              className={inputClass(Boolean(errors.name))}
              disabled={isSubmitting}
            />
            {errors.name && <p className="text-red-400 text-xs mt-1">{errors.name}</p>}
          </div>
          <div>
            <label htmlFor="register-email" className="block text-sm text-gray-300 mb-1">Correo electrónico</label>
            <input
              id="register-email"
              type="email"
              autoComplete="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className={inputClass(Boolean(errors.email))}
              disabled={isSubmitting}
            />
            {errors.email && <p className="text-red-400 text-xs mt-1">{errors.email}</p>}
          </div>
          <div>
            <label htmlFor="register-password" className="block text-sm text-gray-300 mb-1">Contraseña</label>
            <input
              id="register-password"
              type="password"
              autoComplete="new-password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className={inputClass(Boolean(errors.password))}
              disabled={isSubmitting}
            />
            {errors.password
              ? <p className="text-red-400 text-xs mt-1">{errors.password}</p>
              : <p className="text-gray-500 text-xs mt-1">Mínimo {MIN_PASSWORD_LENGTH} caracteres.</p>}
          </div>
          <button
            type="submit"
            disabled={isSubmitting}
            className="w-full mt-4 py-2 bg-[#d4af37] text-black font-bold rounded hover:bg-[#c49a2e] transition-colors disabled:opacity-60 disabled:cursor-not-allowed flex items-center justify-center gap-2"
          >
            {isSubmitting ? <><Spinner label="" size="sm" /> Creando cuenta...</> : 'Crear cuenta'}
          </button>
        </form>
        <p className="mt-4 text-center text-sm text-gray-400">
          ¿Ya tienes cuenta? <Link to="/login" className="text-[#d4af37] hover:underline">Inicia sesión</Link>
        </p>
      </div>
    </div>
  );
}
