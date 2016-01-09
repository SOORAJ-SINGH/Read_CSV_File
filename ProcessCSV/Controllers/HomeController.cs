using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ProcessCSV.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult UploadCSVFile(HttpPostedFileBase csvfile)
        {
            // Set up DataTable place holder
            DataTable dt = new DataTable();

            //check we have a file
            if (csvfile.ContentLength > 0)
            {
                //Workout our file path
                string fileName = Path.GetFileName(csvfile.FileName);
                string path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);

                //Try and upload
                try
                {
                    csvfile.SaveAs(path);
                    //Process the CSV file and capture the results to our DataTable place holder
                    dt = ProcessCSV(path);

                }
                catch (Exception ex)
                {
                    //Catch errors
                    ViewData["Feedback"] = ex.Message;
                }
            }
            else
            {
                //Catch errors
                ViewData["Feedback"] = "Please select a file";
            }

            //Tidy up
            dt.Dispose();

            return View("Index", ViewData["Feedback"]);
        }

        private static DataTable ProcessCSV(string fileName)
        {
            //Set up our variables
            string Feedback = string.Empty;
            string line = string.Empty;
            string[] strArray;
            DataTable dt = new DataTable();
            DataRow row;
            // work out where we should split on comma, but not in a sentence
            Regex r = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            //Set the filename in to our stream
            StreamReader sr = new StreamReader(fileName);

            //Read the first line and split the string at , with our regular expression in to an array
            line = sr.ReadLine();
            strArray = r.Split(line);
            string[] newArray = line.Split(',');
            //For each item in the new split array, dynamically builds our Data columns. Save us having to worry about it.
            Array.ForEach(strArray, s => dt.Columns.Add(new DataColumn()));

            //Read each line in the CVS file until it’s empty
            while ((line = sr.ReadLine()) != null)
            {
                row = dt.NewRow();

                //add our current value to our data row
                row.ItemArray = r.Split(line);
                dt.Rows.Add(row);
            }

            //Tidy Streameader up
            sr.Dispose();

            //return a the new DataTable
            return dt;

        }
    }
    
}