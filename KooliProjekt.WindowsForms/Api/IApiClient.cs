using System;
using System.Collections.Generic;
using System.Text;

namespace KooliProjekt.WindowsForms.Api
{
    public interface IApiClient
    {
        Task<OperationResult<PagedResult<Employees>>> List(int page, int pageSize);
        Task<OperationResult> Save(Employees list);
        Task<OperationResult> Delete(int id);
    }
}