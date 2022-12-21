using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Core.Abstractions;
using AspNetArticle.Data.Abstractions;
using AspNetArticle.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AspNetArticle.Business.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public RoleService(IUnitOfWork unitOfWork, 
            IConfiguration configuration, 
            IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mediator = mediator;
        }

        public async Task<string> GetRoleNameByIdAsync(Guid id) 
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(id);

            return role != null 
                ? role.Name 
                : string.Empty;
        }

        public async Task<Guid?> GetRoleIdByNameAsync(string name)
        {
            var roleId = await _mediator.Send(new GetRoleIdByNameQuery() { RoleName = name });

            return roleId != null
                ? roleId
                : Guid.Empty;
        }
    }
}
