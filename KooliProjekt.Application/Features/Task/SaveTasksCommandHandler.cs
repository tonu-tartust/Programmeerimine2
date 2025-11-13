using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Paging;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KooliProjekt.Application.Features.ToDoLists
{
    public class SaveTasksCommandHandler : IRequestHandler<SaveTasksCommand, OperationResult>
    {
        private readonly ApplicationDbContext _dbContext;

        public SaveTasksCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult> Handle(SaveTasksCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult();

            var list = new Tasks();
            if (request.Id == 0)
            {
                await _dbContext.Taskss.AddAsync(list);
            }
            else
            {
                list = await _dbContext.Taskss.FindAsync(request.Id);
                //_dbContext.ToDoLists.Update(list);
            }

            list.Id = request.Id;

            await _dbContext.SaveChangesAsync();

            return result;
        }
    }
}
