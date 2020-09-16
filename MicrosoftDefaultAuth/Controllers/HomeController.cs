using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MicrosoftDefaultAuth.Models;

namespace MicrosoftDefaultAuth.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secert()
        {
            return View();
        }

        public IActionResult Authenticate()
        {
            var grandmaClaims = new List<Claim>(){
                new Claim(ClaimTypes.Name,"Bob"),
                new Claim(ClaimTypes.Email,"86382516@qq.com"),
                new Claim("HiMinYang","Very Outstanding boi.")
            };
            
            var licenseClaims = new List<Claim>(){
                new Claim(ClaimTypes.Name,"Bob K Foo"),
                new Claim(ClaimTypes.Email,"86382516@qq.com"),
                new Claim("DivingLicense","A+")
            };

            var grandmaIdentity = new ClaimsIdentity(grandmaClaims,"Grandma Identity");
            var licenseIdentity = new ClaimsIdentity(licenseClaims,"Government");

            var userPrincipal =  new ClaimsPrincipal(new []{ grandmaIdentity,licenseIdentity });

            HttpContext.SignInAsync(userPrincipal);
            
            return RedirectToAction("Index");
        }

    }
}
