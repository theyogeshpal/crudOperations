using Microsoft.AspNetCore.Mvc;
using tasktwodotnet.Models;
using tasktwodotnet.Services;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace tasktwodotnet.Controllers
{
    public class UserController : Controller
    {
        DatabaseConnection connection;
        EmailSender emailSender;
        public UserController(DatabaseConnection _db,EmailSender emailSender)
        {
            connection = _db;
            this.emailSender = emailSender;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SaveData(User data)
        {
            var exists = connection.User.FirstOrDefault(x => x.email == data.email);
            if(exists != null)
            {
                TempData["emailavl"] = "Email address already registered in our database";
                return RedirectToAction("Register");
            }
            data.verification = false;
            connection.User.Add(data);
            connection.SaveChanges();


            HttpContext.Session.SetString("verification", data.email);

            int otp = new Random().Next(1000, 9999);
            HttpContext.Session.SetString("otp", otp.ToString());

            emailSender.SendEmail(data.email, "Verify your email", "Please verify your email by entering the OTP sent to your email address.+ '"+otp+"'");

            return RedirectToAction("VerifyOTP");
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoginData(User data)
        {
            var Userdata = connection.User.FirstOrDefault(x => x.email == data.email && x.password == data.password);

            if(Userdata != null)
            {
                HttpContext.Session.SetString("loginemail", data.email);
                return RedirectToAction("Profile");
            }
            else
            {
                TempData["loginerror"] = "Email ID or password is incorrect";
                return RedirectToAction("Login");
            }
        }

        public IActionResult VerifyOTP()
        {
            string verifyemail = HttpContext.Session.GetString("verification");
            var data = connection.User.FirstOrDefault(x => x.email == verifyemail);
            return View(data);
        }
        [HttpPost]
        public IActionResult VerifyOTP(IFormCollection data)
        {
            string id = data["id"];
            string newotp = data["otp"];

            var olddata = connection.User.Find(int.Parse(id));
            int otp = int.Parse(HttpContext.Session.GetString("otp"));

            if(otp == int.Parse(newotp))
            {
                olddata.verification = true;
                connection.User.Update(olddata);
                connection.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                TempData["otperror"] = "OTP did not match, please enter correct OTP";
                return RedirectToAction("VerifyOTP");
            }


        }

        public IActionResult Profile()
        {
            if(HttpContext.Session.GetString("loginemail") == null)
            {
                TempData["loginerror"] = "You need to enter correct email and password to open profile";
                return RedirectToAction("Login");
            }

            var data = connection.User.FirstOrDefault(x => x.email == HttpContext.Session.GetString("loginemail"));

            return View(data);
        }
        public IActionResult Showdata()
        {
            var data = connection.User.ToList();
            return View(data);
        }

        public IActionResult Edit(int id)
        {
            var data = connection.User.Find(id);

            return View(data);
        }
        public IActionResult Delete(int id)
        {
            var data = connection.User.Find(id);
            connection.User.Remove(data);
            connection.SaveChanges();

            return RedirectToAction("Showdata");
        }
        [HttpPost]
        public IActionResult UpdateDetails(User data)
        {
            connection.User.Update(data);

            return RedirectToAction("Showdata");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult changepassword(int id)
        {
            var data = connection.User.Find(id);

            return View(data);
        }
        [HttpPost]
        public IActionResult updatepassword(IFormCollection data)
        {
            string id = data["id"];
            string oldpass = data["oldpass"];
            string newpass = data["newpass"];
            string confirmpass = data["confirmpass"];

            var olddata = connection.User.Find(int.Parse(id));
            

            if(oldpass == olddata.password)
            {
                if(newpass == confirmpass)
                {
                    olddata.password = newpass;
                    connection.User.Update(olddata);
                    connection.SaveChanges();

                    TempData["passchangeerror"] = "Password Changed Successfully";
                    return RedirectToAction("Profile");
                }
                else
                {
                    TempData["passchangeerror"] = "New password and confirm new password did not match";
                    return RedirectToAction("Profile");
                }
            }
            else
            {
                TempData["passchangeerror"] = "Old password is not correct";
                return RedirectToAction("Profile");
            }

        }
    }
}
