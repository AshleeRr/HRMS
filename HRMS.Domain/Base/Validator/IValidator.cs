﻿namespace HRMS.Domain.Base.Validator
{
    public interface IValidator<T>
    {
        OperationResult Validate(T entity);
    }
}
