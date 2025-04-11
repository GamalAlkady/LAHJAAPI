using AutoMapper;
using LAHJAAPI.Data;

namespace LAHJAAPI.V1.Validators.Conditions
{
    public class TFactoryInjector : ITFactoryInjector
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        // يمكنك حقن اي طبقة
        public TFactoryInjector(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public IMapper Mapper => _mapper;
        public DataContext Context => _context;

    }
}