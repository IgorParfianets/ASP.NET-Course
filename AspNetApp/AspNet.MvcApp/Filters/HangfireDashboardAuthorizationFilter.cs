using Hangfire.Dashboard;

namespace AspNetArticle.MvcApp.Filters
{
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return context.GetHttpContext().User.IsInRole("Admin");
        }
    }
}
