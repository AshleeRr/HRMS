using HRMS.Domain.Base;


namespace MyValidator.Validator
{
    public interface IValidator<T>
    {
        OperationResult Validate(T entity);
    }
}
