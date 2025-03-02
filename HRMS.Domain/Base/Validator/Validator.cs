using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;


namespace MyValidator.Validator
{
    public class Validator<T> : IValidator<T>
    {
        public List<RuleValidation<T>> Rules { get; set; }
        private RuleValidation<T> _selectedRule;

        public Validator()
        {
            Rules = new List<RuleValidation<T>>();
        }

        public RuleValidation<T> AddRule(Func<T,bool> predicate)
        {

            var r = new RuleValidation<T>(predicate);
            _selectedRule = r;
            Rules.Add(r);
            return r;
        }

        /*
        public void WithErrorMessage(string message)
        {
            if(_selectedRule != null)
            {
                _selectedRule.WithErrorMessage(message);
            }
        }
        */


        public OperationResult Validate(T entity)
        {
            OperationResult result = new OperationResult();
            List<string> errors = new List<string>();
            foreach(RuleValidation<T> rule in Rules)
            {
                var res = rule.Evaluate(entity);
                if (!res.IsSuccess)
                {
                    errors.Add(res.Message);
                }
            }
            if(errors.Count > 0)
            {
                result.IsSuccess = false;
                result.Message = string.Join(Environment.NewLine, errors);
            }
            return result;
        }
    }
}
