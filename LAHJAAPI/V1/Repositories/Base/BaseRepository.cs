using AutoGenerator;
using AutoGenerator.Repositories.Base;
using LAHJAAPI.Data;
using LAHJAAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using V1.BPR.Layers.Base;

namespace V1.Repositories.Base
{
    /// <summary>
    /// BaseRepository class for ShareRepository.
    /// </summary>
    public sealed class BaseBPRRepository<T> : TBaseBPRRepository<ApplicationUser, IdentityRole, string, T>, IBPRLayer<T, T>, IBaseRepository<T> where T : class
    {
        public BaseBPRRepository(DataContext db, ILoggerFactory logger) : base(db, logger)
        {
        }


        public override async Task<T?> CreateAsync(T entity)
        {
            try
            {
                T item = (await DbSet.AddAsync(entity)).Entity;
                await SaveAsync();
                return item;
            }
            catch (RepositoryException exception)
            {
                _logger.LogError(exception, "Error creating entity");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error creating entity");
                throw;
            }
        }
        public async Task<bool> ExecuteTransactionAsync(Func<Task<bool>> operation)
        {
            return await base.ExecuteTransactionAsync(operation);
        }

        public new async Task<bool> ExistsAsync(object value, string name = "Id")
        {
            return await base.ExistsAsync(e => EF.Property<object>(e, name) == value);
        }

        public async Task<PagedResponse<T>> GetAllAsync(string[]? includes = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = GetQueryable(includes, false);
            return await query.ToPagedResponseAsync(pageNumber, pageSize);
        }

        public async Task<IEnumerable<T>> GetAllAsync(string propertyName, object value, string[]? includes = null)
        {
            var query = GetQueryable(includes, false);
            query = query.Where(e => EF.Property<object>(e, propertyName) == value);
            return await query.ToListAsync();
            //return await base.GetAllAsync(e=>EF.Property<object>(e,propertyName)==value,s=>s.Include());
        }
        public Task<IEnumerable<T>> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<DataResult<IEnumerable<T>>> GetByNameDataResult(string name)
        {
            throw new NotImplementedException();
        }
    }
}