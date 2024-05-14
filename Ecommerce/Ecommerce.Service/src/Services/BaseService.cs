using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Service.src.Interfaces;

namespace Ecommerce.Service.src.Services
{
    public class BaseService<TReadDTO, TCreateDTO, TUpdateDTO, TEntity, QueryOptions> : IBaseService<TReadDTO, TCreateDTO, TUpdateDTO, QueryOptions>
       where TEntity : BaseEntity, new()
    {
        protected readonly IBaseRepository<TEntity, QueryOptions> _repository;
        protected readonly IMapper _mapper;

        public BaseService(IBaseRepository<TEntity, QueryOptions> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public virtual async Task<TReadDTO> CreateOneAsync(TCreateDTO createDto)
        {
            var entity = _mapper.Map<TEntity>(createDto);
            if (await _repository.ExistsAsync(entity))
            {
                throw AppException.DuplicateException();
            }
            entity = await _repository.CreateAsync(entity);
            return _mapper.Map<TReadDTO>(entity);
        }

        public virtual async Task<bool> DeleteOneAsync(Guid id)
        {
            if (!await _repository.DeleteAsync(id))
            {
                throw AppException.NotFound();
            }
            return true;
        }

        public virtual async Task<IEnumerable<TReadDTO>> GetAllAsync(QueryOptions options)
        {
            var entities = await _repository.GetAllAsync(options);
            return _mapper.Map<IEnumerable<TReadDTO>>(entities);
        }

        public virtual async Task<TReadDTO> GetOneByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw AppException.NotFound();
            }
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
            if (await _repository.ExistsAsync(updatedEntity))
            {
                throw AppException.DuplicateException();
            }
            var result = await _repository.UpdateAsync(updatedEntity) ?? throw AppException.BadRequest();
            return _mapper.Map<TReadDTO>(result);
        }
    }
}