using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;

namespace Services
{
    /// <summary>
    /// Summary description for SignIn
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SignIn : System.Web.Services.WebService
    {
        [WebMethod]
        public int signingIn(string name, string password)
        {
            int id = -1;

            SqlConnection con = new SqlConnection("Data Source=./;Initial Catalog=BloodPressureTracker;Integrated Security=True");
            con.Open();

            SqlCommand cmd = new SqlCommand("select id from [user] where name = @name and password = @password", con);
            SqlParameter p1 = new SqlParameter("@name", name);
            SqlParameter p2 = new SqlParameter("@password", password);
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                id = int.Parse(dr["id"].ToString());
            }
            
            con.Close();

            return id;
        }
    }
}
