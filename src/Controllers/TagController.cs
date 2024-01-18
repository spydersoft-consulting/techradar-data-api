using Spydersoft.TechRadar.Api.Data;
using Spydersoft.TechRadar.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace Spydersoft.TechRadar.Api.Controllers
{
    /// <summary>
    /// Class TagController.
    /// Implements the <see cref="Spydersoft.TechRadar.Api.Controllers.DataControllerBase" />
    /// </summary>
    /// <seealso cref="Spydersoft.TechRadar.Api.Controllers.DataControllerBase" />
    [Authorize()]
    public class TagController : EditControllerBase<Tag>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TagController" /> class.
        /// </summary>
        /// <param name="radarDataItemService">The radar data item service.</param>
        public TagController(IRadarDataItemService radarDataItemService) : base(radarDataItemService)
        {
        }
    }
}