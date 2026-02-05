using UnlockOKR.OKRSupoort.Domain.RequestModel;
using UnlockOKR.OKRSupoort.Persistence.EntityFrameworkDataAccess;
using System.Net.Http;
namespace UnlockOKR.OKRSupoort.Infrastructure.Services.Contracts
{
    public interface IBaseService
    {
        IUnitOfWorkAsync UnitOfWorkAsync { get; set; }
        IOperationStatus OperationStatus { get; set; }
        OkrSupportDbContext OkrTaskDbContext { get; set; }
    }
}
