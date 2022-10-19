// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable
using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Enums;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Encodings.Web;
namespace BugTracker.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<BTUser> _signInManager;
        private readonly UserManager<BTUser> _userManager;
        private readonly IUserStore<BTUser> _userStore;
        private readonly IUserEmailStore<BTUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        // Inject image service so we can create a company with an image
        private readonly IImageService _imageService;
        // Inject DbContext, since we'll need to save the new company
        private readonly ApplicationDbContext _context;
        public RegisterModel(
            UserManager<BTUser> userManager,
            IUserStore<BTUser> userStore,
            SignInManager<BTUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IImageService imageService,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _imageService = imageService;
            _context = context;
        }
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            // These are copied/pasted from your BTUser model, since we need a first/last name to save a new user
            [Required]
            [DisplayName("First Name")]
            public string FirstName { get; set; }
            [Required]
            [DisplayName("Last Name")]
            public string LastName { get; set; }
            // These are our properties to create a company, since every user must belong to a company
            // You may consider setting a maximum/minimum length for company name/description and first/last name
            [Required]
            [DisplayName("Company Name")]
            public string CompanyName { get; set; }
            [DisplayName("Company Description")]
            public string CompanyDescription { get; set; }
            [DisplayName("Company Image")]
            [DataType(DataType.Upload)]
            public virtual IFormFile CompanyFormFile { get; set; }
        }
        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // get out new user to work with
                var user = CreateUser();
                // create a company for them to belong to
                Company newCompany = new Company();
                newCompany.Name = Input.CompanyName;
                newCompany.Description = Input.CompanyDescription;

                if (Input.CompanyFormFile != null)
                {
                    newCompany.ImageData = await _imageService.ConvertFileToByteArrayAsync(Input.CompanyFormFile);
                    newCompany.ImageType = Input.CompanyFormFile.ContentType;
                }
                // save our company to give it an ID
                _context.Add(newCompany);
                await _context.SaveChangesAsync();
                // assign our new user to that company
                user.CompanyId = newCompany.Id;
                // let the scaffolded code take care of saving our new user
                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);
                await _userManager.AddToRoleAsync(user, nameof(BTRoles.Admin));

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);
                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }
        private BTUser CreateUser()
        {
            try
            {
                // instantiate our new user with the first/last name from the form
                BTUser newUser = Activator.CreateInstance<BTUser>();
                newUser.FirstName = Input.FirstName;
                newUser.LastName = Input.LastName;
                return newUser;
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(BTUser)}'. " +
                    $"Ensure that '{nameof(BTUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }
        private IUserEmailStore<BTUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<BTUser>)_userStore;
        }
    }
}