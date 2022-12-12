using AspNetArticle.Api.Utils;
using AspNetArticle.Core.Abstractions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AspNetArticle.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IJwtUtil _jwtUtil;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public AdminController(IUserService userService,
            IRoleService roleService,
            IMapper mapper,
            IJwtUtil jwtUtil, IConfiguration configuration)
        {
            _userService = userService;
            _roleService = roleService;
            _mapper = mapper;
            _jwtUtil = jwtUtil;
            _configuration = configuration;
        }
    }
}
