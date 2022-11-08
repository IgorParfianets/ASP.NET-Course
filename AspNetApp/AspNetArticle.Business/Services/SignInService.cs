using System.Security.Claims;
using AspNetArticle.Data.Abstractions;
using AutoMapper;
using Microsoft.Extensions.Configuration;

namespace AspNetArticle.Business.Services
{
    public class SignInService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public SignInService(IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }



    }
}
