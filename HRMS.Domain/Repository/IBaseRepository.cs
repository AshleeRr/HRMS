using HRMS.Domain.Base;
using System.Linq.Expressions;

namespace HRMS.Domain.Repository
{
    public interface IBaseRepository <TEntity, TType> where TEntity : class
    {
       Task<TEntity> GetEntityByIdAsync(int id);
       Task<OperationResult> UpdateEntityAsync(TEntity entity);
       Task<OperationResult> SaveEntityAsync(TEntity entity);
       Task<List<TEntity>> GetAllAsync();
       Task<OperationResult> GetAllAsync(Expression<Func<TEntity, bool>> filter);
       Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter);
    }
}
