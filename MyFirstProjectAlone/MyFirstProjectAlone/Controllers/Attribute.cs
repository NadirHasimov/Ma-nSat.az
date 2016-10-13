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
        User=1,
        Moderator,
        Admin
    }
    public class RequiresRoleAttributes: ActionFilterAttribute
    {
        public const string LoginToken = "c_user";
        private int roleID;
        public RequiresRoleAttributes()
        {
            roleID = 1;
        }
        public RequiresRoleAttributes(int role)
        {
            roleID =(int)role;
        }
        private bool CheckGuide(string guid)
        {
            using (SqlConnection con=new SqlConnection(CarsController.constr))
            {
                con.Open();
                using (SqlCommand cmd=new SqlCommand("uspCheckGuid",con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Guid", SqlDbType.NVarChar).Value = guid;
                    int role = (int)(cmd.ExecuteScalar() ?? 0);
                    if (role < roleID)
                    {
                        return false;
                    }
                }
                //

            }
            return false;
        }
    }
}