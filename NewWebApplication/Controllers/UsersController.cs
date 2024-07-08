using NewWebApplication.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace NewWebApplication.Controllers
{
    public class UsersController : Controller
    {
        private string connectionString = "Data Source=np:\\\\.\\pipe\\LOCALDB#9E207986\\tsql\\query;Initial Catalog=master;Integrated Security=True";

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                // Save the uploaded image file
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(imageFile.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/Images"), fileName);
                    imageFile.SaveAs(filePath);
                    user.ImagePath = "/Images/" + fileName;
                }

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO Users (UserName, Email, Password, ImagePath) VALUES (@UserName, @Email, @Password, @ImagePath)", con);
                    cmd.Parameters.AddWithValue("@UserName", user.UserName);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@ImagePath", user.ImagePath);

                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: User
        public ActionResult Index()
        {
            List<User> users = new List<User>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT Id, UserName, Email, ImagePath FROM Users", con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    User user = new User
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        UserName = reader["UserName"].ToString(),
                        Email = reader["Email"].ToString(),
                        ImagePath = reader["ImagePath"].ToString(),
                    };
                    users.Add(user);
                }
            }
            return View(users);
        }
    }
}
