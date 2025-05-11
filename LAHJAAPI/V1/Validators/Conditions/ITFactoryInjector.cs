using AutoGenerator.Conditions;
using LAHJAAPI.Services2;
using LAHJAAPI.Utilities;

namespace LAHJAAPI.V1.Validators.Conditions
{
    public interface ITFactoryInjector : ITBaseFactoryInjector
    {
        public SingletonContextFactory ContextFactory { get; }
        public TokenService TokenService { get; }
        public AppSettings AppSettings { get; }
    }
}