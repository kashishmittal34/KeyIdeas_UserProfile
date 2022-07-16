using LoginPageRegistrationPage.Models;
using LoginPageRegistrationPage.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Data;
using System.Data.SqlClient;
using X.PagedList;
namespace LoginPageRegistrationPage.Controllers
{
    public class UserLoginController : Controller
    {
        public readonly IConfiguration _configuration;
        public readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserLoginController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
        }
        public string status = "";
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(Register register)
        {

            String SqlCon = _configuration.GetConnectionString("DBConn");
            SqlConnection con = new SqlConnection(SqlCon);
            con.Open();
            if (register.Email == "admin@gmail.com" && register.Password == "Admin@123")
            {
                HttpContext.Session.SetString("Email", register.Email);
                return RedirectToAction("Welcome");
            }
            else
            {
                string SqlQuery = "select Email,Password from Enrollment where Email=@Email and Password=@Password";
                SqlCommand cmd = new SqlCommand(SqlQuery, con);
                cmd.Parameters.AddWithValue("@Email", register.Email);
                cmd.Parameters.AddWithValue("@Password", register.Password);
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.Read())
                {
                    HttpContext.Session.SetString("Email", register.Email);
                    return RedirectToAction("WelcomePage");
                }
                else
                {
                    ViewData["Message"] = "User Login Details Failed!!";
                }
            }
            if (register.Email.ToString() != null)
            {
                HttpContext.Session.SetString("Email", register.Email.ToString());
                status = "1";
            }
            else
            {
                status = "3";
            }
            con.Close();
            return View();
            //return new JsonResult { Data = new { status = status } };  
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            List<Register> registerlist = new List<Register>();

            registerlist = HttpContext.Session.GetComplexData<List<Register>>("items");
            Register register = new Register();
            for (int i = 0; i <= registerlist[0].ID; i++)
            {
                if (id == registerlist[i].ID)
                {
                    register = registerlist[i];
                }
                
            }

