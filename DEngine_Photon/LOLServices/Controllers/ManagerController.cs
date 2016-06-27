using DEngine.Common.Services;
using LOLServices.Helpers;
using LOLServices.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LOLServices.Controllers
{
    public class ManagerController : Controller
    {
        private static SortedList _cmdList = new SortedList();

        static ManagerController()
        {
            _cmdList.Add(0, "-- Manager Command --");
            for (int i = 1; i < (int)CommandCode.Count; i++)
                _cmdList.Add(i, ((CommandCode)i).ToString());
        }

        #region Private Methods

        private UserModel GetUser()
        {
            if (Session["UserData"] != null)
                return (UserModel)Session["UserData"];

            return null;
        }

        private bool UpdateWorlds(int sel)
        {
            return false;
        }

        private bool UpdateCmdCodes(int sel)
        {
            ViewData["cmdcode"] = new SelectList(_cmdList, "Key", "Value", sel);

            if (!_cmdList.ContainsKey(sel))
            {
                ModelState.AddModelError("cmdcode", "Invalid Command!");
                return false;
            }

            return true;
        }

        #endregion

        [HttpGet]
        [ServiceRight]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ContentResult GetCommands(int worldId)
        {
            CommandInfoList cmdList = new CommandInfoList();

            try
            {
                using (MainDBDataContext dbContext = new MainDBDataContext())
                {
                    IEnumerable<Command> allCmds = dbContext.Commands.Where(c => c.WorldId == worldId && c.GetTime == null);
                    foreach (Command cmd in allCmds)
                    {
                        CommandInfo cmdInfo = new CommandInfo()
                        {
                            UserId = cmd.UserId,
                            UserName = cmd.UserName,
                            Code = cmd.CmdCode,
                            Params = cmd.CmdParams,
                        };

                        cmdList.Commands.Add(cmdInfo);

                        // Update record status
                        cmd.GetTime = DateTime.Now;
                    }

                    dbContext.SubmitChanges();
                }
            }
            catch
            {
            }

            return Content(cmdList.ToXML(), "text/xml");
        }

        [Authorize]
        [ManagerRight]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [ManagerRight]
        public ActionResult SetCommand(int? worldId, int? userId, int? cmdCode, string cmdParams)
        {
            if (worldId == null || userId == null || cmdCode == null)
            {
                UpdateCmdCodes(0);
                return View();
            }

            UserModel curUser = GetUser();
            if (curUser == null)
                return RedirectToAction("InvalidRight", "Home");

            UpdateCmdCodes(cmdCode.Value);

            if (!ModelState.IsValid)
                return View();

            try
            {
                using (MainDBDataContext dbContext = new MainDBDataContext())
                {
                    Account acc = dbContext.Accounts.FirstOrDefault(a => a.UserId == userId);
                    if (acc != null)
                    {
                        Command cmd = new Command();
                        cmd.MasterId = curUser.UserId;
                        cmd.WorldId = worldId.Value;
                        cmd.UserId = acc.UserId;
                        cmd.UserName = acc.UserName;
                        cmd.CmdCode = cmdCode.Value;
                        cmd.CmdParams = cmdParams;
                        cmd.SetTime = DateTime.Now;

                        dbContext.Commands.InsertOnSubmit(cmd);
                        dbContext.SubmitChanges();
                    }
                }

                ViewData["Message"] = "SetCommand OK!";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("_FORM", ex.Message);
            }

            return View();
        }

        [Authorize]
        [ManagerRight]
        public ActionResult ViewReports(int? id)
        {
            if (id == null)
                id = 1;

            StringCollection allRows = null;

            switch (id.Value)
            {
                case 1:
                    allRows = GameDB01.GetOnlineLog();
                    break;
            }

            return View(allRows);
        }
    }
}
