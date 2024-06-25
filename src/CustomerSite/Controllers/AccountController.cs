using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace Marketplace.SaaS.Accelerator.CustomerSite.Controllers;

/// <summary>
/// Defines the <see cref="AccountController" />.
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
public class AccountController : Controller
{
    /// <summary>
    /// The SignIn.
    /// </summary>
    /// <param name="returnUrl">The returnUrl<see cref="string" />.</param>
    /// <returns>
    /// The <see cref="IActionResult" />.
    /// </returns>

    public async Task<IActionResult> SignIn(string returnUrl)
    {
        var redirectUrl = Url.Action(nameof(HandleToken), "Account", new { returnUrl });
        return this.Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, OpenIdConnectDefaults.AuthenticationScheme);
    }

    public async Task<IActionResult> HandleToken(string returnUrl)
    {
        var result = await HttpContext.AuthenticateAsync(OpenIdConnectDefaults.AuthenticationScheme);
        
        if (result?.Principal != null)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var idToken = await HttpContext.GetTokenAsync("id_token");

            // Send the token to the frontend. This example returns it as part of the query string.
            // For security reasons, prefer returning it in a secure cookie or via a secure API.
            return Redirect($"{returnUrl}?access_token={accessToken}&id_token={idToken}");
        }

        return RedirectToAction("SignIn");
    }

    // public IActionResult SignIn(string returnUrl)
    // {
    //     return this.Challenge(new AuthenticationProperties { RedirectUri = "/" }, OpenIdConnectDefaults.AuthenticationScheme);
    // }

    /// <summary>
    /// The SignOut.
    /// </summary>
    /// <returns>
    /// The <see cref="IActionResult" />.
    /// </returns>
    public new SignOutResult SignOut()
    {
        return this.SignOut(
            new AuthenticationProperties
            {
                RedirectUri = "Home/Index/",
            },
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// The SignedOut.
    /// </summary>
    /// <returns>
    /// The <see cref="IActionResult" />.
    /// </returns>
    public IActionResult SignedOut() => this.View();
}