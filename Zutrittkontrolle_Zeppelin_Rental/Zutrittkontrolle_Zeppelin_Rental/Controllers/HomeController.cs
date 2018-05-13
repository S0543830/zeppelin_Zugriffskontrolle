using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Zutrittkontrolle_Zeppelin_Rental.Models;
using static Zutrittkontrolle_Zeppelin_Rental.Models.Enums;
 
namespace Zutrittkontrolle_Zeppelin_Rental.Controllers
{
    public class HomeController : Controller
    {
        //private dbZugriffskontrolleEntities1 _db = new dbZugriffskontrolleEntities1();
        public ActionResult Index()
        {
            
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "AutoId,FileName")] Files files, HttpPostedFileBase FileName)
        {
            if (ModelState.IsValid)
            {
                bool exists = System.IO.Directory.Exists(Server.MapPath("~/UserData/"));

                if (!exists)
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath("~/UserData/"));
                }
                    
                // upload the file now
                string path = Server.MapPath("~/UserData/");

                // get extension name of the selcted file
                string extensionName =
                System.IO.Path.GetExtension(FileName.FileName);
                // generate a random file name and appende extension name
                string finalFileName = DateTime.Now.Ticks.ToString() + extensionName;
                // now save the selected file with new name
                FileName.SaveAs(path + finalFileName);

                // set the final file name to the files object so the name would get
                //saved into the database
                files.FileName = finalFileName;
         
              //  db.Files.Add(files);
               // db.SaveChanges();

                return RedirectToAction("Index","Korrektur", files);
            }

            return View(files);
        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";
                        string filename = Path.GetFileName(Request.Files[i].FileName);

                        HttpPostedFileBase file = files[i];
                        string fname;
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        fname = Path.Combine(Server.MapPath("~/Uploads/"), fname);
                        file.SaveAs(fname);
                    }

                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

    }
}
