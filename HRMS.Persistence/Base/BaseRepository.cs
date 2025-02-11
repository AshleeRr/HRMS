using HRMS.Domain.Base;
using HRMS.Domain.Repository;
using HRMS.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HRMS.Persistence.Base
{
    public abstract class BaseRepository<TEntity, TType> : IBaseRepository<TEntity, TType> where TEntity : class
    {
        private readonly HRMSContext _context;
        private DbSet<TEntity> Entity { get; set; }
        protected BaseRepository(HRMSContext context)
        {
            _context = context;
            Entity = _context.Set<TEntity>();
        }
        
        public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter) 
        {
            return await Entity.AnyAsync(filter);

        }
        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            return await Entity.ToListAsync();
        }
        public virtual async Task<OperationResult> GetAllAsync(Expression<Func<TEntity, bool>> filter)
        {
            OperationResult result = new OperationResult();
            try
            {
                var datos = Entity.Where(filter).ToListAsync();
                result.Data = datos;
            }
            catch (Exception ex) {
                result.IsSuccess = false;
                result.Message = "Ocurrió un error obteniendo los datos.";
            }

            return result;
        }
        public virtual async Task<TEntity> GetEntityByIdAsync(int id)
        {
            return await Entity.FindAsync(id);
        }
        
        public virtual async Task<OperationResult> SaveEntityAsync(TEntity entity)
        {
            OperationResult resultSave = new OperationResult();
            try { 
                Entity.Add(entity);
                await _context.SaveChangesAsync();

            }catch(Exception ex) {
                resultSave.IsSuccess = false;
                resultSave.Message = "Ocurrió un error guardando los datos.";
            }
            return resultSave;

        }

        public virtual async Task<OperationResult> UpdateEntityAsync(TEntity entity)
        {
            OperationResult resultUpdate = new OperationResult();
            try
            {
                Entity.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { 
                resultUpdate.IsSuccess = false;
                resultUpdate.Message = "Ocurrió un error actualizando los datos.";
            }
            return resultUpdate;


        }
    }
}
