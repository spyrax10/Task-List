using LoginReg.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LoginReg.Controllers
{
    public class HomeController : Controller
    {
        private defCon con = new defCon();
        private taskCS ts = new taskCS();

        // GET: Home
        
        public ActionResult Index()
        {
            if (Session["Id"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        // GET: Home/Task
        public ActionResult Task()
        {
            if (Session["Id"] != null)
            {
                // Cheking email to make sure task only view by appropriate user
                string email = Session["Email"].ToString();
                return View(ts.taskTB.Where(a => a.Email.Equals(email)));
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        // GET: Home/Create Task
        public ActionResult CreateTask()
        {
            return View();
        }

        // POST: Home/CreateTask

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTask([Bind(Exclude = "Id")]Task task)
        {
            if (ModelState.IsValid)
            {
                string email = Session["Email"].ToString();
                // Cheking whether task already created base on its user email and Due date
                var check = ts.taskTB.FirstOrDefault(s => s.Email == email 
                && s.DueDate == task.DueDate);

                if (check == null)
                {
                    // Adding task to database
                    task.Email = Session["Email"].ToString();
                    task.Status = "False";
                    ts.Configuration.ValidateOnSaveEnabled = false;
                    ts.taskTB.Add(task);
                    ts.SaveChanges();
                    
                    var data = con.userTB.Where(s => s.Email.Equals(email)).ToList();

                    // Adding Session 
                    Session["Id"] = data.FirstOrDefault().Id;
                    Session["Email"] = data.FirstOrDefault().Email;
                    Session["Firstname"] = data.FirstOrDefault().Firstname;
                    
                    ViewBag.Message = "Task Created!";
                    return RedirectToAction("Task");
                }
                else
                {
                    // Duplicate Task Error
                    ViewBag.error = "Task already exists!";
                    return View();
                }
            }
            return View();
        }

        // GET: Home/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Home/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] User userReg)
        {
            if (ModelState.IsValid)
            {
                // Checking if email is already registered
                var check = con.userTB.FirstOrDefault(s => s.Email == userReg.Email);
                if (check == null)
                {
                    // Adding user to database
                    userReg.Password = GetMD5(userReg.Password);
                    con.Configuration.ValidateOnSaveEnabled = false;
                    con.userTB.Add(userReg);
                    con.SaveChanges();
                    
                    // Adding Session
                    var data = con.userTB.Where(s => s.Email.Equals(userReg.Email)
                         && s.Password.Equals(userReg.Password)).ToList();

                    Session["Id"] = data.FirstOrDefault().Id;
                    Session["Email"] = data.FirstOrDefault().Email;
                    Session["Firstname"] = data.FirstOrDefault().Firstname;
                    return RedirectToAction("Index");
                }
                else
                {
                    // Duplicate email error
                    ViewBag.error = "Email already exists!";
                    return View();
                }
            }
            return View();
        }

        //GET: Home/Login
        public ActionResult Login(User user)
        {
            ModelState.Clear();
            return View();
        }

        //POST: Home/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {

            if (ModelState.IsValid)
            {
                // Converting Login password to hash and checking whether user exist
                var f_password = GetMD5(password);
                var data = con.userTB.Where(a => a.Email.Equals(email) && 
                    a.Password.Equals(f_password)).ToList();
               
                if (data.Count() > 0)
                {
                    //Adding session
                    Session["Firstname"] = data.FirstOrDefault().Firstname;
                    Session["Id"] = data.FirstOrDefault().Id;
                    Session["Email"] = data.FirstOrDefault().Email;
                    return RedirectToAction("Index");
                }
                else
                {
                    // Login Error
                    ViewBag.error = "Login Failed!";
                    return View();
                }
            }
            return View();
        }

        //GET: Home/EditTask
        public ActionResult EditTask(int Id)
        {
            //Getting the current task details 
            var task2Edit = ts.taskTB.Where(a => a.Id.Equals(Id)).FirstOrDefault();
            return View(task2Edit);
        }

        //POST: Home/EditTask
        [HttpPost]
        public ActionResult EditTask(Task task)
        {
            //Getting the current task details
            var curTask = ts.taskTB.Where(a => a.Id.Equals(task.Id)).FirstOrDefault();

            if (!ModelState.IsValid)
            {
                return View();
            }

            //Updating database
            task.Email = Session["Email"].ToString();
            task.Status = "False";
            ts.Entry(curTask).CurrentValues.SetValues(task);
            ts.SaveChanges();

            return RedirectToAction("Task");
        }

        //Method for getting and converting connection string of taskTB
        public SqlConnection getCon()
        {
            string cs = new taskCS().Database.Connection.ConnectionString;

            var con = new SqlConnection();
            con.ConnectionString = cs;
            return con;
        }

        //POST: Home/UpdStat method for updating of Status Column
        [HttpPost]
        public bool UpdStat(int taskId, string taskStat)
        {

            bool stat = false;
            using (var con = getCon())
            {
                using (var cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.CommandText = "Update taskTB SET Status = @stat where Id = @id";
                    cmd.Parameters.AddWithValue("@stat", taskStat);
                    cmd.Parameters.AddWithValue("@id", taskId);
                    cmd.ExecuteNonQuery();
                    stat = true;
                }
            }
            return stat;
        }

        //GET: Home/DelTask
        public ActionResult DelTask(int? Id)
        {
            if (Id != null)
            {
                //Finding details of selected task
                var curTask = ts.taskTB.Find(Id);
                return View(curTask);
            }
            return View();
        }

        //POST: Home/DelTask
        [HttpPost, ActionName("DelTask")]
        [ValidateAntiForgeryToken]
        public ActionResult DelTaskCon(int Id)
        {
            //Getting task details
            var curTask = ts.taskTB.Where(a => a.Id.Equals(Id)).FirstOrDefault();

            if (!ModelState.IsValid)
                return View();

            //Removing task from database
            ts.taskTB.Remove(curTask);
            ts.SaveChanges();

            return RedirectToAction("Task");
        }

        //User Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        //Method for converting password into hash
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }

    }
}
