using AspNetArticle.Data.Abstractions;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetArticle.Core.DataTransferObjects;
using Microsoft.EntityFrameworkCore;

namespace AspNetArticle.Business.Services
{
    public class AdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public AdminService(IMapper mapper,
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        //public async Task<DataForAdminDto> GetAllUsersData()
        //{

        //    var users = await _unitOfWork.Users
        //        .Get()
        //        .Include(com => com.Comments)
        //}
    }
}
