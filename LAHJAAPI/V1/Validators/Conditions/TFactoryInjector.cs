using APILAHJA.Utilities;
using AutoMapper;
using LAHJAAPI.Data;

namespace LAHJAAPI.V1.Validators.Conditions
{
    public class TFactoryInjector : ITFactoryInjector
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IUserClaimsHelper _userClaims;

        // يمكنك حقن اي طبقة
        public TFactoryInjector(IMapper mapper, DataContext context, IUserClaimsHelper userClaims)
        {
            _mapper = mapper;
            _context = context;
            _userClaims = userClaims;
        }

        public IMapper Mapper => _mapper;
        public DataContext Context => _context;
        public IUserClaimsHelper UserClaims => _userClaims;

    }
}