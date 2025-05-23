using APILAHJA.Utilities;
using AutoGenerator.Helper;
using AutoGenerator.Helper.Translation;
using AutoMapper;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Enums;
using V1.DyModels.Dso.Responses;
using V1.DyModels.VM;
using V1.Services.Services;

namespace LAHJAAPI.V1.Services.Statistics
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IUseUserServiceService _userServiceService;
        private readonly IUseSubscriptionService _subscriptionService;
        private readonly IUseRequestService _requestService;
        private readonly ILogger<StatisticsService> _logger;
        private readonly IUserClaimsHelper _userClaims;
        private readonly IUseServiceService _serviceService;
        private readonly IMapper _mapper;
        public StatisticsService(
            IUseApplicationUserService userService,
            IUseUserServiceService userServiceService,
            IUseSubscriptionService subscriptionService,
            IUseRequestService requestService,
            ILogger<StatisticsService> logger,
            IUserClaimsHelper userClaims,
            IUseServiceService serviceService,
            IMapper mapper)
        {
            _userServiceService = userServiceService;
            _subscriptionService = subscriptionService;
            _requestService = requestService;
            _logger = logger;
            _userClaims = userClaims;
            _serviceService = serviceService;
            _mapper = mapper;

        }

        public async Task<List<UsedRequestsVm>> GetServiceUsageDataAsync()
        {
            var response = await _userServiceService.GetAllByAsync([new FilterCondition(nameof(UserServiceResponseDso.UserId), _userClaims.UserId)], new ParamOptions(["Service.Requests"]));
            if (response.TotalRecords > 0)
            {

                return response.Data.Select(s => new UsedRequestsVm
                {
                    Name = s.Service.Name,
                    UsageCount = s.Service.Requests.Count(r => r.Status == RequestStatus.Success.ToString()),
                }).ToList();
            }
            return [];
        }

        public async Task<IEnumerable<ServiceUsersCount>> ServiceUsersCount()
        {
            var response = await _serviceService.GetAllAsync([nameof(Service.UserServices)], pageSize: 100);
            var items = response.Data;
            var r = items.Select(s => new ServiceUsersCount
            {
                ServiceType = s.Name,
                Count = s.UserServices.Count()
            }).ToList();
            return r;
        }

        public async Task<IEnumerable<UsedRequestsVm>> GetServiceUsageAndRemaining()
        {
            var response = await _userServiceService.GetAllByAsync([new FilterCondition("UserId", _userClaims.UserId)], new ParamOptions(["Service.Requests"]));
            if (response.TotalRecords == 0) return [];

            var numberRequests = await _subscriptionService.GetNumberRequests();
            var result = response.Data.Select(s =>
            {
                var usageCount = s.Service?.Requests?.Count(r => r.Status == RequestStatus.Success.ToString()) ?? 0;
                return new UsedRequestsVm
                {
                    Name = s.Service.Name,
                    UsageCount = usageCount,
                    Remaining = (numberRequests - usageCount).ToString() // إعادة استخدام القيمة المحسوبة هنا
                };
            }).ToList();


            return result;
        }


        public async Task<UsedRequestsVm> GetUsageAndRemainingRequests()
        {
            var numberRequests = await _subscriptionService.GetNumberRequests();

            var allowedRequests = await _subscriptionService.AllowedRequests(false);
            if (int.TryParse(allowedRequests, out var allowed))
            {
                allowedRequests = (allowed - numberRequests).ToString();
            }
            return new UsedRequestsVm
            {
                UsageCount = numberRequests,
                Remaining = allowedRequests
            };
        }

        public async Task<IEnumerable<RequestData>> GetRequestsByDatetime(FilterBy filterBy, RequestType requestTypes, DateTime? StartDate, DateTime? EndDate, DateTimeFilter groupBy)
        {
            AutoGenerator.PagedResponse<global::V1.DyModels.Dso.Responses.RequestResponseDso> response = await FilterRequests(requestTypes, StartDate, EndDate);

            if (response.TotalRecords == 0) return [];
            var rawData = response.Data
                .GroupBy(r => new
                {
                    Name =
                      (filterBy == FilterBy.Service) ? r.Service.Name
                    : (filterBy == FilterBy.Space) ? r.Space.Name : r.ModelAi,
                    DateTime =
                        groupBy == DateTimeFilter.Year ? r.UpdatedAt.Year.ToString()
                        : groupBy == DateTimeFilter.Month ? r.UpdatedAt.Year.ToString() + "-" + r.UpdatedAt.Month.ToString()
                        : groupBy == DateTimeFilter.Day ? r.UpdatedAt.Date.ToString()
                        : r.UpdatedAt.Date.ToString() + " " + r.UpdatedAt.Hour
                })
            .Select(g => new RequestData
            {
                Name = g.Key.Name,
                Requests = g.Count(r => r.Status == RequestStatus.Success.ToString()),
                Errors = g.Count(r => r.Status == RequestStatus.Failed.GetDisplayName() || r.Status == RequestStatus.FailedApiCore.GetDisplayName()),
                DateTime = g.Key.DateTime
            })
               .OrderByDescending(g => g.DateTime); // ترتيب تنازلي حسب عدد الأخطاء;

            var data = rawData.ToList(); // نحصل على البيانات أولًا قبل التجميع
            return data;
        }

        private async Task<AutoGenerator.PagedResponse<global::V1.DyModels.Dso.Responses.RequestResponseDso>> FilterRequests(RequestType requestTypes, DateTime? StartDate, DateTime? EndDate)
        {
            List<FilterCondition> filterConditions = [new FilterCondition("UserId", _userClaims.UserId)];
            if (RequestType.Errors == requestTypes)
                filterConditions.Add(new FilterCondition("Status", new[] { RequestStatus.Failed.ToString(), RequestStatus.FailedApiCore.ToString() }, FilterOperator.In));
            else if (RequestType.Requests == requestTypes)
                filterConditions.Add(new FilterCondition("Status", new[] { RequestStatus.Success.ToString(), RequestStatus.Processing.ToString() }, FilterOperator.In));

            if (StartDate != null)
                filterConditions.Add(new FilterCondition("UpdatedAt", StartDate, FilterOperator.GreaterThanOrEqual));

            if (EndDate != null)
                filterConditions.Add(new FilterCondition("UpdatedAt", EndDate, FilterOperator.LessThanOrEqual));

            var response = await _requestService.GetAllByAsync(filterConditions, new ParamOptions(["Service"]));
            return response;
        }

        public async Task<IEnumerable<RequestData>> GetRequests(FilterBy filterBy, RequestType requestTypes, DateTime? StartDate, DateTime? EndDate)
        {
            AutoGenerator.PagedResponse<global::V1.DyModels.Dso.Responses.RequestResponseDso> response = await FilterRequests(requestTypes, StartDate, EndDate);
            if (response.TotalRecords == 0) return [];

            var rawData = response.Data
                .GroupBy(r => new
                {
                    Name =
                      (filterBy == FilterBy.Service) ? r.Service.Name
                    : (filterBy == FilterBy.Space) ? r.Space.Name : r.ModelAi
                })
            .Select(g => new RequestData
            {
                Name = g.Key.Name,
                Requests = g.Count(r => r.Status == RequestStatus.Success.GetDisplayName()),
                Errors = g.Count(r => r.Status == RequestStatus.Failed.GetDisplayName() || r.Status == RequestStatus.FailedApiCore.GetDisplayName()),
            })
               .OrderByDescending(g => g.Requests); // ترتيب تنازلي حسب عدد الأخطاء;

            var data = rawData.ToList(); // نحصل على البيانات أولًا قبل التجميع
            return data;
        }

        public async Task<IEnumerable<ServiceDataTod>> GetRequestsByStatus(FilterBy filterBy, RequestType requestTypes, DateTime? StartDate, DateTime? EndDate)
        {
            AutoGenerator.PagedResponse<global::V1.DyModels.Dso.Responses.RequestResponseDso> response = await FilterRequests(requestTypes, StartDate, EndDate);
            if (response.TotalRecords == 0) return [];
            var failedStatuses = new HashSet<string> { RequestStatus.Failed.GetDisplayName(), RequestStatus.FailedApiCore.GetDisplayName() };



            var rawData = response.Data
                .GroupBy(r => new
                {
                    Name =
                      (filterBy == FilterBy.Service) ? r.Service.Name
                    : (filterBy == FilterBy.Space) ? r.Space.Name : r.ModelAi,
                    Status = (failedStatuses.Contains(r.Status)) ? "errors" : "requests"
                })
            .Select(g => new ServiceDataTod
            {
                Name = g.Key.Name,
                Value = g.Count(),
                Status = g.Key.Status
            })
               .OrderByDescending(g => g.Value); // ترتيب تنازلي حسب عدد الأخطاء;

            var data = rawData.ToList(); // نحصل على البيانات أولًا قبل التجميع
            return data;
        }

        public async Task<IEnumerable<ModelAiServiceData>> GetModelAiServicesRequests(string lg)
        {
            List<FilterCondition> filterConditions = [new FilterCondition("UserId", _userClaims.UserId)];
            var response = await _requestService.GetAllByAsync(filterConditions, new ParamOptions(["Service.ModelAi"]));
            if (response.TotalRecords == 0) return [];

            var items = response.Data.GroupBy(s => lg == null ? s.Service.ModelAi!.Name : HelperTranslation.getTranslationValueByLG(s.Service.ModelAi!.Name, lg))
          .Select(s => new ModelAiServiceData
          {
              ModelAi = s.Key,
              UsageCount = s.Count(),
          }).ToList();
            return items;
        }
    }
}
