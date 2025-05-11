using AutoGenerator.Repositories.Base;
using AutoGenerator.Services.Base;
using FluentResults;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.VMs;

namespace V1.Services.Services
{
    public interface IUseAuthorizationSessionService : IAuthorizationSessionService<AuthorizationSessionRequestDso, AuthorizationSessionResponseDso>, IBaseService//يمكنك  التزويد بكل  دوال   طبقة Builder   ببوابات  الطبقة   هذه نفسها
    //, IAuthorizationSessionBuilderRepository<AuthorizationSessionRequestDso, AuthorizationSessionResponseDso>
    , IBasePublicRepository<AuthorizationSessionRequestDso, AuthorizationSessionResponseDso>
    {
        Task<AuthorizationSessionResponseDso> CreateForAllServices(DataAuthSession dataAuthSession);
        Task<AuthorizationSessionResponseDso> CreateForDashboard(DataAuthSession dataAuthSession);
        Task<AuthorizationSessionResponseDso> CreateGeneralSession(DataAuthSession dataAuthSession);
        Task<AuthorizationSessionResponseDso> CreateSecretSession(DataAuthSession dataAuthSession);
        Task<AuthorizationSessionResponseDso> GetOrCreateSession(List<string> servicesIds, string type, DateTime? expire, string modelAiId);
        Task<Result<AuthorizationSessionResponseDso>> GetSessionByServices(string userId, List<string> servicesIds, string authorizationType);
        string SimulationCore(string encrptedToken, string coreToken);
        string SimulationPlatForm(EncryptTokenRequest encryptToken);
    }
}