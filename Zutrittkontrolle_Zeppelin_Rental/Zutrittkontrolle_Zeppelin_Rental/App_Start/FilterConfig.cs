using System.Web;
using System.Web.Mvc;

namespace Zutrittkontrolle_Zeppelin_Rental
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
