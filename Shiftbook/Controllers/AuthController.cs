﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shiftbook.Models;
using Shiftbook.BL;
using Shiftbook.Helping_Classes;
using System.Security.Claims;
using System.Threading;

namespace Shiftbook.Controllers
{
    [ValidationFilter(CheckLogin = false)]
    public class AuthController : Controller
    {
        private readonly GeneralPurpose gp = new GeneralPurpose();
        private readonly DatabaseEntities de = new DatabaseEntities();

        public ActionResult Login(string msg = "", string color = "black")
        {

            if (gp.ValidateLoggedinUser() != null)
            {
                return RedirectToAction("Index", "Admin");
            }

            int userCount = new UserBL().GetActiveUsersList(de).Count();
            if (userCount == 0)
            {
                User obj = new User()
                {
                    FName = "Uzair",
                    LName = "Aslam",
                    UserName = "uzair",
                    Language = "English",
                    Contact = "0000-0000000",
                    Email = "uzair.aslam02@gmail.com",
                    Password = StringCipher.Encrypt("123"),
                    Author = 1,
                    IsPrimary = 0,
                    Role = 1,
                    IsActive = 1,
                    CreatedAt = GeneralPurpose.DateTimeNow()
                };

                bool chkUser = new UserBL().AddUser(obj, de);
            }

            ViewBag.Message = msg;
            ViewBag.Color = color;

            return View();
        }


