using Freezer.Core;
using Freezer.Mvc;
using Freezer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProjectAlpha.Controllers
{

    [Authorize]
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class APIController : Controller
    {
        // GET: API
        public ActionResult Display()
        {
            return View();
        }

        // GET: API/Details/5
        public ActionResult ReturnSS()
        {
            var screenShotJob = ScreenshotJobBuilder.Create("DisplayChart", "Home")
             .SetTransfertRequestCookies(true); // forward session cookies to the capturing browser

            return screenShotJob.Freeze();
        }

        public byte[] FreezeScreen(string inputUrl)
        {
            var screenshotJob = ScreenshotJobBuilder.Create("inputUrl")
              .SetBrowserSize(1366, 768)
              .SetCaptureZone(CaptureZone.FullPage) // Set what should be captured
             // .SetTrigger(new WindowLoadTrigger(60)) // Set when the picture is taken
                .SetTrigger(new DomReadyTrigger(50));
            // document.dispatchEvent(new MessageEvent('FreezerFire'));
            System.IO.File.WriteAllBytes("Screenshot.png", screenshotJob.Freeze());
            byte[] imgSrc = screenshotJob.Freeze();
            return imgSrc;
        }

    }
}
