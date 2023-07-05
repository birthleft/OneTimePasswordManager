using Microsoft.AspNetCore.Mvc;
using OneTimePasswordManager.Data;
using OneTimePasswordManager.Models;

namespace OneTimePasswordManager.Controllers
{
    public class PasswordController : Controller
    {
        private readonly IAppDbContext _context;

        public PasswordController(IAppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Submit(IFormCollection form)
        {
            // Access the submitted form data
            string userId = form["userId"];
            string date = form["date"];
            string time = form["time"];
            bool currentDateTime = form["currentDateTime"] == "on";

            Console.WriteLine(userId + " " + date + " " + time + " " + currentDateTime);

            // A few things to note here:
            // 1. The password is generated using the SHA1 algorithm.
            // 2. The password is generated using the user ID, date, and time.
            // 3. The password "randomly" uses upper and lower case letters.
            string password = Utilities.PasswordUtility.Generate(userId + " " + date + " " + time);

            var validPassword = _context.Passwords.FirstOrDefault(vp => vp.UserId == userId);
            if (validPassword == null)
            {
                _context.Passwords.Add(new ValidPassword { UserId = userId, Password = password });
                if (_context.SaveChanges() > 0)
                {
                    return Json(new
                    {
                        success = true,
                        userId,
                        password
                    });
                }
            }

            return Json(new
            {
                success = false,
                error = "Unexpected Error!"
            });
        }

        [HttpPost]
        public IActionResult Reset(IFormCollection form)
        {
            if (form == null)
            {
                return Json(new
                {
                    success = true,
                });
            }

            string userId = form["userId"];

            var validPassword = _context.Passwords.FirstOrDefault(u => u.UserId == userId);
            if (validPassword != null)
            {
                _context.Passwords.Remove(validPassword);
                if (_context.SaveChanges() > 0)
                {
                    return Json(new
                    {
                        success = true
                    });
                }
            }

            return Json(new
            {
                success = false,
                error = "Unexpected Error!"
            });
        }

        [HttpPost]
        public IActionResult Check(IFormCollection form)
        {
            string password = form["passwordCheck"];

            var validPassword = _context.Passwords.FirstOrDefault(u => u.Password == password);
            if (validPassword != null)
            {
                return Json(new
                {
                    found = true
                });
            }

            return Json(new
            {
                found = false
            });
        }
    }
}
