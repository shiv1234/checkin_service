using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Http;
using UnlockOKR.OKRSupoort.Domain.Ports;

namespace UnlockOKR.OKRSupoort.Infrastructure.Services.Contracts
{
    public interface ISystemService
    {
        HttpContext HttpContext { get; }
        HttpClient SystemHttpClient();
        Uri SystemUri(string path);
    }
}
