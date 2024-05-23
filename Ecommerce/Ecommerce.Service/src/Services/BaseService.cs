using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Service.src.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Ecommerce.Service.src.Services
{
    public class BaseService<TEntity, TReadDTO, TCreateDTO, TUpdateDTO, QueryOptions> : IBaseService<TReadDTO, TCreateDTO, TUpdateDTO, QueryOptions>
       where TEntity : BaseEntity, new()
    {
        protected readonly IBaseRepository<TEntity, QueryOptions> _repository;
        protected readonly IMapper _mapper;
        protected readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public BaseService(IBaseRepository<TEntity, QueryOptions> repository, IMapper mapper, IMemoryCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
            _cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
        }
        public virtual async Task<TReadDTO> CreateOneAsync(TCreateDTO createDto)
        {
            var entity = _mapper.Map<TEntity>(createDto);
            if (await _repository.ExistsAsync(entity))
            {
                throw AppException.DuplicateException();
            }
            entity = await _repository.CreateAsync(entity);
            _cache.Remove($"GetAll-{typeof(TEntity).Name}");
            return _mapper.Map<TReadDTO>(entity);
        }

        public virtual async Task<bool> DeleteOneAsync(Guid id)
        {
            if (!await _repository.DeleteAsync(id))
            {
                return false;
                throw AppException.NotFound();
            }
            _cache.Remove($"GetById-{id}");
            _cache.Remove($"GetAll-{typeof(TEntity).Name}");
            return true;
        }

        public virtual async Task<PaginatedResult<TReadDTO>> GetAllAsync(QueryOptions options)
        {
            string cacheKey = $"GetAll-{typeof(TEntity).Name}-{options!.GetHashCode()}";

            if (!_cache.TryGetValue(cacheKey, out PaginatedResult<TEntity>? paginatedResult))
            {
                paginatedResult = await _repository.GetAllAsync(options);
                _cache.Set(cacheKey, paginatedResult, _cacheOptions);
            }
            var mappedItems = _mapper.Map<IEnumerable<TReadDTO>>(paginatedResult!.Items);
            return new PaginatedResult<TReadDTO>(mappedItems, paginatedResult.TotalCount);
        }

        public virtual async Task<TReadDTO> GetOneByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id) ?? throw AppException.NotFound();
            return _mapper.Map<TReadDTO>(entity);
        }

        public virtual async Task<TReadDTO> UpdateOneAsync(Guid id, TUpdateDTO updateDto)
        {
            var existingEntity = await _repository.GetByIdAsync(id) ?? throw AppException.NotFound();
            var updatedEntity = _mapper.Map(updateDto, existingEntity);
            foreach (var property in updateDto!.GetType().GetProperties())
            {
                var propertyName = property.Name;
                var propertyValue = property.GetValue(updateDto);
                // Find the corresponding property on the entity object
                var entityProperty = updateDto.GetType().GetProperty(propertyName);
                // Check if the entity property exists and is writable
                if (entityProperty != null && entityProperty.CanWrite)
                {
                    // Set the value of the entity property
                    entityProperty.SetValue(updateDto, propertyValue);
                }
            }
            var result = await _repository.UpdateAsync(updatedEntity);
            _cache.Remove($"GetById-{id}");
            _cache.Remove($"GetAll-{typeof(TEntity).Name}");
            return _mapper.Map<TReadDTO>(result);
        }
    }
}