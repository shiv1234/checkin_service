using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OneGuru.CFR.Domain.Common;
using OneGuru.CFR.Domain.Ports;
using OneGuru.CFR.Domain.ResponseModels;
using System.Collections.Generic;
using System.Net;

namespace OneGuru.CFR.Infrastructure.Adapters.BaseAdapter
{
    public class CommonBase : ICommonBase
    {
        public Payload<T> GetPayloadStatus<T>(Payload<T> payload, ModelStateDictionary modelState)
        {
            foreach (var state in modelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    payload.MessageList.Add(state.Key, error.ErrorMessage);
                }
            }
            payload.IsSuccess = false;
            payload.Status = (int)HttpStatusCode.BadRequest;
            return payload;
        }
        public Payload<T> GetPayloadStatus<T>(Payload<T> payload, List<ValidationFailure> errors)
        {
            foreach (var error in errors)
            {
                payload.Status = (int)HttpStatusCode.BadRequest;
                payload.IsSuccess = false;
                payload.MessageType = MessageType.Error.ToString();
                if (!payload.MessageList.ContainsKey(error.PropertyName))
                {
                    payload.MessageList.Add(error.PropertyName, error.ErrorMessage);
                }
            }
            return payload;
        }
    }
}
