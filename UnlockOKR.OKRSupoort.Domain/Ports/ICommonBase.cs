using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using UnlockOKR.OKRSupoort.Domain.ResponseModels;
using System.Collections.Generic;

namespace UnlockOKR.OKRSupoort.Domain.Ports
{
    public interface ICommonBase
    {
        Payload<T> GetPayloadStatus<T>(Payload<T> payload, ModelStateDictionary modelState);
        Payload<T> GetPayloadStatus<T>(Payload<T> payload, List<ValidationFailure> errors);
    }
}
