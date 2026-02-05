using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace UnlockOKR.OKRSupoort.Application.Filters
{
    public class ResultFilter : ResultFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}
