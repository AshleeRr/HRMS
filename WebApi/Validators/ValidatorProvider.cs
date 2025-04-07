using HRMS.Domain.Base.Validator;

namespace WebApi.Validators
{
    public class ValidatorProvider : IValidatorProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidatorProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IValidator<T> GetValidator<T>()
        {
           var val = _serviceProvider.GetService<IValidator<T>>()
                ?? throw new InvalidOperationException($"Validator for type {typeof(T).Name} not registered.");

            return val;
        }
    }
 
}
