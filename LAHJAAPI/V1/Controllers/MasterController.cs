using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using V1.DyModels.VMs;
using V1.Services.Services;

namespace LAHJAAPI.V1.Controllers
{
    [AllowAnonymous]
    [Route("api/v1/[controller]")]
    [ApiController]

    public class MasterController
        : ControllerBase
    {
        private readonly ILogger<MasterController> _logger;
        private readonly IMapper _mapper;
        private readonly IUseLanguageService _languageService;
        private readonly IUseCategoryModelService _categoryModelService;
        private readonly IUseTypeModelService _typeModelService;

        public MasterController(
            ILogger<MasterController> logger,
            IMapper mapper,
            IUseLanguageService useLanguageService,
            IUseCategoryModelService categoryModelService,
            IUseTypeModelService typeModelService)
        {
            _logger = logger;
            _mapper = mapper;
            _languageService = useLanguageService;
            _categoryModelService = categoryModelService;
            _typeModelService = typeModelService;
        }

        #region Language
        [HttpGet("GetLanguages")]
        public async Task<ActionResult<List<LanguageOutputVM>>> GetLanguages(string? lg = null)
        {
            return RedirectToRoute("GetLanguages", new { lg });
        }

        [HttpGet("GetLanguageByCode/{code}")]
        public async Task<ActionResult<List<LanguageOutputVM>>> GetLanguageByCode(string code, string lg = "en")
        {
            return RedirectToRoute("GetLanguageByCode", new { code, lg });
        }

        #endregion

        #region Category
        [HttpGet("GetCategoryModelByName/{name}")]
        public IActionResult GetCategoryModelByName(string name, string lg = "en")
        {
            return RedirectToRoute("GetCategoryModelByName", new { name, lg });

        }

        #endregion

        #region Type
        [HttpGet("GetTypeByName/{name}")]
        public IActionResult GetTypeByName(string name, string lg = "en")
        {
            return RedirectToRoute("GetTypeModelByName", new { name, lg });
        }

        [HttpGet("types/active")]
        public IActionResult GetActiveTypes(string lg = "en")
        {
            return RedirectToRoute("GetActiveTypeModels", lg);
        }

        #endregion

        #region Dialect
        [HttpGet("dialect/{languageId}")]
        public ActionResult<DialectOutputVM> GetDialectByLanguage(string languageId, string lg = "en")
        {
            return RedirectToRoute("GetDialectByLanguage", new { languageId, lg });
        }

        [HttpGet("dialects/{languageId}")]
        public async Task<ActionResult<List<DialectOutputVM>>> GetDialectsByLanguage(string languageId, string lg = "en")
        {
            return RedirectToRoute("GetDialectsByLanguage", new { languageId, lg });
        }

        #endregion

        #region Advertisement
        [HttpGet("advertisements/{id}")]
        public ActionResult<AdvertisementOutputVM> GetActiveAdvertisementById(string id, string lg = "en")
        {
            return RedirectToRoute("GetAdvertisement", new { id, lg });
        }

        [HttpGet("GetActiveAdvertisements")]
        public async Task<ActionResult<List<AdvertisementOutputVM>>> GetActiveAdvertisements(string lg = "en")
        {
            return RedirectToRoute("GetActiveAdvertisements", lg);
        }


        #endregion

        #region AdvertisementTab
        [HttpGet("advertisementtab/{id}")]
        public ActionResult<AdvertisementTabOutputVM> GetAdvertisementTabbyId(string id, string lg = "en")
        {
            return RedirectToRoute("GetAdvertisementTab", new { id, lg });
        }

        [HttpGet("advertisementtabs/{advertisementId}")]
        public ActionResult<List<AdvertisementTabOutputVM>> GetByAdvertisementId(string advertisementId, string lg = "en")
        {
            return RedirectToRoute("GetByAdvertisementId", new { advertisementId, lg });
        }

        #endregion
    }
}