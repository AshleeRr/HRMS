using HRMS.Domain.Base;


namespace MyValidator.Validator
{
    public class RuleValidation<T>
    {
        public Func<T, bool> Predicate { get; set; }
        public string Message { get; set; }


        public RuleValidation(Func<T, bool> predicate)
        {
            Predicate = predicate;
        }

        public void WithErrorMessage(string meessage)
            => Message = meessage;

        public OperationResult Evaluate(T entity)
        {
            OperationResult result = new OperationResult();
            result.IsSuccess = Predicate(entity);
            if (!result.IsSuccess)
                result.Message = Message;
            return result;
        }
    }
}
