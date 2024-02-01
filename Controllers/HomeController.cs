using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookShop.Core.Classes;

namespace BookMarketingVisual.Controllers
{
    public class HomeController : Controller
    {
        [RoleAttribute(9)]
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
