using AutoGenerator;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using AutoGenerator.Services.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using LAHJAAPI.Models;
using V1.DyModels.Dto.Share.Requests;
using V1.DyModels.Dto.Share.Responses;
using V1.Repositories.Share;
using System.Linq.Expressions;
using V1.Repositories.Builder;
using AutoGenerator.Repositories.Base;
using AutoGenerator.Helper;
using System;

namespace V1.Services.Services
{
    public class AuthorizationSessionServiceService : BaseService<AuthorizationSessionServiceRequestDso, AuthorizationSessionServiceResponseDso>, IUseAuthorizationSessionServiceService
    {
        private readonly IAuthorizationSessionServiceShareRepository _share;
        public AuthorizationSessionServiceService(IAuthorizationSessionServiceShareRepository buildAuthorizationSessionServiceShareRepository, IMapper mapper, ILoggerFactory logger) : base(mapper, logger)
        {
            _share = buildAuthorizationSessionServiceShareRepository;
        }

        public override Task<int> CountAsync()
        {
            try
            {
                _logger.LogInformation("Counting AuthorizationSessionService entities...");
                return _share.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CountAsync for AuthorizationSessionService entities.");
                return Task.FromResult(0);
            }
        }

        public override async Task<AuthorizationSessionServiceResponseDso> CreateAsync(AuthorizationSessionServiceRequestDso entity)
        {
            try
            {
                _logger.LogInformation("Creating new AuthorizationSessionService entity...");
                var result = await _share.CreateAsync(entity);
                var output = GetMapper().Map<AuthorizationSessionServiceResponseDso>(result);
                _logger.LogInformation("Created AuthorizationSessionService entity successfully.");
                return output;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating AuthorizationSessionService entity.");
                return null;
            }
        }

        public override Task DeleteAsync(string id)
        {
            try
            {
                _logger.LogInformation($"Deleting AuthorizationSessionService entity with ID: {id}...");
                return _share.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while deleting AuthorizationSessionService entity with ID: {id}.");
                return Task.CompletedTask;
            }
        }

        public override async Task<IEnumerable<AuthorizationSessionServiceResponseDso>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all AuthorizationSessionService entities...");
                var results = await _share.GetAllAsync();
                return GetMapper().Map<IEnumerable<AuthorizationSessionServiceResponseDso>>(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllAsync for AuthorizationSessionService entities.");
                return null;
            }
        }

        public override async Task<AuthorizationSessionServiceResponseDso?> GetByIdAsync(string id)
        {
            try
            {
                _logger.LogInformation($"Retrieving AuthorizationSessionService entity with ID: {id}...");
                var result = await _share.GetByIdAsync(id);
                var item = GetMapper().Map<AuthorizationSessionServiceResponseDso>(result);
                _logger.LogInformation("Retrieved AuthorizationSessionService entity successfully.");
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetByIdAsync for AuthorizationSessionService entity with ID: {id}.");
                return null;
            }
        }

        public override IQueryable<AuthorizationSessionServiceResponseDso> GetQueryable()
        {
            try
            {
                _logger.LogInformation("Retrieving IQueryable<AuthorizationSessionServiceResponseDso> for AuthorizationSessionService entities...");
                var queryable = _share.GetQueryable();
                var result = GetMapper().ProjectTo<AuthorizationSessionServiceResponseDso>(queryable);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetQueryable for AuthorizationSessionService entities.");
                return null;
            }
        }

        public override async Task<AuthorizationSessionServiceResponseDso> UpdateAsync(AuthorizationSessionServiceRequestDso entity)
        {
            try
            {
                _logger.LogInformation("Updating AuthorizationSessionService entity...");
                var result = await _share.UpdateAsync(entity);
                return GetMapper().Map<AuthorizationSessionServiceResponseDso>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateAsync for AuthorizationSessionService entity.");
                return null;
            }
        }

        public override async Task<bool> ExistsAsync(object value, string name = "Id")
        {
            try
            {
                _logger.LogInformation("Checking if AuthorizationSessionService exists with {Key}: {Value}", name, value);
                var exists = await _share.ExistsAsync(value, name);
                if (!exists)
                {
                    _logger.LogWarning("AuthorizationSessionService not found with {Key}: {Value}", name, value);
                }

                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while checking existence of AuthorizationSessionService with {Key}: {Value}", name, value);
                return false;
            }
        }

        public override async Task<PagedResponse<AuthorizationSessionServiceResponseDso>> GetAllAsync(string[]? includes = null, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching all AuthorizationSessionServices with pagination: Page {PageNumber}, Size {PageSize}", pageNumber, pageSize);
                var results = (await _share.GetAllAsync(includes, pageNumber, pageSize));
                var items = GetMapper().Map<List<AuthorizationSessionServiceResponseDso>>(results.Data);
                return new PagedResponse<AuthorizationSessionServiceResponseDso>(items, results.PageNumber, results.PageSize, results.TotalPages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all AuthorizationSessionServices.");
                return new PagedResponse<AuthorizationSessionServiceResponseDso>(new List<AuthorizationSessionServiceResponseDso>(), pageNumber, pageSize, 0);
            }
        }

        public override async Task<AuthorizationSessionServiceResponseDso?> GetByIdAsync(object id)
        {
            try
            {
                _logger.LogInformation("Fetching AuthorizationSessionService by ID: {Id}", id);
                var result = await _share.GetByIdAsync(id);
                if (result == null)
                {
                    _logger.LogWarning("AuthorizationSessionService not found with ID: {Id}", id);
                    return null;
                }

                _logger.LogInformation("Retrieved AuthorizationSessionService successfully.");
                return GetMapper().Map<AuthorizationSessionServiceResponseDso>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving AuthorizationSessionService by ID: {Id}", id);
                return null;
            }
        }

        public override async Task DeleteAsync(object value, string key = "Id")
        {
            try
            {
                _logger.LogInformation("Deleting AuthorizationSessionService with {Key}: {Value}", key, value);
                await _share.DeleteAsync(value, key);
                _logger.LogInformation("AuthorizationSessionService with {Key}: {Value} deleted successfully.", key, value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting AuthorizationSessionService with {Key}: {Value}", key, value);
            }
        }

        public override async Task DeleteRange(List<AuthorizationSessionServiceRequestDso> entities)
        {
            try
            {
                var builddtos = entities.OfType<AuthorizationSessionServiceRequestShareDto>().ToList();
                _logger.LogInformation("Deleting {Count} AuthorizationSessionServices...", 201);
                await _share.DeleteRange(builddtos);
                _logger.LogInformation("{Count} AuthorizationSessionServices deleted successfully.", 202);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting multiple AuthorizationSessionServices.");
            }
        }

        public override async Task<PagedResponse<AuthorizationSessionServiceResponseDso>> GetAllByAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {
            try
            {
                _logger.LogInformation("Retrieving all AuthorizationSessionService entities...");
                var results = await _share.GetAllAsync();
                var response = await _share.GetAllByAsync(conditions, options);
                return response.ToResponse(GetMapper().Map<IEnumerable<AuthorizationSessionServiceResponseDso>>(response.Data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllAsync for AuthorizationSessionService entities.");
                return null;
            }
        }

        public override async Task<AuthorizationSessionServiceResponseDso?> GetOneByAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {
            try
            {
                _logger.LogInformation("Retrieving AuthorizationSessionService entity...");
                return GetMapper().Map<AuthorizationSessionServiceResponseDso>(await _share.GetOneByAsync(conditions, options));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOneByAsync  for AuthorizationSessionService entity.");
                return null;
            }
        }
    }
}