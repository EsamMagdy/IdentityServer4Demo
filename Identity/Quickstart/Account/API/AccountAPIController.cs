// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using API.Controllers;
using Identity;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace IdentityServerHost.Quickstart.UI
{
    /// <summary>
    /// This sample controller implements a typical login/logout/provision workflow for local and external accounts.
    /// The login service encapsulates the interactions with the user data store. This data store is in-memory only and cannot be used for production!
    /// The interaction service provides a way for the UI to communicate with identityserver for validation and context retrieval
    /// </summary>
    [SecurityHeaders]
    [AllowAnonymous]
    public class AccountAPIController : BaseApiController
    {
        // private readonly TestUserStore _users;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AccountAPIController(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {

            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _userManager = userManager;
            _context = context;
        }


        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _signInManager.UserManager.Users.FirstOrDefault(t => t.PhoneNumber == model.Username);
                    
                    if (user == null)
                    {
                        //return new HttpResponseMessage(HttpStatusCode.NotFound);
                        return NotFound(new ResponseDto<string> { Message = "من فضلك تاكد من البريد الالكترونى" });
                    }
                    // validate username/password against in-memory store

                    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);
                    if (!result.Succeeded)
                    {

                        return NotFound(new ResponseDto<string> { Message = "من فضلك تاكد من الرقم السرى" });
                    }



                    // only set explicit expiration here if user chooses "remember me". 
                    // otherwise we rely upon expiration configured in cookie middleware.
                    AuthenticationProperties props = null;
                    if (AccountOptions.AllowRememberLogin && model.RememberLogin)
                    {
                        props = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.Add(AccountOptions.RememberMeLoginDuration)
                        };
                    };

                    // issue authentication cookie with subject ID and username
                    var isuser = new IdentityServerUser(user.Id)
                    {
                        DisplayName = user.UserName
                    };

                    await HttpContext.SignInAsync(isuser, props);

                    return Ok(new ResponseDto<string> { Data = _tokenService.GetToken("weatherapi.read").Result });


                }

                // something went wrong, show form with error
                return BadRequest();
            }
            catch (Exception ex)
            {

                throw;
            }
           

        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (await IsUserExistBefore(model.PhoneNumber))
                    {
                        return BadRequest(new ResponseDto<string> { Data = "username is already taken" });
                    }

                    var user = new ApplicationUser
                    {
                        UserName = model.PhoneNumber,
                        PhoneNumber = model.PhoneNumber,
                        Email = model.Email,
                        EmailConfirmed = true,
                        //Name = RegisterDto.FirstName.Trim() + " " + userModel.MiddleName.Trim() + " " + userModel.LastName.Trim(),
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        IsDeleted = false,
                        IsDeactivated = false
                        //ConcurrencyStamp="",
                        //LockoutEnabled=false,
                        //LockoutEnd=new DateTimeOffset()

                    };

                    var result = await _userManager.CreateAsync(user,model.Password);

                    if (!result.Succeeded) return null;

                    await _userManager.AddToRoleAsync(user, "User");
                    return Ok(new ResponseDto<string> { Data = _tokenService.GetToken("weatherapi.read").Result });
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                
                throw;
            }
            // return new HttpResponseMessage(HttpStatusCode.OK);
           



        }
        private async Task<bool> IsUserExistBefore(string Email)
        {
            var users = await _context.Users.ToListAsync();
            var user = _context.Users.FirstOrDefault(x => x.UserName == Email);
            return user != null ? true : false;
        }

        /// <summary>
        /// Show logout page
        /// </summary>
        //     [HttpGet]
        //     public async Task<IActionResult> Logout(string logoutId)
        //     {
        //         // build a model so the logout page knows what to display
        //         var vm = await BuildLogoutViewModelAsync(logoutId);

        //         if (vm.ShowLogoutPrompt == false)
        //         {
        //             // if the request for logout was properly authenticated from IdentityServer, then
        //             // we don't need to show the prompt and can just log the user out directly.
        //             return await Logout(vm);
        //         }

        //         return View(vm);
        //     }

        //     /// <summary>
        //     /// Handle logout page postback
        //     /// </summary>
        //     [HttpPost]
        //     [ValidateAntiForgeryToken]
        //     public async Task<IActionResult> Logout(LogoutInputModel model)
        //     {
        //         // build a model so the logged out page knows what to display
        //         var vm = await BuildLoggedOutViewModelAsync(model.LogoutId);

        //         if (User?.Identity.IsAuthenticated == true)
        //         {
        //             // delete local authentication cookie
        //             await HttpContext.SignOutAsync();

        //             // raise the logout event
        //             await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
        //         }

        //         // check if we need to trigger sign-out at an upstream identity provider
        //         if (vm.TriggerExternalSignout)
        //         {
        //             // build a return URL so the upstream provider will redirect back
        //             // to us after the user has logged out. this allows us to then
        //             // complete our single sign-out processing.
        //             string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

        //             // this triggers a redirect to the external provider for sign-out
        //             return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
        //         }

        //         return View("LoggedOut", vm);
        //     }

        //     [HttpGet]
        //     public IActionResult AccessDenied()
        //     {
        //         return View();
        //     }


        //     /*****************************************/
        //     /* helper APIs for the AccountController */
        //     /*****************************************/
        //     private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        //     {
        //         var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
        //         if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
        //         {
        //             var local = context.IdP == IdentityServer4.IdentityServerConstants.LocalIdentityProvider;

        //             // this is meant to short circuit the UI and only trigger the one external IdP
        //             var vm = new LoginViewModel
        //             {
        //                 EnableLocalLogin = local,
        //                 ReturnUrl = returnUrl,
        //                 Username = context?.LoginHint,
        //             };

        //             if (!local)
        //             {
        //                 vm.ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } };
        //             }

        //             return vm;
        //         }

        //         var schemes = await _schemeProvider.GetAllSchemesAsync();

        //         var providers = schemes
        //             .Where(x => x.DisplayName != null)
        //             .Select(x => new ExternalProvider
        //             {
        //                 DisplayName = x.DisplayName ?? x.Name,
        //                 AuthenticationScheme = x.Name
        //             }).ToList();

        //         var allowLocal = true;
        //         if (context?.Client.ClientId != null)
        //         {
        //             var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
        //             if (client != null)
        //             {
        //                 allowLocal = client.EnableLocalLogin;

        //                 if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
        //                 {
        //                     providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
        //                 }
        //             }
        //         }

        //         return new LoginViewModel
        //         {
        //             AllowRememberLogin = AccountOptions.AllowRememberLogin,
        //             EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
        //             ReturnUrl = returnUrl,
        //             Username = context?.LoginHint,
        //             ExternalProviders = providers.ToArray()
        //         };
        //     }

        //     private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        //     {
        //         var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
        //         vm.Username = model.Username;
        //         vm.RememberLogin = model.RememberLogin;
        //         return vm;
        //     }

        //     private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        //     {
        //         var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

        //         if (User?.Identity.IsAuthenticated != true)
        //         {
        //             // if the user is not authenticated, then just show logged out page
        //             vm.ShowLogoutPrompt = false;
        //             return vm;
        //         }

        //         var context = await _interaction.GetLogoutContextAsync(logoutId);
        //         if (context?.ShowSignoutPrompt == false)
        //         {
        //             // it's safe to automatically sign-out
        //             vm.ShowLogoutPrompt = false;
        //             return vm;
        //         }

        //         // show the logout prompt. this prevents attacks where the user
        //         // is automatically signed out by another malicious web page.
        //         return vm;
        //     }

        //     private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        //     {
        //         // get context information (client name, post logout redirect URI and iframe for federated signout)
        //         var logout = await _interaction.GetLogoutContextAsync(logoutId);

        //         var vm = new LoggedOutViewModel
        //         {
        //             AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
        //             PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
        //             ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
        //             SignOutIframeUrl = logout?.SignOutIFrameUrl,
        //             LogoutId = logoutId
        //         };

        //         if (User?.Identity.IsAuthenticated == true)
        //         {
        //             var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
        //             if (idp != null && idp != IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
        //             {
        //                 var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
        //                 if (providerSupportsSignout)
        //                 {
        //                     if (vm.LogoutId == null)
        //                     {
        //                         // if there's no current logout context, we need to create one
        //                         // this captures necessary info from the current logged in user
        //                         // before we signout and redirect away to the external IdP for signout
        //                         vm.LogoutId = await _interaction.CreateLogoutContextAsync();
        //                     }

        //                     vm.ExternalAuthenticationScheme = idp;
        //                 }
        //             }
        //         }

        //         return vm;
        //     }
    }
}
