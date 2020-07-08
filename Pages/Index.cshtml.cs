using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace az204webapp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            _logger.LogCritical("level 5: Here is a Critical Message"); // Writes an error message at log level 5
            _logger.LogError("level 4: Here is a Error Message"); // Writes an error message at log level 4
            _logger.LogWarning("level 3: Here is a Warning Message"); // Writes a warning message at log level 3
            _logger.LogInformation("level 2: Here is a Information Message"); // Writes an information message at log level 2
            _logger.LogDebug("level 1: Here is a Debug Message"); // Writes a debug message at log level 1
            _logger.LogTrace("level 0: Here is a Trace message"); // Writes a detailed trace message at log level 0
        }
    }
}