        [HttpPost]
        public ActionResult PostLogin(string Email = "", string Password = "")
        {
            User user = new UserBL().GetActiveUsersList(de).Where(x => x.Email.Trim().ToLower() == Email.Trim().ToLower() && StringCipher.Decrypt(x.Password).Equals(Password)).FirstOrDefault();

            if (user == null)
            {
                return RedirectToAction("Login", new { msg = "Incorrect Email/Password!", color = "red" });
            }

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Sid,user.Id.ToString()),
                new Claim("UserName", user.FName + " " + user.LName),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim("Role", user.Role.ToString()),
            }, "ApplicationCookie");

            var claimsPrincipal = new ClaimsPrincipal(identity); // Set current principal
            Thread.CurrentPrincipal = claimsPrincipal;
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignIn(identity);

            return RedirectToAction("Index", "Admin");
        }



        #region Forgot Password

        public ActionResult ForgotPassword(string msg = "", string color = "black")
        {
            ViewBag.Color = color;
            ViewBag.Message = msg;

            return View();
        }

        public ActionResult PostForgotPassword(string Email = "")
        {
            bool checkEmail = gp.ValidateEmail(Email);

            if (checkEmail == false)
            {
                int id = new UserBL().GetActiveUsersList(de).Where(x => x.Email.ToLower() == Email.ToLower()).Select(x => x.Id).FirstOrDefault();

                string BaseUrl = string.Format("{0}://{1}{2}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority, "/");

                bool checkMail = MailSender.SendForgotPasswordEmail(Email, StringCipher.Base64Encode(id.ToString()), BaseUrl);

                if (checkMail == true)
                {
                    return RedirectToAction("ForgotPassword", "Auth", new { msg = "Please check your mails' inbox/spam", color = "green" });
                }
                else
                {
                    return RedirectToAction("ForgotPassword", "Auth", new { msg = "Mail sending fail!", color = "red" });
                }
            }
            else
            {
                return RedirectToAction("ForgotPassword", "Auth", new { msg = "Email does not belong to our record!!", color = "red" });
            }
        }


        public ActionResult ResetPassword(string encId = "", string t = "", string msg = "", string color = "black")
        {
            DateTime dt = Convert.ToDateTime(t);
            var passDate = dt.ToString("MM/dd/yyyy");
            var currDate = DateTime.Now.ToString("MM/dd/yyyy");
            if (passDate != currDate)
            {
                return RedirectToAction("Login", "Auth", new { msg = "Link expired, Please try again!", color = "red" });
            }

            ViewBag.Time = t;
            ViewBag.EncId = encId;
            ViewBag.Message = msg;
            ViewBag.Color = color;

            return View();
        }

        [HttpPost]
        public ActionResult PostResetPassword(string encId = "", string t = "", string NewPassword = "", string ConfirmPassword = "")
        {
            if (NewPassword != ConfirmPassword)
            {
                return RedirectToAction("ResetPassword", "Auth", new { encId = encId, t = t, msg = "Password and confirm password did not match", color = "red" });
            }

            User user = new UserBL().GetActiveUserById(Convert.ToInt32(StringCipher.Base64Decode(encId)), de);
            user.Password = StringCipher.Encrypt(NewPassword);

            bool check = new UserBL().UpdateUser(user, de);

            if (check == true)
            {
                return RedirectToAction("Login", "Auth", new { msg = "Your Password is Reset", color = "green" });
            }
            else
            {
                return RedirectToAction("ResetPassword", "Auth", new { encId = encId, t = t, msg = "Something is Wrong! Please Try Again", color = "red" });
            }
        }

        #endregion


        #region Manage Profile

        [ValidationFilter(CheckLogin = true)]
        public ActionResult UpdateProfile(string msg = "", string color = "black")
        {
            User u = new UserBL().GetActiveUserById(gp.ValidateLoggedinUser().Id, de);

            ViewBag.User = u;
            ViewBag.Message = msg;
            ViewBag.Color = color;

            return View();
        }


        [ValidationFilter(CheckLogin = true)]
        public ActionResult PostUpdateProfile(string EncId, User _user)
        {
            int UserId = Convert.ToInt32(StringCipher.Decrypt(EncId));

            bool checkEmail = gp.ValidateEmail(_user.Email, UserId);

            if (checkEmail == false)
            {
                return RedirectToAction("UpdateProfile", "Auth", new { msg = "Email used by someone else, Please try another", color = "red" });
            }

            DatabaseEntities de = new DatabaseEntities();
            User u = new UserBL().GetActiveUserById(UserId, de);

            u.FName = _user.FName.Trim();
            u.LName = _user.LName.Trim();
            u.UserName = _user.UserName;
            u.Language = _user.Language;
            u.Contact = _user.Contact; 
            u.Email = _user.Email.Trim();

            bool chk = new UserBL().UpdateUser(u, de);

            if (chk)
            {
                return RedirectToAction("UpdateProfile", "Auth", new { msg = "Profile updated successfully!", color = "green" });
            }
            else
            {
                return RedirectToAction("UpdateProfile", "Auth", new { msg = "Somthings' Wrong!", color = "red" });
            }
        }


        [ValidationFilter(CheckLogin = true)]
        public ActionResult UpdatePassword(string msg = "", string color = "black")
        {
            ViewBag.Message = msg;
            ViewBag.Color = color;

            return View();
        }


        [ValidationFilter(CheckLogin = true)]
        public ActionResult PostUpdatePassword(string oldPassword = "", string newPassword = "", string confirmPassword = "")
        {
            if (newPassword != confirmPassword)
            {
                return RedirectToAction("UpdatePassword", "Auth", new { msg = "New password and Confirm password did not match!", color = "red" });
            }

            User u = new UserBL().GetActiveUserById(gp.ValidateLoggedinUser().Id, de);

            if (StringCipher.Decrypt(u.Password) != oldPassword)
            {
                return RedirectToAction("UpdatePassword", "Auth", new { msg = "Old password did not match!", color = "red" });
            }

            u.Password = StringCipher.Encrypt(newPassword);

            bool chk = new UserBL().UpdateUser(u, de);

            if (chk)
            {
                return RedirectToAction("UpdatePassword", "Auth", new { msg = "Password updated successfully!", color = "green" });
            }
            else
            {
                return RedirectToAction("UpdatePassword", "Auth", new { msg = "Somthings' wrong!", color = "red" });
            }
        }

        #endregion


        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;
            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("Login", "Auth");
        }
    }
}