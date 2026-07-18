using System;
using BobsBookstoreClassic.Data;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Web.Controllers
{
    public class AuthenticationController : Controller
    {
        public IActionResult Login(string redirectUri = null)
        {
            if (string.IsNullOrWhiteSpace(redirectUri))
                return RedirectToAction("Index", "Home");
            return Redirect(redirectUri);
        }

        public IActionResult LogOut()
        {
            return BookstoreConfiguration.Get("Services/Authentication") == "aws" ? CognitoSignOut() : LocalSignOut();
        }

        private IActionResult LocalSignOut()
        {
            if (HttpContext.Request.Cookies.ContainsKey("LocalAuthentication"))
            {
                HttpContext.Response.Cookies.Delete("LocalAuthentication");
            }

            return RedirectToAction("Index", "Home");
        }

        private IActionResult CognitoSignOut()
        {
            if (Request.Cookies.ContainsKey(".AspNet.Cookies"))
            {
                Response.Cookies.Delete(".AspNet.Cookies");
            }

            var domain = BookstoreConfiguration.Get("Authentication/Cognito/CognitoDomain");
            var clientId = BookstoreConfiguration.Get("Authentication/Cognito/LocalClientId");
            var logoutUri = $"{Request.Scheme}://{Request.Host.Host}:{Request.Host.Port}/";
            return Redirect($"{domain}/logout?client_id={clientId}&logout_uri={logoutUri}");
        }
    }
}
