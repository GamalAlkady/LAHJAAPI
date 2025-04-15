using APILAHJA.Utilities;
using AutoGenerator.Conditions;
using LAHJAAPI.Data;

namespace LAHJAAPI.V1.Validators.Conditions
{
    public interface ITFactoryInjector : ITBaseFactoryInjector
    {
        public DataContext Context { get; }
        public IUserClaimsHelper UserClaims { get; }
    }
}