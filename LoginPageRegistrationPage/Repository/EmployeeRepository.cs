using LoginPageRegistrationPage.Models;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.Data;
using System.Data.SqlClient;

namespace LoginPageRegistrationPage.Repository
{
    public interface IEmployeeRepository
    {
        string UploadedFile(Register model);
        string  UploadedImage(Register model);
        List<Register> GetUserList();
        byte[] ExporttoExcel<T>(List<T> table, string filename);
    }
    public class EmployeeRepository:IEmployeeRepository
    {
      
        private IConfiguration _configuration;
        private readonly IWebHostEnvironment webHostEnvironment;
        //To Handle connection related activities
        public EmployeeRepository(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
           
            _configuration = configuration;
            this.webHostEnvironment = webHostEnvironment;
        }

        public List<Register> GetUserList()
        {
            Register user = new Register();
            DataSet ds = new DataSet();
            String SqlCon = _configuration.GetConnectionString("DBConn");
            using (SqlConnection con = new SqlConnection(SqlCon))
            {
                
                using (SqlCommand cmd = new SqlCommand("sp_GetEnrollmentDetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email","abc@gmail.com");
                       
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
                        uobj.Resumepath = ds.Tables[0].Rows[i]["Resume"].ToString();
                        userlist.Add(uobj);
                    }
                    return userlist;
                }
            }

        }
        public string UploadedImage(Register model)
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
        public string UploadedFile(Register model)
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

        public byte[] ExporttoExcel<T>(List<T> table, string filename)
        {
            using ExcelPackage pack = new ExcelPackage();
            ExcelWorksheet ws = pack.Workbook.Worksheets.Add(filename);
            ws.Cells["A1"].LoadFromCollection(table, true, TableStyles.Light1);
            return pack.GetAsByteArray();
        }
    }
}
