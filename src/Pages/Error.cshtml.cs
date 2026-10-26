using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ContosoCrafts.WebSite.Pages
{
    /// <summary>
    /// Supplies diagnostic information for the application's error page.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorModel : PageModel
    {
        /// <summary>
        /// Gets or sets the identifier for the current request.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Gets whether the request identifier should be shown on the page.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        // Retained for the page's diagnostic logging dependency.
        private readonly ILogger<ErrorModel> _logger;

        /// <summary>
        /// Creates the error page model with its logger.
        /// </summary>
        /// <param name="logger">Logger used by this page model.</param>
        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Captures the current request identifier for display on the error page.
        /// </summary>
        public void OnGet()
        {
            // Prefer the current activity ID; fall back to ASP.NET Core's request ID.
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}
