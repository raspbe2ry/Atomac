using Atomac.Models;
using DataTables.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;

namespace Atomac.Controllers
{
    public class DataTablesController : Controller
    {
        [HttpPost]
        public JsonResult GetActiveUsers([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            IQueryable<ApplicationUser> query = null;
            int totalCount = 0;
            int filteredCount = 0;

            using (ApplicationDbContext hfdb = new ApplicationDbContext())
            {
                query = from user in hfdb.Users
                        where user.Status == PStatus.Active
                        select user;

                totalCount = query.Count();

                if (requestModel.Search.Value != string.Empty)
                {
                    string value = requestModel.Search.Value.Trim();
                    query = query.Where(user => user.FirstName.Contains(value) ||
                                                user.LastName.Contains(value) ||
                                                user.NickName.Contains(value)
                                       );
                }

                filteredCount = query.Count();

                IOrderedEnumerable<Column> sortedColumns = requestModel.Columns.GetSortedColumns();
                string orderByString = string.Empty;

                foreach (Column column in sortedColumns)
                {
                    orderByString += orderByString != string.Empty ? "," : "";
                    orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
                }

                query = query.OrderBy(orderByString == string.Empty ? "LastName asc" : orderByString);
                query = query.Skip(requestModel.Start).Take(requestModel.Length);

                var data = query.Select(user =>
                    new {
                        id = user.Id,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        nickName = user.NickName,
                        points = user.Points
                    }).ToList();

                return Json(new DataTablesResponse(requestModel.Draw, data, filteredCount, totalCount));
            }
        }
    }
}