using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Http;
using OneGuru.CFR.Domain.Ports;

namespace OneGuru.CFR.Infrastructure.Services.Contracts
{
    public interface ISystemService
    {
        HttpContext HttpContext { get; }
        HttpClient SystemHttpClient();
        Uri SystemUri(string path);
    }
}
