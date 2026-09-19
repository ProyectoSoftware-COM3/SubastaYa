// src/pages/EditAuctionPage.jsx
import { useEffect, useState } from 'react';
import { useNavigate, useParams, Link } from 'react-router-dom';
import Navbar from '../components/layout/Navbar';
import AuctionImage from '../components/common/AuctionImage';
import Spinner from '../components/common/Spinner';
import ErrorMessage from '../components/common/ErrorMessage';
import { getAuctionById, getCategories, updateAuction } from '../services/auctionService';
import { useAsyncData } from '../hooks/useAsyncData';
import { useToast } from '../hooks/useToast';
import { hasErrors, isHttpUrl } from '../utils/validation';

const MAX_TITLE_LENGTH = 120;


function validateEditForm(form) {
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
  };
}

function FieldError({ message }) {
  return message ? <p className="text-red-400 text-xs mt-1">{message}</p> : null;
}

const fieldClass = (hasError) =>
  `w-full p-3 bg-black border rounded text-white focus:outline-none disabled:opacity-60 ${hasError ? 'border-red-500' : 'border-gray-700 focus:border-[#d4af37]'}`;

export default function EditAuctionPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const toast = useToast();

  const auction = useAsyncData(() => getAuctionById(id), `auction-${id}`);
  const categories = useAsyncData(getCategories, 'categories');

  const [formData, setFormData] = useState(null);
  const [errors, setErrors] = useState({});
  const [hasTriedSubmit, setHasTriedSubmit] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);

  // El formulario se completa una sola vez con los datos actuales de la subasta.
  useEffect(() => {
    if (!auction.data || formData) return;
    setFormData({
      title: auction.data.title ?? '',
      description: auction.data.description ?? '',
      imageUrl: auction.data.imageUrl ?? '',
      categoryId: auction.data.categoryId ?? '',
      basePrice: String(auction.data.basePrice ?? ''),
      minIncrement: String(auction.data.minIncrement ?? ''),
    });
  }, [auction.data, formData]);

  const updateField = (field, value) => {
    const nextForm = { ...formData, [field]: value };
    setFormData(nextForm);
    if (hasTriedSubmit) setErrors(validateEditForm(nextForm));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (isSubmitting || !formData) return;

    const validationErrors = validateEditForm(formData);
    setHasTriedSubmit(true);
    setErrors(validationErrors);
    if (hasErrors(validationErrors)) {
      toast.warning('Revisá los campos marcados antes de guardar.', 'Formulario incompleto');
      return;
    }

    setIsSubmitting(true);
    try {
      await updateAuction(id, {
        title: formData.title.trim(),
        description: formData.description.trim(),
        imageUrl: formData.imageUrl.trim(),
        categoryId: formData.categoryId,
        basePrice: Number(formData.basePrice),
        minIncrement: Number(formData.minIncrement),
      });
      toast.success('Los cambios fueron guardados.', 'Subasta actualizada');
      navigate(`/auction/${id}`);
    } catch (error) {
      // 409: la subasta dejo de ser editable mientras se completaba el formulario (recibio una oferta o venció el plazo).
      if (error.status === 409) toast.error(error.message, 'Ya no se puede editar');
      else if (hasErrors(error.fieldErrors ?? {})) setErrors((current) => ({ ...current, ...error.fieldErrors }));
      else toast.error(error.message, 'No se pudieron guardar los cambios');
      setIsSubmitting(false);
    }
  };

  const isLoading = (auction.isLoading && !auction.data) || (categories.isLoading && !categories.data);

  return (
    <div className="min-h-screen bg-[#050505] text-white">
      <Navbar />
      <main className="max-w-2xl mx-auto px-4 sm:px-8 py-8 sm:py-12">
        <h1 className="text-3xl font-serif font-bold text-[#d4af37] mb-2">Editar Subasta</h1>
        <p className="text-xs text-gray-500 mb-8">
          Solo se puede editar mientras la subasta no tenga ofertas y dentro de los primeros minutos desde su inicio.
          Las fechas de inicio y cierre no se modifican.
        </p>

        {isLoading ? (
          <Spinner label="Cargando subasta..." className="py-12" />
        ) : auction.error ? (
          <ErrorMessage message={auction.error.message} onRetry={auction.reload} />
        ) : !formData ? null : (
          <form onSubmit={handleSubmit} className="flex flex-col gap-5 bg-[#0a0a0a] p-6 border border-gray-800 rounded-lg" noValidate>
            <fieldset disabled={isSubmitting} className="flex flex-col gap-5">
              <div>
                <label htmlFor="edit-title" className="text-xs text-gray-400 block mb-1">Título</label>
                <input
                  id="edit-title"
                  type="text"
                  maxLength={MAX_TITLE_LENGTH}
                  value={formData.title}
                  onChange={(e) => updateField('title', e.target.value)}
                  className={fieldClass(Boolean(errors.title))}
                />
                <FieldError message={errors.title} />
              </div>

              <div>
                <label htmlFor="edit-description" className="text-xs text-gray-400 block mb-1">Descripción</label>
                <textarea
                  id="edit-description"
                  value={formData.description}
                  onChange={(e) => updateField('description', e.target.value)}
                  className={`${fieldClass(Boolean(errors.description))} h-28`}
                />
                <FieldError message={errors.description} />
              </div>

              <div>
                <label htmlFor="edit-image" className="text-xs text-gray-400 block mb-1">URL de la imagen</label>
                <input
                  id="edit-image"
                  type="url"
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
                <label htmlFor="edit-category" className="text-xs text-gray-400 block mb-1">Categoría</label>
                <select
                  id="edit-category"
                  value={formData.categoryId}
                  onChange={(e) => updateField('categoryId', e.target.value)}
                  className={fieldClass(Boolean(errors.categoryId))}
                >
                  <option value="">Seleccioná una categoría</option>
                  {(categories.data ?? []).map((category) => (
                    <option key={category.id} value={category.id}>{category.name}</option>
                  ))}
                </select>
                <FieldError message={errors.categoryId} />
              </div>

              <div className="flex flex-col sm:flex-row gap-4">
                <div className="sm:w-1/2">
                  <label htmlFor="edit-base-price" className="text-xs text-gray-400 block mb-1">Precio base ($)</label>
                  <input
                    id="edit-base-price"
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
                  <label htmlFor="edit-min-increment" className="text-xs text-gray-400 block mb-1">Incremento mínimo ($)</label>
                  <input
                    id="edit-min-increment"
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

            <div className="flex flex-col sm:flex-row gap-3 mt-2">
              <button
                type="submit"
                disabled={isSubmitting}
                className="flex-1 py-3 bg-[#d4af37] text-black font-bold rounded hover:bg-[#c49a2e] transition-colors disabled:opacity-60 disabled:cursor-not-allowed flex items-center justify-center gap-2"
              >
                {isSubmitting ? <><Spinner label="" size="sm" /> Guardando...</> : 'Guardar Cambios'}
              </button>
              <Link
                to={`/auction/${id}`}
                className="py-3 px-6 border border-gray-700 text-gray-300 rounded text-center hover:border-[#d4af37] hover:text-[#d4af37] transition-colors"
              >
                Cancelar
              </Link>
            </div>
          </form>
        )}
      </main>
    </div>
  );
}