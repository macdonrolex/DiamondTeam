using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ContosoCrafts.WebSite.Pages
{
    /// <summary>
    /// Supplies the page handler for the Privacy page.
    /// </summary>
    public class PrivacyModel : PageModel
    {
        // Logger injected for page diagnostics.
        public readonly ILogger<PrivacyModel> Logger;

        /// <summary>
        /// Creates the privacy page model with its logger.
        /// </summary>
        /// <param name="logger">Logger associated with this page model.</param>
        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Handles a request to display the Privacy page.
        /// </summary>
        public void OnGet()
        {
        }
    }
}
