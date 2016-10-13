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
        public IHttpActionResult Ok(bool scs, string msg)
        {
            return Ok(new
            {
                success = scs,
                message = msg
            });
        }
        [HttpPost]
        public IHttpActionResult Register(UserModel registerModel)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("uspChekEmail"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = registerModel.Email;
                    int result = (int)(cmd.ExecuteScalar() ?? 0);
                    if (result > 0)
                    {
                        return Ok(false, "Bu email artıq bir dəfə daxil edilib! Yenidən cəhd edin!");
                    }
                }
                using (SqlCommand cmd = new SqlCommand("uspRegister"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = registerModel.FirstName;
                    cmd.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = registerModel.LastName;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = registerModel.Email;
                    cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = registerModel.Password;
                    cmd.Parameters.Add("Number", SqlDbType.NVarChar).Value = registerModel.PhoneNumber;
                    var affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows > 1)
                    {
                        return Ok(true, "Qeydiyyat uğurludur!");
                    }
                    else return Ok(false, "Qeydiyyat uğursuz oldu! Yenidən cəhd edin!");
                }
            }
        }
    }
}
}
