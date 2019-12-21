using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;

namespace Services
{
    /// <summary>
    /// Summary description for SignUp
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SignUp : System.Web.Services.WebService
    {

        [WebMethod]
        public int signingUp(string name, int weight, int age, char gender, string bloodPressure, string email, string password)
        {
            int userId = -1;
            int dietId = 0;

            SqlConnection con = new SqlConnection("Data Source=./;Initial Catalog=BloodPressureTracker;Integrated Security=True");
            con.Open();
            SqlCommand cmd = new SqlCommand("select id from [user] where name = @name", con);
            SqlParameter p1 = new SqlParameter("@name", name);
            cmd.Parameters.Add(p1);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                userId = int.Parse(dr["id"].ToString());
            }
            con.Close();

            if (userId != -1)
                return -1;

            con.Open();

            cmd = new SqlCommand("insert into [user] (name,weight,age,gender,email,password) values (@name, @weight, @age, @gender, @email, @password)", con);
            p1 = new SqlParameter("@name", name);
            SqlParameter p2 = new SqlParameter("@weight", weight);
            SqlParameter p3 = new SqlParameter("@age", age);
            SqlParameter p4 = new SqlParameter("@gender", gender);
            SqlParameter p5 = new SqlParameter("@email", email);
            SqlParameter p6 = new SqlParameter("@password", password);
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);
            cmd.Parameters.Add(p5);
            cmd.Parameters.Add(p6);
            cmd.ExecuteNonQuery();
            con.Close();

            con.Open();
            cmd = new SqlCommand("select id from [user] where name = @name", con);
            p1 = new SqlParameter("@name", name);
            cmd.Parameters.Add(p1);
            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                userId = int.Parse(dr["id"].ToString());
            }
            con.Close();

            con.Open();
            cmd = new SqlCommand("insert into bloodPressure (userId,dateTime,pressureSample,dietId) values (@userId, @dateTime, @pressureSample, @dietId)", con);
            p1 = new SqlParameter("@userId", userId);
            p2 = new SqlParameter("@dateTime", DateTime.Now.ToString());
            p3 = new SqlParameter("@pressureSample", bloodPressure);
            if (bloodPressure == "120/80")
            {
                dietId = 2;
            }
            else if (bloodPressure == "130/90")
            {
                dietId = 3;
            }
            else
            {
                dietId = 1;
            }
            p4 = new SqlParameter("@dietId", dietId);
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);
            cmd.ExecuteNonQuery();
            con.Close();

            return userId;
        }
    }
}
