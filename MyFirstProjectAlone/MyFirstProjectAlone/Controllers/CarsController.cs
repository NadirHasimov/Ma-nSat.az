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
using MyFirstProjectAlone.Models;


namespace MyFirstProjectAlone.Controllers
{

    public class CarsController : ApiController
    {
        public const string constr = @"Data Source=(localdb)\ProjectsV13;Initial Catalog=CarDB1;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public int CurrentUserID { get; set; }

        public IHttpActionResult Ok(bool scs, string msg)
        {
            return Ok(new
            {
                success = scs,
                message = msg
            });
        }

        [HttpPost, ValidateModel]
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

        [HttpPost, ValidateModel]
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
                using (SqlCommand cmd = new SqlCommand("uspInsertUserLogins", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                    cmd.Parameters.Add("@Guid", SqlDbType.NVarChar).Value = GuidStr;
                    cmd.Parameters.Add("@LastLogin", SqlDbType.DateTimeOffset).Value = now.ToString(dtFormat);
                    cmd.Parameters.Add("@ExpireTime", SqlDbType.DateTimeOffset).Value = expireDate.ToString(dtFormat);
                    var affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows < 1)
                    {
                        return Ok(false, "Error,Try it again!");
                    }
                }

                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                var cookie = new CookieHeaderValue(RequiresRoleAttribute.LoginToken, GuidStr);

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

