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

        //public async Task<bool> ExistsAsync(object value, string name = "Id")
        // {
        //     return await base.ExistsAsync(value, name);
        // }   
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