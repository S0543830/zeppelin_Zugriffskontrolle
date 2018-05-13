using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Zutrittkontrolle_Zeppelin_Rental.Models;
using Newtonsoft.Json;

namespace Zutrittkontrolle_Zeppelin_Rental.Controllers
{
    public class KorrekturController : Controller
    {
        private string _fileName { get; set; }
        // GET: Korrektur
        public ActionResult Index(Files files)
        {
            string file = Server.MapPath("~/UserData/" + files.FileName);
            //deserialize JSON from file  
            string Json = System.IO.File.ReadAllText(file);
            JavaScriptSerializer ser = new JavaScriptSerializer();
            JsonFileViewModel viewModel = new JsonFileViewModel();
            viewModel.cRFIDs = ser.Deserialize<List<CRFID>>(Json);
            viewModel.fileName = files.FileName;
            _fileName = files.FileName;
            string createText = file;
            using (StreamWriter sw = new StreamWriter(Server.MapPath("~/UserData/")+"tmp.txt"))
            {
                sw.WriteLine(createText);
                sw.Flush();
            }
            return View(viewModel);
            
        }

        /*
        **Regelwerk**
     
     - maximale Anwesenheit: 8 Stunden
     - Korrekturen erfolgen täglich für die Vortage
     - beim Überschreiten der max. Anwesenheit erfolgt eine automatische Gehen-Buchung (Korrekturintervall ausnahmsweise  im Minutenraster, zum Beispiel 15 Minuten)
     - beim Fehlen einer Kommen-Buchung wird diese, ausgehend von einer vorhandenen Gehen-Buchung und unter Berücksichtigung der maximalen Anwesenheit und vorheriger Gehen-Buchungen, automatisch erzeugt
     - beim Überschreiten der Tagesgrenze – egal in welche Richtung – wird für 23:59:59 eine Gehen-Buchung und für 00:00:00 eine Kommen-Buchung erzeugt
     -	nachgemeldete Buchungen (Offline-Fall) ersetzten automatische Buchungen, aber nur innerhalb des maximalen Anwesenheitsfensters
      
             
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(JsonFileViewModel _viewModel)
        {
            string s="";
            using (StreamReader sr = new StreamReader(Server.MapPath("~/UserData/") + "tmp.txt"))
            {
                
                while ((s = sr.ReadLine()) != null)
                {
                    _fileName = s;
                }
            }
            string Json = System.IO.File.ReadAllText(_fileName);
            JavaScriptSerializer ser = new JavaScriptSerializer();
            JsonFileViewModel viewModel = new JsonFileViewModel();
            List<CcorrectModel> ldbModel = ConvertJsonToModel(ser.Deserialize<List<CRFID>>(Json));
            CorrectJsonData(ldbModel);

            string json = JsonConvert.SerializeObject(ConvertModelToJson(ldbModel).ToArray(),Formatting.Indented);
            System.IO.File.WriteAllText(_fileName.Insert(_fileName.Length-5,"_correct"), json);

            CompareViewModel compareViewModel = new CompareViewModel
            {
                _oldModel = ConvertJsonToModel(ser.Deserialize<List<CRFID>>(Json)),
                _newModel = ldbModel
            };
            return View("Compare",compareViewModel);
        }

        void CorrectJsonData(List<CcorrectModel> ldbModel)
        {
            for (int i = 0; i < ldbModel.Count; i++)
            {
                for (int j = 1; j < ldbModel[i].LcorrectModel.Count; j++)
                {
                    if (false == CheckTimeInterval(ldbModel[i].LcorrectModel[j - 1].EventTime, ldbModel[i].LcorrectModel[j].EventTime))
                    {
                        if (ldbModel[i].LcorrectModel[j - 1].EventType != ldbModel[i].LcorrectModel[j].EventType)
                        {
                            ;
                        }
                        else if (ldbModel[i].LcorrectModel[j - 1].EventType == "ENTER")
                        {
                            ldbModel[i].LcorrectModel.Insert(j, AddNewJsonElement(ldbModel, i, j, "LEAVE"));
                        }
                        else if (ldbModel[i].LcorrectModel[j - 1].EventType == "LEAVE")
                        {
                            ldbModel[i].LcorrectModel.Insert(j, AddNewJsonElement(ldbModel, i, j, "ENTER"));
                        }
                    }
                    else
                    {
                        if (ldbModel[i].LcorrectModel[j - 1].EventType == ldbModel[i].LcorrectModel[j].EventType)
                        {
                            if (ldbModel[i].LcorrectModel[j - 1].EventType == "ENTER")
                            {
                                ldbModel[i].LcorrectModel.Insert(j, AddNewJsonElement(ldbModel, i, j, "LEAVE"));
                            }
                            else
                            {
                                ldbModel[i].LcorrectModel.Insert(j, AddNewJsonElement(ldbModel, i, j, "ENTER"));
                            }
                        }
                        else if (ldbModel[i].LcorrectModel[j - 1].EventType == "ENTER")
                        {
                            ldbModel[i].LcorrectModel[j].EventSource = "SYS";
                            ldbModel[i].LcorrectModel[j].EventTime = ldbModel[i].LcorrectModel[j - 1].EventTime.AddHours(8);
                        }
                    }

                }
            }
        }

        CRFID AddNewJsonElement(List<CcorrectModel> lIn,int i,int j, string Event)
        {
            CRFID tmpL = new CRFID();
            if (Event == "LEAVE")
            {
                tmpL.EventSource = "SYS";
                tmpL.EventType = "LEAVE";
                tmpL.RFID = lIn[i].RFID;
                tmpL.EventTime = new DateTime(lIn[i].LcorrectModel[j - 1].EventTime.Year, lIn[i].LcorrectModel[j - 1].EventTime.Month, lIn[i].LcorrectModel[j - 1].EventTime.Day, 23, 59, 59);
            }
            else
            {
                tmpL.EventSource = "SYS";
                tmpL.EventType = "ENTER";
                tmpL.RFID = lIn[i].RFID;
                tmpL.EventTime = new DateTime(lIn[i].LcorrectModel[j].EventTime.Year, lIn[i].LcorrectModel[j].EventTime.Month, lIn[i].LcorrectModel[j].EventTime.Day, 00, 00, 00);
            }
            return tmpL;
        }

        List<CRFID> ConvertModelToJson(List<CcorrectModel> lIn)
        {
            List<CRFID> tmp = new List<CRFID>();
            for(int i = 0;i<lIn.Count;i++)
            {
                for(int j = 0;j< lIn[i].LcorrectModel.Count;j++)
                {
                    tmp.Add(lIn[i].LcorrectModel[j]);
                }
            }
            return tmp;
        }

        List<CcorrectModel> ConvertJsonToModel(List<CRFID> lIn)
        {
            List<CcorrectModel> ltmp = new List<CcorrectModel>();
            CcorrectModel modeltmp = new CcorrectModel();
            for(int i = 0;i< lIn.Count;i++)
            {
                if(0==i)
                {
                    modeltmp.RFID = lIn[i].RFID;
                    modeltmp.LcorrectModel = new List<CRFID>();
                    modeltmp.LcorrectModel.Add(lIn[i]);
                    ltmp.Add(modeltmp);
                }
                else
                {
                    for(int j = 0;j<ltmp.Count;j++)
                    {
                        if(ltmp[j].RFID == lIn[i].RFID)
                        {
                            ltmp[j].LcorrectModel.Add(lIn[i]);
                            break;
                        }
                        if(j == ltmp.Count-1)
                        {
                            modeltmp = new CcorrectModel();
                            modeltmp.RFID = lIn[i].RFID;
                            modeltmp.LcorrectModel = new List<CRFID>();
                            modeltmp.LcorrectModel.Add(lIn[i]);
                            ltmp.Add(modeltmp);
                            break;
                        }
                    }
                }
            }
            return ltmp;
        }

        bool CheckTimeInterval(DateTime first,DateTime second)
        {
            bool iRet = false;
            TimeSpan time = first - second;
            if (Math.Abs(time.Hours)>8)
            {
                iRet = true;
            }
            else
            {
                iRet = false;
            }
            return iRet;
        }
    }
}