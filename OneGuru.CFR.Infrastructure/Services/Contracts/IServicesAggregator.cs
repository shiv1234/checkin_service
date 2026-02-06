using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OneGuru.CFR.Persistence.EntityFrameworkDataAccess;

namespace OneGuru.CFR.Infrastructure.Services.Contracts
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
