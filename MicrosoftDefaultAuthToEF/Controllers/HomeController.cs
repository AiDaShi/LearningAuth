using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using MicrosoftDefaultAuthToEF.Models;
using Microsoft.AspNetCore.Authorization;
using NETCore.MailKit.Core;

namespace MicrosoftDefaultAuthToEF.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailService _emailService;

        public HomeController(
            ILogger<HomeController> logger,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IEmailService emailService
        )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            
            return View();
        }

        [HttpGet]
        public IActionResult Login(){
            return View();
        }

        [HttpGet]
        public IActionResult Register(){
            return View();
        }

        [Authorize]
        public IActionResult Secert()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username,string password){

            //login functionality

            var user = await _userManager.FindByNameAsync(username);
            if (user!=null)
            {
                var signResult = await _signInManager.PasswordSignInAsync(username,password,false,false);
                if (signResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username,string password){

            //Register functionality

            var user = new IdentityUser{
                UserName = username,
                Email = ""
            };
            var result = await _userManager.CreateAsync(user,password);

            if(result.Succeeded)
            {   
                //generation of the email token
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //join link
                var link = Url.Action(nameof(VerifyEmail),"Home",new { userid=user.Id,code },Request.Scheme,Request.Host.ToString());
                //send Email
                await _emailService.SendAsync("Testo@testto.com","email verify",$"<a href=\"{link}\">Verify Email<a/>",true);

                _logger.LogInformation($"The Link is :{link}");

                return RedirectToAction("EmailVerification");
            }

            return RedirectToAction("Index");
        }
        
        //Verify Email
        public async Task<IActionResult> VerifyEmail(string userid,string code)
        {
            var user = await _userManager.FindByIdAsync(userid);
            if(user==null) return BadRequest();
            
            //verify Email
            var result = await _userManager.ConfirmEmailAsync(user,code);
            if(result.Succeeded){
                return View();
            }

            return BadRequest();

        }

        public IActionResult EmailVerification()=>View();

        public async Task<IActionResult> LogOut(){
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }
    }
}
