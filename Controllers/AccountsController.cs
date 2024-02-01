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
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

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
                    try
                    {
                        MessageSender sender = new MessageSender();
                        sender.SMS(viewModel.Mobile, "به کتابفروشی خوش آمدید" + Environment.NewLine+"کد فعال سازی : "+ user.ActiveCode);
                    }
                    catch 
                    {

                        throw;
                    }

                    return RedirectToAction(nameof(Activate));
                }
            }
            return View(viewModel);
        }

        public IActionResult Activate()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Activate(ActivateViewModel viewModel)
        {

            if (ModelState.IsValid)
            {
                if (_account.ActivateUser(viewModel.ActiveCode))
                {
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    ModelState.AddModelError("ActiveCode", "کد فعال سازی شما معتبر نیست");


                }

            }
            return View(viewModel);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string hashPassword = HashGenerators.MD5Encoding(viewModel.Password);

                User user = _account.LoginUser(viewModel.Mobile, hashPassword);

                if (user!=null)
                {
                    if (user.IsActive)
                    {
                        var claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.NameIdentifier , user.Id.ToString()),
                            new Claim(ClaimTypes.Name , user.Mobile)
                        };

                        var identity= new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        var properties = new AuthenticationProperties()
                        {
                            IsPersistent = true
                        };

                        HttpContext.SignInAsync(principal, properties);

                        if (user.Role.Name=="کاربر")
                        {
                            return RedirectToAction("Dashboard", "Home");

                        }
                    }
                    else
                    {
                        return RedirectToAction(nameof(Activate));
                    }
          
                }
                else
                {
                    ModelState.AddModelError("Password", "مشخصات کاربری اشتباه است");
                }
            }
                

            return View();
        }

            
        }
    }

