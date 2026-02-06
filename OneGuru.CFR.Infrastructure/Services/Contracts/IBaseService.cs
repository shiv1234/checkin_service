using OneGuru.CFR.Domain.RequestModel;
using OneGuru.CFR.Persistence.EntityFrameworkDataAccess;
using System.Net.Http;
namespace OneGuru.CFR.Infrastructure.Services.Contracts
{
    public interface IBaseService
    {
        IUnitOfWorkAsync UnitOfWorkAsync { get; set; }
        IOperationStatus OperationStatus { get; set; }
        CfrDbContext CfrDbContext { get; set; }
    }
}
