using LOLServices.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace LOLServices.Models
{
    public class GameDB01
    {
        public static StringCollection GetOnlineLog()
        {
            try
            {
                StringCollection allRows = new StringCollection();
                string rowText = "<th>LogTime</th><th>TotalCCU</th>";
                allRows.Add(rowText);
                using (GameDB_01DataContext dbContext = new GameDB_01DataContext())
                {
                    IEnumerable<OnlineLog> logs = dbContext.OnlineLogs.OrderByDescending(l => l.LogId).Take(36);
                    foreach (OnlineLog log in logs)
                    {
                        rowText = String.Format("<td>{0:dd-MMM-yyyy HH:mm:ss}</td><td>{1:#,##0}</td>", log.LogTime, log.TotalCCU);
                        allRows.Add(rowText);

                    }
                }
                return allRows;

            }
            catch (Exception ex)
            {
                LogService.WriteLine(ex);
                return null;
            }
        }
    }
}