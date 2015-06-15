using System.Web.Mvc;

//<httpErrors errorMode="Detailed" />
namespace ExceptionHandler
{
    public class HandleErrorAttribute : System.Web.Mvc.HandleErrorAttribute
    {
        private static bool IsAjax(ExceptionContext filterContext)
        {
            return filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
            {
                return;
            }

            if (IsAjax(filterContext))
            {
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        message = filterContext.Exception.Message
                    }, 
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.StatusCode = 500;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }
            else
            {
                base.OnException(filterContext);
            }
        }
    }
}