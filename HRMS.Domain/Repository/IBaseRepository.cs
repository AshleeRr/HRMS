using HRMS.Domain.Base;
using System.Linq.Expressions;

namespace HRMS.Domain.Repository
{
    public interface IBaseRepository <TEntity> where TEntity : class
    {
       Task<TEntity> GetEntityByIdAsync(int id);
       Task UpdateEntityAsinc(TEntity entity);
       Task DeleteEntityAsync(TEntity entity);
       Task SaveEntityAsync(TEntity entity);
       Task<List<TEntity>> GetAllAsync();
      //Task<OperactionResult> GetAllAsync(Expression<Func<TEntity, bool>> filter);
       Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter);
    }
}
