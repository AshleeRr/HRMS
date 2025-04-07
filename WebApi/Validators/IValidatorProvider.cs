using HRMS.Domain.Base.Validator;

namespace WebApi.Validators
{
    public interface IValidatorProvider
    {
        IValidator<T> GetValidator<T>();
    }
}
