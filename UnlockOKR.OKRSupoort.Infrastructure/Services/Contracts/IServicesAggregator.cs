using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UnlockOKR.OKRSupoort.Persistence.EntityFrameworkDataAccess;

namespace UnlockOKR.OKRSupoort.Infrastructure.Services.Contracts
{
    public interface IServicesAggregator
    {
        IUnitOfWorkAsync UnitOfWorkAsync { get; set; }
        IOperationStatus OperationStatus { get; set; }
        IConfiguration Configuration { get; set; }
        IWebHostEnvironment HostingEnvironment { get; set; }
        IMapper Mapper { get; set; }
        ILoggerFactory LoggerFactory { get; set; }
    }
}
