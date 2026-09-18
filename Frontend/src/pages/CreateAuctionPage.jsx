// src/pages/CreateAuctionPage.jsx
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Navbar from '../components/layout/Navbar';
import AuctionImage from '../components/common/AuctionImage';
import Spinner from '../components/common/Spinner';
import ErrorMessage from '../components/common/ErrorMessage';
import { getCategories, createAuction } from '../services/auctionService';
import { useAsyncData } from '../hooks/useAsyncData';
import { useToast } from '../hooks/useToast';
import { localInputToIso, toLocalInputValue } from '../utils/dates';
import { hasErrors, isHttpUrl } from '../utils/validation';
 
const MAX_TITLE_LENGTH = 120;
// Margen para que "empezar ahora" siga siendo válido mientras el vendedor completa el formulario.
const START_DATE_TOLERANCE_MS = 5 * 60 * 1000;
 
const EMPTY_FORM = {
  title: '',
  description: '',
  imageUrl: '',
  categoryId: '',
  basePrice: '',
  minIncrement: '',
  startDate: '',
  endDate: '',
};
 
// Consigna Módulo 2: validaciones en pantalla antes de enviar. Replica y amplía CreateAuctionCommandValidator.
function validateAuctionForm(form, now) {
  const start = form.startDate ? new Date(form.startDate).getTime() : null;
  const end = form.endDate ? new Date(form.endDate).getTime() : null;
  const basePrice = Number(form.basePrice);
  const minIncrement = Number(form.minIncrement);
 
  return {
    title: !form.title.trim()
      ? 'Ingresá el título del producto.'
      : form.title.length > MAX_TITLE_LENGTH ? `El título no puede superar ${MAX_TITLE_LENGTH} caracteres.` : '',
    description: !form.description.trim() ? 'Ingresá una descripción detallada.' : '',
    imageUrl: !isHttpUrl(form.imageUrl) ? 'Ingresá una URL válida que empiece con http:// o https://.' : '',
    categoryId: !form.categoryId ? 'Elegí una categoría.' : '',
    basePrice: form.basePrice === '' || !(basePrice > 0) ? 'El precio base debe ser un valor positivo.' : '',
    minIncrement: form.minIncrement === '' || !(minIncrement > 0) ? 'El incremento mínimo debe ser un valor positivo.' : '',
    startDate: start === null
      ? 'Indicá la fecha y hora de inicio.'
      : start < now - START_DATE_TOLERANCE_MS ? 'La fecha de inicio no puede estar en el pasado.' : '',
    endDate: end === null
      ? 'Indicá la fecha y hora de finalización.'
      : start !== null && end <= start ? 'La fecha de finalización debe ser posterior a la de inicio.'
        : end <= now ? 'La fecha de finalización debe ser futura.' : '',
  };
}
 
function FieldError({ message }) {
  return message ? <p className="text-red-400 text-xs mt-1">{message}</p> : null;
}
 
const fieldClass = (hasError) =>
  `w-full p-3 bg-black border rounded text-white focus:outline-none disabled:opacity-60 ${hasError ? 'border-red-500' : 'border-gray-700 focus:border-[#d4af37]'}`;
 
