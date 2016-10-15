using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Web.Http.Controllers;
using CarController.Models;
using System.Text;
using Newtonsoft.Json;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Web;

namespace MyFirstProjectAlone.Controllers
{
    public class CarsController : ApiController
    {
        public const string constr = @"Data Source=(localdb)\ProjectsV13;Initial Catalog=CarDB1;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public int CurrentUserID{ get; set; }
        public IHttpActionResult Ok(bool scs, string msg)
        {
            return Ok(new
            {
                success = scs,
                message = msg
            });
        }
        [HttpPost,ValidateModel]
        public IHttpActionResult Register(UserModel registerModel)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("uspChekEmail", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = registerModel.Email;
                    int result = (int)(cmd.ExecuteScalar() ?? 0);
                    if (result > 0)
                    {
                        return Ok(false, "Bu email artıq bir dəfə qeydiyyatdan keçib! Yenidən cəhd edin!");
                    }
                }
                using (SqlCommand cmd = new SqlCommand("uspRegister", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = registerModel.FirstName;
                    cmd.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = registerModel.LastName;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = registerModel.Email;
                    cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = registerModel.Password;
                    cmd.Parameters.Add("@Number", SqlDbType.NVarChar).Value = registerModel.PhoneNumber;
                    cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = registerModel.Username;
                    var affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        return Ok(true, "Qeydiyyat uğurludur!");
                    }
                    else return Ok(false, "Qeydiyyat uğursuz oldu! Yenidən cəhd edin!");
                }
            }
        }
        [HttpPost,ValidateModel]
        public IHttpActionResult Login(UserModel model)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                int UserID;
                con.Open();
                using (SqlCommand cmd = new SqlCommand("uspLogin", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = model.Email;
                    cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = model.Password;
                    UserID = (int)(cmd.ExecuteScalar() ?? 0);
                    if (UserID <= 0)
                    {
                        return Ok(false, "Email və parolunuzu birdə yoxlayın!");
                    }
                }
                const string dtFormat = "yyyy-MM-dd HH:mm:ss.fffffff zzz";
                var now = DateTimeOffset.Now;
                var expireDate = now.AddMonths(3);
                string GuidStr = Guid.NewGuid().ToString().ToLower();
                using (SqlCommand cmd = new SqlCommand("uspInsertUserLogins",con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value =UserID ;
                    cmd.Parameters.Add("@Guid", SqlDbType.NVarChar).Value = GuidStr;
                    cmd.Parameters.Add("@LastLogin", SqlDbType.DateTimeOffset).Value = now.ToString(dtFormat);
                    cmd.Parameters.Add("@ExpireTime", SqlDbType.DateTimeOffset).Value = expireDate.ToString(dtFormat);
                    var affectedRows= cmd.ExecuteNonQuery();
                    if (affectedRows < 1)
                    {
                        return Ok(false,"Error,Try it again!");
                    }
                }

                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                var cookie = new CookieHeaderValue(RequiresRoleAttributes.LoginToken, GuidStr);

                cookie.Expires = expireDate;
                cookie.Domain = Request.RequestUri.Host;
                cookie.Path = "/";
                responseMessage.Headers.AddCookies(new[] { cookie });

                var successMsg = new { success = true, message = "Sucessfully logged in" };
                var param = JsonConvert.SerializeObject(successMsg);
                responseMessage.Content = new StringContent(param, Encoding.UTF8, "application/json");

                return ResponseMessage(responseMessage);
            }
        }

    }
}

