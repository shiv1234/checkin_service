using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OneGuru.CFR.Domain.ResponseModels;
using System.Collections.Generic;

namespace OneGuru.CFR.Domain.Ports
{
    public interface ICommonBase
    {
        Payload<T> GetPayloadStatus<T>(Payload<T> payload, ModelStateDictionary modelState);
        Payload<T> GetPayloadStatus<T>(Payload<T> payload, List<ValidationFailure> errors);
    }
}
