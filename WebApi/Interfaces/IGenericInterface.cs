using WebApi.Models;

namespace WebApi.Interfaces
{
    /// <summary>
    /// Interfaz genérica para operaciones básicas de repositorio
    /// </summary>
    public interface IGenericInterface<T> where T : class
    {
        /// <summary>
        /// Obtiene todos los elementos
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();
        
        /// <summary>
        /// Obtiene un elemento por su ID
        /// </summary>
        Task<T> GetByIdAsync(int id);
        
        /// <summary>
        /// Crea un nuevo elemento
        /// </summary>
        Task<OperationResult> CreateAsync(T entity);
        
        /// <summary>
        /// Actualiza un elemento existente
        /// </summary>
        Task<OperationResult> UpdateAsync(int id, T entity);
        
        /// <summary>
        /// Elimina un elemento
        /// </summary>
        Task<OperationResult> DeleteAsync(int id);
    }
}