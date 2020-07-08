﻿using System;
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
            _logger.LogCritical("level 5: Critical Message"); // Writes an error message at log level 4 <br>
            _logger.LogWarning("level 4: Error Message"); // Writes a warning message at log level 3<br>
            _logger.LogInformation("level 3: Information Message"); // Writes an information message at log level 2<br>
            _logger.LogDebug("level 2: Debug Message"); // Writes a debug message at log level 1<br>
            _logger.LogTrace("level 1: Trace message"); // Writes a detailed trace message at log level 0<br>
        }
    }
}
