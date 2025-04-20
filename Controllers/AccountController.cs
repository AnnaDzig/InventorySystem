using InventorySystem.Data;
using InventorySystem.Models;
using InventorySystem.Services;
using InventorySystem.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InventorySystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;
         private readonly IPasswordHasher<User> _passwordHasher;



        public AccountController(ApplicationDbContext context, EmailService emailService, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _emailService = emailService;
            _passwordHasher = passwordHasher;
        }


        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use.");
                    return View(user);
                }

                user.Password = _passwordHasher.HashPassword(user, user.Password);


                if (user.Email == "anna.soft.dev@gmail.com") 
                {
                    user.Role = "Admin";
                }
                else
                {
                    user.Role = "User";
                }

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                await SignInUser(user);
                return RedirectToAction("Index", "Home");
            }

            return View(user);
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
            {
                ViewBag.LoginError = "Invalid email or password.";
                return View(model);
            }

            try
            {
                var passwordCheck = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);
                if (passwordCheck == PasswordVerificationResult.Failed)
                {
                    ViewBag.LoginError = "Invalid email or password.";
                    return View(model);
                }
            }
            catch (FormatException)
            {
                ViewBag.LoginError = "Your account uses an outdated password format. Please reset your password.";
                return View(model);
            }

            await SignInUser(user);
            return RedirectToAction("Index", "Home");
        }


        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // Приватний метод авторизації через Claims
        private async Task SignInUser(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
               new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email),
                 new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var identity = new ClaimsIdentity(
         claims,
         CookieAuthenticationDefaults.AuthenticationScheme,
         ClaimTypes.Name,
         ClaimTypes.Role);

            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(3)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Email is required.");
                return View();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                ModelState.AddModelError("", "No account associated with this email.");
                return View();
            }

            // Генеруємо токен
            var token = Guid.NewGuid().ToString();
            var resetToken = new PasswordResetToken
            {
                Email = email,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            _context.PasswordResetTokens.Add(resetToken);
            await _context.SaveChangesAsync();

            // Формуємо посилання
            var resetLink = Url.Action("ResetPassword", "Account", new { token = token }, Request.Scheme);

            // Надсилаємо лист
            await _emailService.SendEmailAsync(
                toEmail: email,
                subject: "Password Reset Request",
                htmlContent: $"<p>Click the link below to reset your password:</p><p><a href='{resetLink}'>{resetLink}</a></p>"
            );

            TempData["Message"] = "Password reset instructions sent to your email.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token)) return BadRequest();

            var resetEntry = await _context.PasswordResetTokens.FirstOrDefaultAsync(t => t.Token == token);
            if (resetEntry == null || resetEntry.ExpiresAt < DateTime.UtcNow)
                return BadRequest("Invalid or expired token.");

            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string token, string newPassword)
        {
            var resetEntry = await _context.PasswordResetTokens.FirstOrDefaultAsync(t => t.Token == token);
            if (resetEntry == null || resetEntry.ExpiresAt < DateTime.UtcNow)
                return BadRequest("Invalid or expired token.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == resetEntry.Email);
            if (user == null) return NotFound();

            user.Password = _passwordHasher.HashPassword(user, newPassword);
            _context.PasswordResetTokens.Remove(resetEntry);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Password successfully reset!";
            return RedirectToAction("Login");
        }



    }
}
