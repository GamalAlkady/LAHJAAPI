using AutoGenerator.Services.Base;
using FluentResults;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.VMs;

namespace V1.Services.Services
{
    public interface IUseAuthorizationSessionService : IAuthorizationSessionService<AuthorizationSessionRequestDso, AuthorizationSessionResponseDso>, IBaseService, IBaseBPRServiceLayer<AuthorizationSessionRequestDso, AuthorizationSessionResponseDso>

    {
        Task<AuthorizationSessionResponseDso> CreateForMyAsignedServices(DataAuthSession dataAuthSession);
        Task<AuthorizationSessionResponseDso> PrepareSession(DataAuthSession dataAuthSession, params Func<Dictionary<string, object>, DataAuthSession, Task<Dictionary<string, object>>>[] conditions);
        Task<AuthorizationSessionResponseDso> CreateSession(DataAuthSession dataAuthSession);
        Task<Result<AuthorizationSessionResponseDso>> GetSessionByServices(string userId, List<string> servicesIds, string authorizationType);
        string SimulationCore(string encrptedToken, string coreToken);
        string SimulationPlatForm(EncryptTokenRequest encryptToken);
    }
}