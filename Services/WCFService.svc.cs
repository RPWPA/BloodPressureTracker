using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WCFService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WCFService.svc or WCFService.svc.cs at the Solution Explorer and start debugging.
    public class WCFService : IWCFService
    {
        public void UpdateInformation(int id, string email, int weight, int age, char gender)
        {
            SqlConnection con = new SqlConnection("Data Source=./;Initial Catalog=BloodPressureTracker;Integrated Security=True"); // 
            con.Open();
            SqlCommand cmd = new SqlCommand("update [user] set email=@e , weight = @w , age=@a , gender=@g  where id=@i", con);
            SqlParameter p1 = new SqlParameter("@e", email);
            SqlParameter p2 = new SqlParameter("@w", weight);
            SqlParameter p3 = new SqlParameter("@a", age);
            SqlParameter p4 = new SqlParameter("@g", gender);
            SqlParameter p5 = new SqlParameter("@i", id);
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);
            cmd.Parameters.Add(p5);
            cmd.ExecuteNonQuery();
            con.Close();
        }
        public DataTable ViewInfo(int id)
        {
            SqlConnection con = new SqlConnection("Data Source=./;Initial Catalog=BloodPressureTracker;Integrated Security=True");
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from [user] where id =@i", con);

            SqlParameter p1 = new SqlParameter("@i", id);
            cmd.Parameters.Add(p1);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dt.TableName = "User";
            con.Close();
            return dt;

        }
        public DataTable Viewbloodhistory(int id)
        {
            SqlConnection con = new SqlConnection("Data Source=./;Initial Catalog=BloodPressureTracker;Integrated Security=True");
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from bloodPressure where userId =@i", con);
            SqlParameter p1 = new SqlParameter("@i", id);
            cmd.Parameters.Add(p1);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dt.TableName = "blood";
            con.Close();
            return dt;
        }
        public string ViewDiet(int id)
        {
            SqlConnection con = new SqlConnection("Data Source=./;Initial Catalog=BloodPressureTracker;Integrated Security=True");
            con.Open();
            SqlCommand cmd = new SqlCommand("select dietId from bloodPressure where userId = @i", con);
            SqlParameter p1 = new SqlParameter("@i", id);
            cmd.Parameters.Add(p1);
            SqlDataReader dr = cmd.ExecuteReader();
            int dietId = 0;
            while (dr.Read())
            {
                dietId = int.Parse(dr["dietId"].ToString());
            }
            con.Close();

            var dataSet = new DataSet();
            con = new SqlConnection("Data Source=./;Initial Catalog=BloodPressureTracker;Integrated Security=True");

            con.Open();

            cmd = new SqlCommand("select Meals from diet where id =@i", con);
            p1 = new SqlParameter("@i", dietId);
            cmd.Parameters.Add(p1);


            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dt.TableName = "diet";

            string Meals = dt.Rows[0][0].ToString();

            con.Close();
            return Meals;

        }
    }
}
