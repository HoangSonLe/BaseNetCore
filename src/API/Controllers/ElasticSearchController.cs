using Application.Base;
using Application.Services.WebInterfaces;
using Infrastructure.Elastic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("/api/[controller]")]
    [AllowAnonymous]
    public class ElasticSearchController : BaseController<ElasticSearchController>
    {
        private readonly IBaseElasticSearchService _elasticSearchService;
        public ElasticSearchController(
            ILogger<ElasticSearchController> logger,
            IUserService userService,
            IBaseElasticSearchService elasticSearchService
            ) : base(logger, userService)
        {
            _elasticSearchService = elasticSearchService;
        }
        [HttpGet]
        public async Task<IActionResult> Test()
        {
            return Ok();
        }

        [HttpGet]
        [Route("test-simple-query")]
        public async Task<IActionResult> TestSimpleQuery()
        {
            var response = await _elasticSearchService.TestSimpleQuery();
            return MapToIActionResult(response);
        }

        [HttpPost]
        [Route("post-simple-data")]
        public async Task<IActionResult> PostSimpleData()
        {
            var response = await _elasticSearchService.PostSimpleData();
            return MapToIActionResult(response);
        }

        [HttpPost]
        [Route("post-simple-data-using-client-elastic-search")]
        public async Task<IActionResult> PostSimpleDataUsingClientElasticSearch()
        {
            var response = await _elasticSearchService.PostDataUsingClientElasticSearch();
            return MapToIActionResult(response);
        }

        [HttpGet]
        [Route("test-query-using-client-elastic-search")]
        public async Task<IActionResult> TestQueryUsingClientElasticSearch()
        {
            var response = await _elasticSearchService.TestQueryUsingClientElasticSearch();
            return MapToIActionResult(response);
        }

        [HttpPatch]
        [Route("update-data-using-client-elastic-search")]
        public async Task<IActionResult> UpdateDataUsingClientElasticSearch()
        {
            var response = await _elasticSearchService.UpdateDataUsingClientElasticSearch();
            return MapToIActionResult(response);
        }

        [HttpDelete]
        [Route("delete-data-using-client-elastic-search")]
        public async Task<IActionResult> DeleteDataUsingClientElasticSearch()
        {
            var response = await _elasticSearchService.DeleteDataUsingClientElasticSearch();
            return MapToIActionResult(response);
        }

        [HttpGet]
        [Route("test-query-using-nest")]
        public async Task<IActionResult> TestQueryUsingNEST()
        {

            return Ok();
        }
    }
}
