using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Data.Abstractions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AspNetArticle.Business.Services
{
    public class SourceService : ISourceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SourceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SourceDto>> GetSourcesAsync()
        {
            return await _unitOfWork.Sources.Get()
                .Select(source => _mapper.Map<SourceDto>(source))
                .ToListAsync();
        }

        public async Task<SourceDto> GetSourcesByIdAsync(Guid id)
        {
            return _mapper.Map<SourceDto>(await _unitOfWork.Sources.GetByIdAsync(id));
        }

        public async Task RemoveSourceById(Guid id)
        {
            var source = await _unitOfWork.Sources.GetByIdAsync(id);

            if(source != null)
            {
                _unitOfWork.Sources.Remove(source);
                await _unitOfWork.Commit();
            }
        }
    }
}
