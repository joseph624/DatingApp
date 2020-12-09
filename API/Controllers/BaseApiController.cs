using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // All controllers can make use off this action filter
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        
    }
}