export default function CreateAuctionPage() {
  const navigate = useNavigate();
  const toast = useToast();
  const categories = useAsyncData(getCategories, 'categories');
 
  const [formData, setFormData] = useState(EMPTY_FORM);
  const [errors, setErrors] = useState({});
  const [hasTriedSubmit, setHasTriedSubmit] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
 
  // Después del primer intento de envío, los errores se recalculan mientras el usuario corrige.
  const updateField = (field, value) => {
    const nextForm = { ...formData, [field]: value };
    setFormData(nextForm);
    if (hasTriedSubmit) setErrors(validateAuctionForm(nextForm, Date.now()));
  };
 
  const setStartNow = () => updateField('startDate', toLocalInputValue(new Date()));
 
  const handleSubmit = async (e) => {
    e.preventDefault();
    if (isSubmitting) return;
 
    const validationErrors = validateAuctionForm(formData, Date.now());
    setHasTriedSubmit(true);
    setErrors(validationErrors);
    if (hasErrors(validationErrors)) {
      toast.warning('Revisá los campos marcados antes de publicar.', 'Formulario incompleto');
      return;
    }
 
    setIsSubmitting(true);
    try {
      const payload = {
        title: formData.title.trim(),
        description: formData.description.trim(),
        imageUrl: formData.imageUrl.trim(),
        categoryId: formData.categoryId,
        basePrice: Number(formData.basePrice),
        minIncrement: Number(formData.minIncrement),
        startDate: localInputToIso(formData.startDate),
        endDate: localInputToIso(formData.endDate),
      };
 
      const responseData = await createAuction(payload);
      toast.success('La subasta fue publicada.', 'Publicación exitosa');
      navigate(`/auction/${responseData?.id ?? responseData}`);
    } catch (error) {
      if (hasErrors(error.fieldErrors ?? {})) setErrors((current) => ({ ...current, ...error.fieldErrors }));
      toast.error(error.message, 'No se pudo publicar la subasta');
      setIsSubmitting(false);
    }
  };
 
  return (
    <div className="min-h-screen bg-[#050505] text-white">
      <Navbar />
      <main className="max-w-2xl mx-auto px-4 sm:px-8 py-8 sm:py-12">
        <h1 className="text-3xl font-serif font-bold text-[#d4af37] mb-8">Publicar Nueva Subasta</h1>
 
        <form onSubmit={handleSubmit} className="flex flex-col gap-5 bg-[#0a0a0a] p-6 border border-gray-800 rounded-lg" noValidate>
          <fieldset disabled={isSubmitting} className="flex flex-col gap-5">
            <legend className="text-sm font-bold uppercase tracking-wider text-gray-400 mb-1">Datos del producto</legend>
 
            <div>
              <label htmlFor="auction-title" className="text-xs text-gray-400 block mb-1">Título</label>
              <input
                id="auction-title"
                type="text"
                maxLength={MAX_TITLE_LENGTH}
                value={formData.title}
                onChange={(e) => updateField('title', e.target.value)}
                className={fieldClass(Boolean(errors.title))}
              />
              <FieldError message={errors.title} />
            </div>
 
            <div>
              <label htmlFor="auction-description" className="text-xs text-gray-400 block mb-1">Descripción detallada</label>
              <textarea
                id="auction-description"
                value={formData.description}
                onChange={(e) => updateField('description', e.target.value)}
                className={`${fieldClass(Boolean(errors.description))} h-28`}
              />
              <FieldError message={errors.description} />
            </div>
 
            <div>
              <label htmlFor="auction-image" className="text-xs text-gray-400 block mb-1">URL de la imagen</label>
              <input
                id="auction-image"
                type="url"
                placeholder="https://..."
                value={formData.imageUrl}
                onChange={(e) => updateField('imageUrl', e.target.value)}
                className={fieldClass(Boolean(errors.imageUrl))}
              />
              <FieldError message={errors.imageUrl} />
              {isHttpUrl(formData.imageUrl) && (
                <div className="mt-3 h-40 rounded border border-gray-800 overflow-hidden">
                  <AuctionImage src={formData.imageUrl} alt="Vista previa" className="w-full h-full" fit="contain" />
                </div>
              )}
            </div>
 
            <div>
              <label htmlFor="auction-category" className="text-xs text-gray-400 block mb-1">Categoría</label>
              {categories.isLoading ? (
                <Spinner label="Cargando categorías..." size="sm" className="justify-start py-3" />
              ) : categories.error ? (
                <ErrorMessage message={categories.error.message} onRetry={categories.reload} />
              ) : (
                <select
                  id="auction-category"
                  value={formData.categoryId}
                  onChange={(e) => updateField('categoryId', e.target.value)}
                  className={fieldClass(Boolean(errors.categoryId))}
                >
                  <option value="">Seleccioná una categoría</option>
                  {(categories.data ?? []).map((category) => (
                    <option key={category.id} value={category.id}>{category.name}</option>
                  ))}
                </select>
              )}
              <FieldError message={errors.categoryId} />
            </div>
          </fieldset>
 
          <fieldset disabled={isSubmitting} className="flex flex-col gap-5 border-t border-gray-800 pt-5">
            <legend className="text-sm font-bold uppercase tracking-wider text-gray-400 mb-1">Configuración económica</legend>
            <div className="flex flex-col sm:flex-row gap-4">
              <div className="sm:w-1/2">
                <label htmlFor="auction-base-price" className="text-xs text-gray-400 block mb-1">Precio base ($)</label>
                <input
                  id="auction-base-price"
                  type="number"
                  min="1"
                  step="any"
                  value={formData.basePrice}
                  onChange={(e) => updateField('basePrice', e.target.value)}
                  className={fieldClass(Boolean(errors.basePrice))}
                />
                <FieldError message={errors.basePrice} />
              </div>
              <div className="sm:w-1/2">
                <label htmlFor="auction-min-increment" className="text-xs text-gray-400 block mb-1">Incremento mínimo por puja ($)</label>
                <input
                  id="auction-min-increment"
                  type="number"
                  min="1"
                  step="any"
                  value={formData.minIncrement}
                  onChange={(e) => updateField('minIncrement', e.target.value)}
                  className={fieldClass(Boolean(errors.minIncrement))}
                />
                <FieldError message={errors.minIncrement} />
              </div>
            </div>
          </fieldset>
 
          <fieldset disabled={isSubmitting} className="flex flex-col gap-5 border-t border-gray-800 pt-5">
            <legend className="text-sm font-bold uppercase tracking-wider text-gray-400 mb-1">Ventana temporal</legend>
            <div className="flex flex-col sm:flex-row gap-4">
              <div className="sm:w-1/2">
                <div className="flex items-center justify-between mb-1">
                  <label htmlFor="auction-start" className="text-xs text-gray-400">Fecha de inicio</label>
                  <button type="button" onClick={setStartNow} className="text-xs text-[#d4af37] hover:underline">Empezar ahora</button>
                </div>
                <input
                  id="auction-start"
                  type="datetime-local"
                  value={formData.startDate}
                  onChange={(e) => updateField('startDate', e.target.value)}
                  className={fieldClass(Boolean(errors.startDate))}
                />
                <FieldError message={errors.startDate} />
              </div>
              <div className="sm:w-1/2">
                <label htmlFor="auction-end" className="text-xs text-gray-400 block mb-1">Fecha de finalización</label>
                <input
                  id="auction-end"
                  type="datetime-local"
                  min={formData.startDate || undefined}
                  value={formData.endDate}
                  onChange={(e) => updateField('endDate', e.target.value)}
                  className={fieldClass(Boolean(errors.endDate))}
                />
                <FieldError message={errors.endDate} />
              </div>
            </div>
          </fieldset>
 
          <button
            type="submit"
            disabled={isSubmitting || categories.isLoading}
            className="mt-2 py-3 bg-[#d4af37] text-black font-bold rounded hover:bg-[#c49a2e] transition-colors disabled:opacity-60 disabled:cursor-not-allowed flex items-center justify-center gap-2"
          >
            {isSubmitting ? <><Spinner label="" size="sm" /> Publicando...</> : 'Crear Subasta'}
          </button>
        </form>
      </main>
    </div>
  );
}
