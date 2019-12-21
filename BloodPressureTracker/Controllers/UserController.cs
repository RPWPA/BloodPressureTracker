using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BloodPressureTracker.Models;
using System.Data.SqlClient;
using System.Data;

namespace BloodPressureTracker.Controllers
{
    public class UserController : Controller
    {
        // GET: User/Register
        public ActionResult register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Rauthorize(user User)
        {
            try
            {
                SignUpServiceReference.SignUpSoapClient S = new SignUpServiceReference.SignUpSoapClient();
                int userID = S.signingUp(User.name, User.weight, User.age, User.gender[0], User.BPsample, User.email, User.password);
                if (userID != -1)
                {
                    Session["UserID"] = userID;

                    return RedirectToAction("userHome", "User");
                }
                else
                {
                    User.LoginErrorMsg = "Choose Another User Name";
                    return View("register", User);
                }
            }
            catch { return View("register", User); }
        }

        static Dictionary<int, DateTime> dict = new Dictionary<int, DateTime>();

        public ActionResult userHome()
        {
            bloodPressure P = new bloodPressure();
            P.RemainTime = 0;
            return View(P);
        }

        public ActionResult EnterSample(bloodPressure B)
        {
            SqlConnection conn = new SqlConnection("Data Source=./;Initial Catalog=BloodPressureTracker;Integrated Security=True");

            conn.Open();

            SqlCommand cmd = new SqlCommand("insert into bloodPressure (userId,dietId,pressureSample,dateTime) values (@userId, @dietId, @pressureSample, @dateTime)", conn);
            
            int dietId;
            if (B.pressureSample == "120/80")
            {
                dietId = 2;
            }
            else if (B.pressureSample == "130/90")
            {
                dietId = 3;
            }
            else
            {
                dietId = 1;
            }
            B.dateTime = DateTime.Now.ToString();
            B.dietId = dietId;
            B.userId = int.Parse(Session["UserID"].ToString());
            SqlParameter p1 = new SqlParameter("@userId", B.userId);
            SqlParameter p2 = new SqlParameter("@dietId", B.dietId);
            SqlParameter p3 = new SqlParameter("@pressureSample", B.pressureSample);
            SqlParameter p4 = new SqlParameter("@dateTime", DateTime.Now.ToString());
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);
            cmd.ExecuteNonQuery();
            conn.Close();

            return RedirectToAction("userHome", "User");
        }

        [HttpPost]
        public ActionResult Remind()
        {
            EmailServiceReference.EmailServiceClient s = new EmailServiceReference.EmailServiceClient();
            WCFServiceReference.WCFServiceClient sr = new WCFServiceReference.WCFServiceClient();
            DataTable DT = sr.ViewInfo(int.Parse(Session["UserID"].ToString()));
            string temp_email = DT.Rows[0][5].ToString();

            int temp_minutes = 0;

            string strOption = Request.Form["RemindOptions"].ToString();
            string[] emailOptions = Request.Form["emailOption"].ToString().Split(',');
            string emailOption = emailOptions[0];
            bloodPressure P = new bloodPressure();
            DateTime D = DateTime.Now;
            if (strOption == "OneMinute")
            {
                P.RemainTime = 1;
                temp_minutes = 1;
            }
            else if (strOption == "OneHour")
            {
                P.RemainTime = 11;
                temp_minutes = 60;
            }
            else if (strOption == "Twohours")
            {
                P.RemainTime = 2;
                temp_minutes = 2 * 60;

            }
            else if (strOption == "ThreeHours")
            {
                P.RemainTime = 3;
                temp_minutes = 3 * 60;
            }

            if (emailOption == "true")
            {
                s.remind_userByEmail(temp_email, (DateTime.Now).AddMinutes(temp_minutes));
            }

            return View("userHome", P);
        }

        // GET: User/dietPlan
        public ActionResult dietPlan()
        {
            WCFServiceReference.WCFServiceClient S = new WCFServiceReference.WCFServiceClient();
            string Diet = S.ViewDiet(int.Parse(Session["UserID"].ToString()));
            diet D = new diet();
            D.Meals = Diet;
            return View(D);
        }

        // GET: User/viewProfile
        public ActionResult viewProfile()
        {
            user U = retriveUser();
            return View(U);
        }
        public user retriveUser()
        {
            WCFServiceReference.WCFServiceClient S = new WCFServiceReference.WCFServiceClient();
            DataTable DT = S.ViewInfo(int.Parse(Session["UserID"].ToString()));

            user U = new user();
            U.name = DT.Rows[0][1].ToString();
            U.weight = int.Parse(DT.Rows[0][2].ToString());
            U.age = int.Parse(DT.Rows[0][3].ToString());
            U.gender = DT.Rows[0][4].ToString();
            U.email = DT.Rows[0][5].ToString();


            return U;
        }

        // GET: User/updateProfile
        public ActionResult updateProfile()
        {
            return View();
        }
        public ActionResult UpdateUser(user U)
        {
            WCFServiceReference.WCFServiceClient S = new WCFServiceReference.WCFServiceClient();
            S.UpdateInformation(int.Parse(Session["UserID"].ToString()), U.email, U.weight, U.age, U.gender[0]);

            return RedirectToAction("viewProfile", "User");
        }
        
        public ActionResult graph(int id)
        {
            SqlConnection conn = new SqlConnection("Data Source=./;Initial Catalog=BloodPressureTracker;Integrated Security=True");
            List<bloodPressure> Samples = new List<bloodPressure>();

            conn.Open();

            SqlCommand cmd = new SqlCommand("select * from bloodPressure where userId=@x", conn);
            SqlParameter p = new SqlParameter("@x", id);
            cmd.Parameters.Add(p);
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                bloodPressure P = new bloodPressure();
                P.userId = int.Parse(dr[0].ToString());
                P.dietId = int.Parse(dr[1].ToString());
                P.pressureSample = dr[2].ToString();
                P.dateTime = dr[3].ToString();

                Samples.Add(P);
            }
            dr.Close();

            conn.Close();

            return View(Samples);
        }

    }
}