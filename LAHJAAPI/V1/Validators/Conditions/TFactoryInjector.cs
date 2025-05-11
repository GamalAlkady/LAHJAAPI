using AutoGenerator.Conditions;
using AutoGenerator.Notifications;
using AutoMapper;
using LAHJAAPI.Services2;
using LAHJAAPI.Utilities;
using Microsoft.Extensions.Options;

namespace LAHJAAPI.V1.Validators.Conditions
{
    public class TFactoryInjector : TBaseFactoryInjector, ITFactoryInjector
    {
        private readonly SingletonContextFactory _contextFactory;
        public TFactoryInjector(
            IMapper mapper,
            IAutoNotifier notifier,
            IOptions<AppSettings> appSettings,
            TokenService tokenService,
            SingletonContextFactory contextFactory) : base(mapper, notifier)
        {
            TokenService = tokenService;
            _contextFactory = contextFactory;
            AppSettings = appSettings.Value;
        }

        public SingletonContextFactory ContextFactory => _contextFactory;
        public TokenService TokenService { get; }
        public AppSettings AppSettings { get; }
        // يمكنك حقن اي طبقة
    }
}