using LoginPageRegistrationPage.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace LoginPageRegistrationPage.Controllers
{
    public class RegistrationController : Controller
    {
        public readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment webHostEnvironment;
        public RegistrationController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            this.webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(Register register)
        {
            if (Request.Method == "POST")
            {
                Register er = new Register();
                string Connection = _configuration.GetConnectionString("DBConn");
                register.ProfilePicture = UploadedImage(register);
                register.Resumepath = UploadedFile(register);
                using (SqlConnection con = new SqlConnection(Connection))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_EnrollDetail", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FirstName", register.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", register.LastName);
                        cmd.Parameters.AddWithValue("@Password", register.Password);
                        cmd.Parameters.AddWithValue("@Gender", register.Gender);
                        cmd.Parameters.AddWithValue("@Email", register.Email);
                        cmd.Parameters.AddWithValue("@Phone", register.PhoneNumber);
                        cmd.Parameters.AddWithValue("@SecurityAnwser", register.SecurityAnwser);
                        cmd.Parameters.AddWithValue("@SecurityQuestion", register.SecurityQuestion);
                        cmd.Parameters.AddWithValue("@ProfileImage", register.ProfilePicture);
                        cmd.Parameters.AddWithValue("@Resume", register.Resumepath);
                        cmd.Parameters.AddWithValue("@status", "INSERT");
                        con.Open();
                        ViewData["result"] = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            return View();
        }
        private string UploadedImage(Register model)
        {
            string imagepath = null;

            if (model.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                imagepath = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, imagepath);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(fileStream);
                }
            }
            return imagepath;
        }
        private string UploadedFile(Register model)
        {
            string uniqueFileName = null;

            if (model.Resume != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Upload");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Resume.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Resume.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

    }
}
