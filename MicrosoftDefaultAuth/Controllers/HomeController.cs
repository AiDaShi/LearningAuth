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
using MicrosoftDefaultAuth.CustomAuthorizationPolicyProvider;
using MicrosoftDefaultAuth.Models;

namespace MicrosoftDefaultAuth.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAuthorizationService _authorizationService;


        public HomeController(ILogger<HomeController> logger, IAuthorizationService authorizationService)
        {
            _logger = logger;
            _authorizationService = authorizationService;
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

        /// <summary>
        /// 针对是否有生日日期进行验证
        /// SecurityLevel
        /// </summary>
        /// <returns></returns>
        [Authorize(Policy = "Claim.DoB")]
        [Authorize(Policy = "SecurityLevel")]
        public IActionResult SecertPolicy()
        {
            return View("Secert");
        }

        [Authorize(Policy = "Admin")]
        public IActionResult SecertRole()
        {
            return View("Secert");
        }

        //使用自定义的SecurityLevel
        [SecurityLevel(5)]
        public IActionResult SecertLevel5()
        {
            return View("Secert");
        }

        [SecurityLevel(10)]
        public IActionResult SecertLevel10()
        {
            return View("Secert");
        }


        [AllowAnonymous]
        public IActionResult Authenticate()
        {
            var grandmaClaims = new List<Claim>(){
                new Claim(ClaimTypes.Name,"Bob"),
                new Claim(ClaimTypes.Email,"86382516@qq.com"),
                new Claim(ClaimTypes.DateOfBirth,DateTime.Now.ToString()),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.Role,"AdminTwo"),
                new Claim(DynamicPilicies.SecurityLevel,"7"),
                new Claim("HeMinYang","Very Outstanding boi.")
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

        public async Task<IActionResult> DoStuff() 
        {
            var builder = new AuthorizationPolicyBuilder("Schema");
            var customPolicy = builder.RequireClaim("Hello").Build();

            var authresult = await _authorizationService.AuthorizeAsync(HttpContext.User, customPolicy);

            if (authresult.Succeeded)
            {
                return View("Index");
            }

            return View("Index");
        }
    }
}
