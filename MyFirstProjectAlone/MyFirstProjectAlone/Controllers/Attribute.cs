using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using MyFirstProjectAlone.Controllers;


namespace MyFirstProjectAlone.Controllers
{
    public enum Roles
    {
        User = 1,
        Moderator,
        Admin
    }
    public class RequiresRoleAttributes : ActionFilterAttribute
    {
        public const string LoginToken = "c_user";
        private int roleID;
        public RequiresRoleAttributes()
        {
            roleID = 1;
        }
        public RequiresRoleAttributes(int role)
        {
            roleID = (int)role;
        }
        private bool CheckGuide(string guid)
        {
            using (SqlConnection con = new SqlConnection(CarsController.constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("uspCheckGuid", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Guid", SqlDbType.NVarChar).Value = guid;
                    int role = (int)(cmd.ExecuteScalar() ?? 0);
                    if (role < roleID)
                    {
                        return false;
                    }
                }
                const string dtFormat = "yyyy-MM-dd HH:mm:ss.fffffff zzz";

                using (SqlCommand cmd = new SqlCommand("uspUpdateLastLogin", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@LastLogin", SqlDbType.DateTimeOffset).Value = DateTimeOffset.Now.ToString(dtFormat);
                    var affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows < 1)
                    {
                        return false;
                    }
                }

            }
            return true;
        }
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var login = actionContext.Request.Headers.GetCookies(LoginToken).FirstOrDefault();
            if (login != null)
            {
                string guid = login[LoginToken].Value;
                if (CheckGuide(guid))
                {
                    using (SqlConnection con=new SqlConnection(CarsController.constr))
                    {
                        con.Open();
                        int userID;
                        using (SqlCommand cmd=new SqlCommand("uspGetUserID", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@Guid", SqlDbType.NVarChar).Value = guid;
                            userID = (int)(cmd.ExecuteScalar() ?? 0);
                            (actionContext.ControllerContext.Controller as CarsController).CurrentUserID = userID;
                        }
                    }
                    return;
                }
            }
            var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            actionContext.Response = response;  
        }
    }

    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.ModelState.IsValid == false)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, actionContext.ModelState);
            }
        }
    }
}