        [HttpPost, RequiresRole(Roles.User)]
        public IHttpActionResult Upload(CarModel model)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("uspInsertIntoCarsTBL", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@TypeOfCarID", SqlDbType.Int).Value = model.TypeOfCarID;
                    cmd.Parameters.Add("@MarkID", SqlDbType.Int).Value = model.MarkID;
                    cmd.Parameters.Add("@ModelID", SqlDbType.Int).Value = model.ModelID;
                    cmd.Parameters.Add("@Oturucu_ID", SqlDbType.Int).Value = model.Oturucu_ID;
                    cmd.Parameters.Add("@TypeOfEngineID", SqlDbType.Int).Value = model.TypeOfEngineID;
                    cmd.Parameters.Add("@TypesOfFuelID", SqlDbType.Int).Value = model.TypesOfFuelID;
                    cmd.Parameters.Add("@CapacityOfEngineID", SqlDbType.Int).Value = model.CapacityOfEngineID;
                    cmd.Parameters.Add("@ProducedTimeID", SqlDbType.Int).Value = model.ProducedTimeID;
                    cmd.Parameters.Add("@UserNameID", SqlDbType.Int).Value = CurrentUserID;
                    cmd.Parameters.Add("@Price", SqlDbType.Int).Value = model.Price;
                    cmd.Parameters.Add("@PowerOfEngine", SqlDbType.Int).Value = model.PowerOfEngine;
                    cmd.Parameters.Add("@ColourID", SqlDbType.Int).Value = model.ColourID;

                    cmd.Parameters.Add("@ABS_ID", SqlDbType.Int).Value = model.ABS_ID;
                    cmd.Parameters.Add("@YungulLehimliDiskler_ID", SqlDbType.Int).Value = model.YungulLEhimliDiskler_ID;
                    cmd.Parameters.Add("@Lyuk_ID", SqlDbType.Int).Value = model.Lyuk_ID;
                    cmd.Parameters.Add("@YagisSensoru_ID", SqlDbType.Int).Value = model.YagisSensoru_ID;
                    cmd.Parameters.Add("@MerkeziQapanma_ID", SqlDbType.Int).Value = model.MerkeziQapanma_ID;
                    cmd.Parameters.Add("@ParkRadari_ID", SqlDbType.Int).Value = model.ParkRadari_ID;
                    cmd.Parameters.Add("@Kondisoner_ID", SqlDbType.Int).Value = model.Kondisoner_ID;
                    cmd.Parameters.Add("@OturacaqlarinIsisdilmesi_ID", SqlDbType.Int).Value = model.OturacaqlarinItirilmesi_ID;
                    cmd.Parameters.Add("@DeriSalon_ID", SqlDbType.Int).Value = model.DeriSalon_ID;
                    cmd.Parameters.Add("@KsenonLampalar_ID", SqlDbType.Int).Value = model.KsenonLampalar_ID;
                    cmd.Parameters.Add("@ArxaGoruntuKamerasi_ID", SqlDbType.Int).Value = model.ArxaGoruntuKamerasi_ID;
                    cmd.Parameters.Add("@YanPerdeler_ID", SqlDbType.Int).Value = model.@YanPerdeler_ID;
                    cmd.Parameters.Add("@OturacaqlarinVentilyasiyasi_ID", SqlDbType.Int).Value = model.OturacaqlarinVentilyasiyasi_ID;
                    var affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows < 1)
                    {
                        return Ok(false, "Elan yerləşdirilə bilmədi! Yenidən cəhd edin !");
                    }
                    return Ok(true, "Elan uğurla yerləşdirildi!");
                }
            }
        }

        [HttpGet, RequiresRole]
        public IEnumerable<CarDataString> GetCurrentCar(int CarID)
        {
            List<CarDataString> model = new List<CarDataString>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("uspSelectCarINFO", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = CarID;
                    SqlDataReader reader = cmd.ExecuteReader();
                    int i = 0;
                    while (reader.Read())
                    {
                        model.Add(new CarDataString
                        {
                            TypeOfCar = reader.GetString(i++),
                            MarkName = reader.GetString(i++),
                            ModelName = reader.GetString(i++),
                            TypeOfEngine = reader.GetString(i++),
                            CapacityOfEngine = reader.GetInt32(i++),
                            TypeOfFuel = reader.GetString(i++),
                            ProducedTime = reader.GetInt32(i++),
                            UserName = reader.GetString(i++),
                            Oturucu = reader.GetString(i++),
                            Price = reader.GetInt32(i++),
                            Colour=reader.GetString(i++),
                            PowerOfEngine=reader.GetInt32(i++),

                            ABS_string = reader.GetString(i++),
                            ArxaGoruntuKamerasi_string = reader.GetString(i++),
                            DeriSalon_string = reader.GetString(i++),
                            Kondisoner_string = reader.GetString(i++),
                            KsenonLampalar_string = reader.GetString(i++),
                            Lyuk_string = reader.GetString(i++),
                            MerkeziQapanma_string = reader.GetString(i++),
                            OturacaqlarinItirilmesi_string = reader.GetString(i++),
                            OturacaqlarinVentilyasiyasi_string = reader.GetString(i++),
                            ParkRadari_string = reader.GetString(i++),
                            YagisSensoru_string = reader.GetString(i++),
                            YanPerdeler_string = reader.GetString(i++),
                            YungulLEhimliDiskler_string = reader.GetString(i++)
                        });
                    }
                }
            }
            return model;
        }

        [HttpGet]
        public IEnumerable<Get> GetTypesOfCar()
        {
            List<Get> model = new List<Get>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("uspGetTypesOfCar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        model.Add(new Get
                        {
                            ID = reader.GetInt32(0),
                            Text = reader.GetString(1)
                        });
                    }
                }
            }
            return model;
        }

        [HttpGet]
        public IEnumerable<Get> GetMarks(int TypeID)
        {
            List<Get> model = new List<Get>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("uspGetMarks", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@TypeID", SqlDbType.Int).Value = TypeID;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        model.Add(new Get
                        {
                            ID = reader.GetInt32(0),
                            Text = reader.GetString(1)
                        });
                    }
                }
            }
            return model;
        }

        [HttpGet]
        public IEnumerable<Get> GetModels(int markID)
        {
            List<Get> model = new List<Get>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("uspGetModels", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@markID", SqlDbType.Int).Value = markID;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        model.Add(new Get
                        {
                            ID = reader.GetInt32(0),
                            Text = reader.GetString(1)
                        });
                    }
                }
            }
            return model;
        }

        [HttpGet]
        public IEnumerable<Get> GetOturucu()
        {
            List<Get> model = new List<Get>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("uspGetOturucu", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        model.Add(new Get
                        {
                            ID = reader.GetInt32(0),
                            Text = reader.GetString(1)
                        });
                    }
                }
            }
            return model;
        }

        [HttpGet]
        public IEnumerable<Get> GetTypesOfEngine()
        {
            List<Get> model = new List<Get>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("uspGetTypesOfEngine", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        model.Add(new Get
                        {
                            ID = reader.GetInt32(0),
                            Text = reader.GetString(1)
                        });
                    }
                }
            }
            return model;
        }

        [HttpGet]
        public IEnumerable<Get> GetCapacityOfEngine()
        {
            List<Get> model = new List<Get>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("uspGetCapacityOfEngine", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        model.Add(new Get
                        {
                            ID = reader.GetInt32(0),
                            Text = reader.GetInt32(1).ToString()
                        });
                    }
                }
            }
            return model;
        }

        [HttpGet]
        public IEnumerable<Get> GetTypesOfFuel()
        {
            List<Get> model = new List<Get>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("uspGetTypesOfFuel", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        model.Add(new Get
                        {
                            ID = reader.GetInt32(0),
                            Text = reader.GetString(1)
                        });
                    }
                }
            }
            return model;
        }

        [HttpPost, RequiresRole(Roles.User)]
        public IHttpActionResult UpdatePrice(int carID, int Price)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("uspUpdateCarPrice", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@CarID", SqlDbType.Int).Value = carID;
                    cmd.Parameters.Add("@Price", SqlDbType.Int).Value = Price;
                    var affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows < 1)
                    {
                        return Ok(false, "Əməliyyat alınmadı! Yenidən cəhd edin!");
                    }
                    return Ok(true, "Əməliyyat yerinə yetirildi!");
                }
            }
        }

        [HttpPost, RequiresRole(Roles.Admin)]
        public IHttpActionResult SetRole(int UserID, int RoleID)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("uspSetRole", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                    cmd.Parameters.Add("RoleID", SqlDbType.Int).Value = RoleID;
                    var affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows < 1)
                    {
                        return Ok(false, "Əməliyyat alınmadı! Yenidən cəhd edin!");
                    }
                    return Ok(true, "Əməliyyat yerinə yetirildi!");
                }
            }
        }

        [HttpGet, RequiresRole(Roles.Admin)]
        public IEnumerable<Get> getRoles()
        {
            List<Get> model = new List<Get>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("uspGetRoles", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        model.Add(new Get
                        {
                            ID = reader.GetInt32(0),
                            Text = reader.GetString(1)
                        });
                    }
                }
            }
            return model;
        }

        [HttpPost, RequiresRole(Roles.Moderator)]
        public IHttpActionResult DeleteAdvertise(int CarID)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("uspDeleteCarID"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@CarID", SqlDbType.Int).Value = CarID;
                    var affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows < 1)
                    {
                        return Ok(false, "Əməliyyat yerinə bilmədi!");
                    }
                    return Ok(true, "Əməliyyat uğurla yerinə yetirildi!");
                }
            }
        }
        public class nameDoesNotMatter
        {
            public IEnumerable<int> IDs;
        }

        [HttpPost, RequiresRole(Roles.Moderator)]
        public IHttpActionResult ConfirmAdvertise(nameDoesNotMatter model)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("uspConfirmAdvertises", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter Param = cmd.Parameters.AddWithValue("@CarIDS", CreateDataTable(model.IDs));

                    Param.SqlDbType = SqlDbType.Structured;
                    Param.TypeName = "IntListType";

                    int affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows < 1)
                    {
                        return Ok(false, "Əməliyyat alınmadı! ");
                    }
                    return Ok(true, "Əməliyyat yerinə yetirildi!");
                }
            }
        }

        [HttpPost, RequiresRole(Roles.Moderator)]
        public IHttpActionResult DeleteAdvertises(nameDoesNotMatter model)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("uspDeleteAdvertises", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter Param = cmd.Parameters.AddWithValue("CarIDs", CreateDataTable(model.IDs));

                    Param.SqlDbType = SqlDbType.Structured;
                    Param.TypeName = "IntListType";

                    int affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows < 1)
                    {
                        return Ok(false, "Əməliyyat yerinə yetirilə bilmədi !");
                    }
                    return Ok(true, "Əməliyyat uğurla yerinə yetirldi!");
                }
            }
        }

        [HttpPost, RequiresRole(Roles.User)]
        public FilterResponseModel GetFilteredCars(FilterModel model)
        {
            int i = 0;
            int countOfCars = 0;
            List<CarDataString> model1 = new List<CarDataString>();
                    
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("uspGetFilteredCars", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter Param = cmd.Parameters.AddWithValue("@TypeOfCarIDs", CreateDataTable(model.TypeOfCar_IDs));
                    Param.SqlDbType = SqlDbType.Structured;
                    Param.TypeName = "IntListType";

                    Param = cmd.Parameters.AddWithValue("@MarkIDs", CreateDataTable(model.Mark_IDs));
                    Param.SqlDbType = SqlDbType.Structured;
                    Param.TypeName = "IntListType";

                    Param = cmd.Parameters.AddWithValue("@ModelIDs", CreateDataTable(model.Model_IDs));
                    Param.SqlDbType = SqlDbType.Structured;
                    Param.TypeName = "IntListType";

                    Param = cmd.Parameters.AddWithValue("@OturucuIDs", CreateDataTable(model.Oturucu_IDs));
                    Param.SqlDbType = SqlDbType.Structured;
                    Param.TypeName = "IntListType";

                    Param = cmd.Parameters.AddWithValue("@TypesOfFuelIDs", CreateDataTable(model.TypesOfFuel_IDs));
                    Param.SqlDbType = SqlDbType.Structured;
                    Param.TypeName = "IntListType";


                    Param = cmd.Parameters.AddWithValue("ColourIDs", CreateDataTable(model.ColourIDs));
                    Param.SqlDbType = SqlDbType.Structured;
                    Param.TypeName = "IntListType";

                    cmd.Parameters.Add("@LowPrice", SqlDbType.Int).Value = model.LowPirce;
                    cmd.Parameters.Add("@HighPrice", SqlDbType.Int).Value = model.HighPrice;

                    cmd.Parameters.Add("@LowProducedTimeID", SqlDbType.Int).Value = model.MinProducedTimeID;
                    cmd.Parameters.Add("@HighProducedTimeID", SqlDbType.Int).Value = model.MaxProducedTimeID;

                    cmd.Parameters.Add("@LowCapacityOfEngineID", SqlDbType.Int).Value = model.LowCapacityOfEngineID;
                    cmd.Parameters.Add("@HighCapacityOfEngineID", SqlDbType.Int).Value = model.HighCapacityOfEngineID;

                    cmd.Parameters.Add("@PageNumber", SqlDbType.Int).Value = model.PageNumber;
                    cmd.Parameters.Add("@PageLength", SqlDbType.Int).Value = model.PageLength;
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        countOfCars = reader.GetInt32(0);
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        model1.Add(new CarDataString
                        {
                            TypeOfCar = reader.GetString(i++),//1
                            MarkName = reader.GetString(i++),//2
                            ModelName = reader.GetString(i++),//3
                            TypeOfEngine = reader.GetString(i++),//4
                            CapacityOfEngine = reader.GetInt32(i++),//5
                            TypeOfFuel = reader.GetString(i++),//6
                            ProducedTime = reader.GetInt32(i++),//7
                            UserName = reader.GetString(i++),//8
                            Oturucu = reader.GetString(i++),//9
                            Price = reader.GetInt32(i++),//10
                            Colour=reader.GetString(i++),//11
                            PowerOfEngine=reader.GetInt32(i++),//12

                            ABS_string = reader.GetString(i++),//13
                            ArxaGoruntuKamerasi_string = reader.GetString(i++),//14
                            DeriSalon_string = reader.GetString(i++),//15
                            Kondisoner_string = reader.GetString(i++),//16
                            KsenonLampalar_string = reader.GetString(i++),//17
                            Lyuk_string = reader.GetString(i++),//18
                            MerkeziQapanma_string = reader.GetString(i++),//19
                            OturacaqlarinItirilmesi_string = reader.GetString(i++),//20
                            OturacaqlarinVentilyasiyasi_string = reader.GetString(i++),//21
                            ParkRadari_string = reader.GetString(i++),//22
                            YagisSensoru_string = reader.GetString(i++),//23
                            YanPerdeler_string = reader.GetString(i++),//24
                            YungulLEhimliDiskler_string = reader.GetString(i++)//25
                        });
                    }
                }
            }
            return new FilterResponseModel
            {
                CountOfCars=countOfCars,
                Cars = model1
            };
        }

        [HttpPost, RequiresRole(Roles.User)]
        private static DataTable CreateDataTable<T>(IEnumerable<T> items)
        {
            DataTable dbTable = new DataTable();
            dbTable.Columns.Add("item", typeof(T));
            foreach (T item in items)
            {
                dbTable.Rows.Add(item);
            }
            return dbTable;
        }
    }

}

