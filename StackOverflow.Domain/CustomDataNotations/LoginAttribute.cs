using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;

namespace StackOverflow.Domain.CustomDataNotations
{
    class LoginAttribute : FilterAttribute, IResultFilter, IActionFilter,IExceptionFilter,IAuthorizationFilter
    {
        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            
        }

        public void OnException(ExceptionContext filterContext)
        {
            throw new NotImplementedException();
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            throw new NotImplementedException();
        }
    }
}
