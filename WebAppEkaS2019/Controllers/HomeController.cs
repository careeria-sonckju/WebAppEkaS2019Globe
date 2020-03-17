using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Mvc;
using WebAppEkaS2019.Models;

namespace WebAppEkaS2019.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Tietoja Northwind-yhtiöstä ja Careeriasta";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Yhteystiedot";

            return View();
        }

        public ActionResult Laskuri()
        {
            string ilmoitus = "";
            int i = 0;
            string connStr = "Server=L17HE01A;Database=NorthwindLargeDev;Trusted_Connection=False;User Id=sa;Password=Careeria2019;"; //<--oman koneen SQL Serverin sekä tietokannan nimi
            SqlConnection conn = new SqlConnection(connStr);
            conn.Open();

            string sql = "Select * from customers where country = 'Finland'";
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                i++;
                string otsikko0 = reader.GetName(0);
                string kentta0 = reader.GetString(0);
                string kentta1 = reader.GetString(1);
                string kentta2 = reader.GetString(2);
                ilmoitus = String.Format("Rivi{4}: {0} = {1}  {2}  {3}", otsikko0, kentta0, kentta1, kentta2, i.ToString());
            }
            reader.Close();
            cmd.Dispose();
            conn.Dispose();

            string apumuuttuja="";

            //for (int i = 0; i < 10; i++)
            //{
            //    apumuuttuja = apumuuttuja + "Laskurin arvo on nyt = " + i.ToString();
            //}

            ViewBag.Message = ilmoitus;
            return View();
        }

        public ActionResult Map()
        {
            ViewBag.Message = "Saapumisohje";
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Authorize(Logins LoginModel)
        {
            northwindEntities db = new northwindEntities();
            //Haetaan käyttäjän/Loginin tiedot annetuilla tunnustiedoilla tietokannasta LINQ -kyselyllä
            var LoggedUser = db.Logins.SingleOrDefault(x => x.UserName == LoginModel.UserName && x.PassWord == LoginModel.PassWord);
            if (LoggedUser != null)
            {
                ViewBag.LoginMessage = "Successfull login";
                ViewBag.LoggedStatus = "In";
                Session["UserName"] = LoggedUser.UserName;
                Session["LoginID"] = LoggedUser.LoginId;
                //Session["AccessLevel"] = LoggedUser.AccessLevel;
                return RedirectToAction("Index", "Home"); //Tässä määritellään mihin onnistunut kirjautuminen johtaa --> Home/Index
            }
            else
            {
                ViewBag.LoginMessage = "Login unsuccessfull";
                ViewBag.LoggedStatus = "Out";
                LoginModel.LoginErrorMessage = "Tuntematon käyttäjätunnus tai salasana.";
                return View("Login", LoginModel);
            }
        }
        public ActionResult LogOut()
        {
            Session.Abandon();
            ViewBag.LoggedStatus = "Out";
            return RedirectToAction("Index", "Home"); //Uloskirjautumisen jälkeen pääsivulle
        }
    }




}