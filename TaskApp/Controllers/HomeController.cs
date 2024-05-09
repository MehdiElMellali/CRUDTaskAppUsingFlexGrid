using C1.Web.Mvc;
using C1.Web.Mvc.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TaskApp.Data;
using TaskApp.Models;
using Task = TaskApp.Data.Task;

namespace TaskApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TasksContext _db;

        public HomeController(ILogger<HomeController> logger, TasksContext context)
        {
            _logger = logger;
            _db = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// In order to get the real exception message.
        /// </summary>
        private static SqlException GetSqlException(Exception e)
        {
            while (e != null && !(e is SqlException))
            {
                e = e.InnerException;
            }
            return e as SqlException;
        }

        // Get the real exception message
        internal static string GetExceptionMessage(Exception e)
        {
            var msg = e.Message;
            var sqlException = GetSqlException(e);
            if (sqlException != null)
            {
                msg = sqlException.Message;
            }
            return msg;
        }

        public ActionResult ReadGridData([C1JsonRequest] CollectionViewRequest<Task> requestData)
        {
            return this.C1Json(CollectionViewHelper.Read(requestData, _db.Tasks.ToList()));
        }


        public ActionResult GridCreateTask([C1JsonRequest] CollectionViewEditRequest<Task> requestData)
        {
            return this.C1Json(CollectionViewHelper.Edit<Task>(requestData, item =>
            {
                string error = string.Empty;
                bool success = true;
                try
                {
                    _db.Entry(item as object).State = EntityState.Added;
                    _db.SaveChanges();
                }
                catch (Exception e)
                {
                    error = GetExceptionMessage(e);
                    success = false;
                }
                return new CollectionViewItemResult<Task>
                {
                    Error = error,
                    Success = success,
                    Data = item
                };
            }, () => (IEnumerable<Task>)_db.Tasks.ToList()));
        }


        public ActionResult GridUpdateTask([C1JsonRequest] CollectionViewEditRequest<Task> requestData)
        {
            return this.C1Json(CollectionViewHelper.Edit<Task>(requestData, item =>
            {
                string error = string.Empty;
                bool success = true;
                try
                {
                    _db.Entry(item as object).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                catch (Exception e)
                {
                    error = GetExceptionMessage(e);
                    success = false;
                }
                return new CollectionViewItemResult<Task>
                {
                    Error = error,
                    Success = success,
                    Data = item
                };
            }, () => (IEnumerable<Task>)_db.Tasks.ToList()));
        }

        public ActionResult GridDeleteTask([C1JsonRequest] CollectionViewEditRequest<Task> requestData)
        {
            return this.C1Json(CollectionViewHelper.Edit(requestData, item =>
            {
                string error = string.Empty;
                bool success = true;
                try
                {
                    {
                        _db.Entry(item as object).State = EntityState.Deleted;
                        _db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    error = GetExceptionMessage(e);
                    success = false;
                }
                return new CollectionViewItemResult<Task>
                {
                    Error = error,
                    Success = success,
                    Data = item
                };
            }, () => _db.Tasks.ToList()));
        }
    }
}
