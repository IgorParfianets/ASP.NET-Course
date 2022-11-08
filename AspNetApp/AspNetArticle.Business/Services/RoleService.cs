using AspNetArticle.Core.Abstractions;
using AspNetArticle.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AspNetArticle.Business.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public RoleService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
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
            var role = await _unitOfWork.Roles
                .FindBy(role => 
                    role.Name.Equals(name))
                .FirstOrDefaultAsync();

            return role?.Id ?? Guid.Empty;
        }
    }
}
