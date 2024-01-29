using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BookShop.Core.Interfaces;
using BookShop.Core.Services;
using BookShop.Core.ViewModels;
using BookShop.DataAccessLayer.Entities;
using BookShop.Core.Classes;
using System.Globalization;


namespace BookMarketingVisual.Controllers
{
    public class AccountsController : Controller
    {
        private IAccount _account;

        private PersianCalendar pc = new PersianCalendar();
        public AccountsController(IAccount account)
        {
            _account = account;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel viewModel)
        {

            if (ModelState.IsValid)
            {
                if (_account.ExistsMobileNumbers(viewModel.Mobile))
                {
                    //Login again
                }
                else
                {
                    User user = new User
                    {

                        Mobile = viewModel.Mobile,
                        ActiveCode = CodeGenerators.ActiveCode(),
                        Code = null,
                        Date = pc.GetYear(DateTime.Now).ToString("0000") + "/" + pc.GetMonth(DateTime.Now).ToString("00") + "/" +
                        pc.GetDayOfMonth(DateTime.Now).ToString("00"),

                        FullName = null,
                        IsActive = false,
                        Password = HashGenerators.MD5Encoding(viewModel.Password),
                        RoleId = _account.GetMaxRole()
                    };

                    _account.AddUser(user);
                    // Send SMS

                    //Go to Activate
                }
            }
            return View(viewModel);
        }
    }
}
