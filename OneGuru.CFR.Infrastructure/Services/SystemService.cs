using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OneGuru.CFR.Domain.Ports;
using OneGuru.CFR.Infrastructure.Services.Contracts;

namespace OneGuru.CFR.Infrastructure.Services
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
