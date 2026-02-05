using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using UnlockOKR.OKRSupoort.Domain.Ports;
using UnlockOKR.OKRSupoort.Infrastructure.Services.Contracts;

namespace UnlockOKR.OKRSupoort.Infrastructure.Services
{
   public class SystemService : ISystemService
    {
        public HttpContext HttpContext => new HttpContextAccessor().HttpContext;
        public HttpClient SystemHttpClient()
        {
            return new HttpClient();
        }
       
        public Uri SystemUri(string path)
        {
            return new Uri(path);
        }
    }
}
