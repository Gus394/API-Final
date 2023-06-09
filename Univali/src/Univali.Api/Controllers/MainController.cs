using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace Univali.Api.Controllers;

[ApiController]
public abstract class MainController : ControllerBase
{
    public override ActionResult ValidationProblem(
    [ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
    {
        // base.ValidationProblem();
        var options = HttpContext.RequestServices
            .GetRequiredService<IOptions<ApiBehaviorOptions>>();

        return (ActionResult)options.Value
            .InvalidModelStateResponseFactory(ControllerContext);
    }
}