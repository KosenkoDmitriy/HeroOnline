using DEngine.Common.Services;
using LOLServices.Models;
using System.Linq;
using System.Web.Mvc;

namespace LOLServices.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class GameController : Controller
    {
        [HttpGet]
        public ContentResult GetWorlds()
        {
            WorldInfoList worldList = new WorldInfoList();

            try
            {
                using (MainDBDataContext dbContext = new MainDBDataContext())
                {
                    worldList.Worlds = dbContext.Worlds.Select(w => new WorldInfo() { Id = w.WorldId, Name = w.WorldName, ServiceAddress = w.ServiceAddress, Version = w.Version }).ToList();
                }
            }
            catch
            {
            }

            return Content(worldList.ToXML(), "text/xml");
        }
    }
}