            return View(register);
        }

        [HttpPost]

        public ActionResult Edit(int id, Register register)
        {
            IEmployeeRepository employeeRepository = new EmployeeRepository(_configuration, _webHostEnvironment);

            if (Request.Method == "POST")
            {

                string Connection = _configuration.GetConnectionString("DBConn");
                register.ID = id;
                register.ProfilePicture = employeeRepository.UploadedImage(register);
                register.Resumepath = employeeRepository.UploadedFile(register);
                using (SqlConnection con = new SqlConnection(Connection))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_UpdateEnrollDetail", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID", register.ID);
                        cmd.Parameters.AddWithValue("@FirstName", register.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", register.LastName);
                        //cmd.Parameters.AddWithValue("@Password", register.Password);
                        cmd.Parameters.AddWithValue("@Gender", register.Gender);
                        cmd.Parameters.AddWithValue("@Email", register.Email);
                        cmd.Parameters.AddWithValue("@Phone", register.PhoneNumber);
                        cmd.Parameters.AddWithValue("@SecurityAnwser", register.SecurityAnwser);
                        cmd.Parameters.AddWithValue("@SecurityQuestion", register.SecurityQuestion);
                        cmd.Parameters.AddWithValue("@ProfileImage", register.ProfilePicture);
                        cmd.Parameters.AddWithValue("@Resume", register.Resumepath);
                        cmd.Parameters.AddWithValue("@status", "Update");
                        con.Open();
                        int i = cmd.ExecuteNonQuery();
                        if (i > 0)
                        {
                            ViewBag.Edit = "Records Updated Succcessfully";
                        }
                        else
                        {
                            ViewBag.Edit = "No Records is Update";
                        }
                        con.Close();
                    }
                }
            }
            return View();
        }
        [HttpGet]
        public ActionResult Welcome(int? page, string search, string option, string sort)
        {
        //    ViewBag.SortByName = string.IsNullOrEmpty(sort) ? "descending name" : "";
        //    ViewBag.SortByGender = sort == "Gender" ? "descending gender" : "Gender";

            int pageIndex = 1;
            int pageSize = 4;
            Register user = new Register();
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            X.PagedList.IPagedList<Register> reg = null;
            DataSet ds = new DataSet();
            String SqlCon = _configuration.GetConnectionString("DBConn");
            using (SqlConnection con = new SqlConnection(SqlCon))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetEnrollmentDetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Email", SqlDbType.VarChar, 50).Value = HttpContext.Session.GetString("Email");
                    con.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    sda.Fill(ds);
                    List<Register> userlist = new List<Register>();

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Register uobj = new Register();
                        uobj.ID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString());
                        uobj.FirstName = ds.Tables[0].Rows[i]["FirstName"].ToString();
                        uobj.LastName = ds.Tables[0].Rows[i]["LastName"].ToString();
                        // uobj.Password = ds.Tables[0].Rows[i]["Password"].ToString();
                        uobj.Email = ds.Tables[0].Rows[i]["Email"].ToString();
                        uobj.PhoneNumber = ds.Tables[0].Rows[i]["Phone"].ToString();
                        uobj.SecurityAnwser = ds.Tables[0].Rows[i]["SecurityAnwser"].ToString();
                        uobj.Gender = ds.Tables[0].Rows[i]["Gender"].ToString();
                        uobj.SecurityQuestion = ds.Tables[0].Rows[i]["SecurityQuestion"].ToString();
                        uobj.ProfilePicture = ds.Tables[0].Rows[i]["ProfileImage"].ToString();
                        userlist.Add(uobj);
                    }
                   
                    HttpContext.Session.SetComplexData("items", userlist);
                    reg = userlist.ToPagedList(pageIndex, pageSize);
      
                }
                //switch (sort)
                //{

                //    case "descending name":
                //        reg = reg.OrderByDescending(x => x.FirstName).ToList().ToPagedList(1,3);
                //        break;

                //    case "descending gender":
                //        reg = reg.OrderByDescending(x => x.Gender).ToList().ToPagedList(1,3);
                //        break;

                //    case "Gender":
                //        reg = reg.OrderBy(x => x.Gender).ToList().ToPagedList(1,3);
                //        break;

                //    default:
                //        reg = reg.OrderBy(x => x.FirstName).ToList().ToPagedList(1,3);
                //        break;

                //}
                con.Close();
                if (option == "FirstName")
                {
                    return View(reg.Where(x => x.FirstName.StartsWith(search) || search == null).ToList().ToPagedList(1,3));
                }
                else if (option == "LastName")
                {
                    return View(reg.Where(x => x.LastName.StartsWith(search) || search == null).ToList().ToPagedList(1,3));
                }
                else if (option == "Email")
                {
                    return View(reg.Where(x => x.Email.StartsWith(search) || search == null).ToList().ToPagedList(1,3));
                }
                return View(reg);
            }

        }

        [HttpGet]
        //[ValidateAntiForgeryToken]
        //[ActionName("Welcome")]

        public ActionResult WelcomePage(Register e)
        {
            Register user = new Register();
            DataSet ds = new DataSet();
            String SqlCon = _configuration.GetConnectionString("DBConn");
            using (SqlConnection con = new SqlConnection(SqlCon))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetEnrollmentDetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Email", SqlDbType.VarChar, 50).Value = HttpContext.Session.GetString("Email");
                    con.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    sda.Fill(ds);
                    List<Register> userlist = new List<Register>();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Register uobj = new Register();
                        uobj.ID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString());
                        uobj.FirstName = ds.Tables[0].Rows[i]["FirstName"].ToString();
                        uobj.LastName = ds.Tables[0].Rows[i]["LastName"].ToString();
                        //uobj.Password = ds.Tables[0].Rows[i]["Password"].ToString();
                        uobj.Email = ds.Tables[0].Rows[i]["Email"].ToString();
                        uobj.PhoneNumber = ds.Tables[0].Rows[i]["Phone"].ToString();
                        uobj.SecurityAnwser = ds.Tables[0].Rows[i]["SecurityAnwser"].ToString();
                        uobj.Gender = ds.Tables[0].Rows[i]["Gender"].ToString();
                        uobj.SecurityQuestion = ds.Tables[0].Rows[i]["SecurityQuestion"].ToString();
                        uobj.ProfilePicture = ds.Tables[0].Rows[i]["ProfileImage"].ToString();
                        userlist.Add(uobj);
                    }
                    user.Registersinfo = userlist;
                }
                con.Close();

            }
            return View(user);
        }

        public ActionResult Delete(int id)
        {
            Register register = new Register();
            register.ID = id;
            String SqlCon = _configuration.GetConnectionString("DBConn");
            using (SqlConnection con = new SqlConnection(SqlCon))
            {
                using (SqlCommand cmd = new SqlCommand("sp_DeleteEnrollmentDetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    cmd.Parameters.AddWithValue("@Id", register.ID);
                    ViewData["delete"] = cmd.ExecuteNonQuery();
                    int i = Convert.ToInt32(ViewData["delete"]);
                    if (i >= 1)
                    {
                        ViewBag.AlertMsg = "Employee details deleted successfully";
                    }
                }
            }
            return RedirectToAction("Welcome");
        }
        public ActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "UserLogin");
        }
        public Register checkCookie()
        {
            Register user = new Register();
            string email = string.Empty, password = string.Empty;
            if (user.RememberMe == true)
            {
                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Secure = true;
                cookieOptions.Expires = DateTime.Now.AddMinutes(10);
                Response.Cookies.Append(email, user.Email, cookieOptions);
                Response.Cookies.Append(password, user.Password, cookieOptions);
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
                {
                    user = new Register { Email = email, Password = password };
                }

            }
            return user;
        }
        [HttpPost]
        public IActionResult DownloadReport(IFormCollection obj)
        {
            string reportname = $"User_Wise_{Guid.NewGuid():N}.xlsx";
            IEmployeeRepository employeeRepository = new EmployeeRepository(_configuration, _webHostEnvironment);

            var list = employeeRepository.GetUserList();
            if (list.Count > 0)
            {
                var exportbytes = employeeRepository.ExporttoExcel<Register>(list, reportname);
                return File(exportbytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", reportname);
            }
            else
            {
                TempData["Message"] = "No Data to Export";
                return RedirectToAction("Welcome");
            }
        }


    }

}
