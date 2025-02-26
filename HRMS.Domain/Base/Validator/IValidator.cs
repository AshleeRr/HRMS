using HRMS.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyValidator.Validator
{
    public interface IValidator<T>
    {
        OperationResult Validate(T entity);
    }
}
