using EventApplication.Models;
using EventApplication.PageModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventApp_v2.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {

            try
            {
                using (MassCodeEntities db = new MassCodeEntities())
                {
                    db.Configuration.LazyLoadingEnabled = true;
                    db.Configuration.ProxyCreationEnabled = false;
                    var context = db.Events.Select(x => x.AreaName).Distinct().AsQueryable();
                    //var context = db.Locations.Select(x => x.Lokasyon).Distinct().AsQueryable();

                    var pageModel = new EventSelectModel
                    {
                        AreaNames = context.ToList(),
                        Sources = db.Events.Select(x => x.Source).Distinct().ToList()
                        //Sources = db.Locations.Select(x => x.Point).Distinct().ToList()
                    };
                    db.Dispose();
                    return View(pageModel);
                }
            }
            catch (Exception ex)
            {

                return null;
            }
            //return View();
        }
        
        public JsonResult GetSource(EventSelectModel model, string AreaName)
        {
            var resultModel = new EventSelectModel();
            try
            {
                using (MassCodeEntities db = new MassCodeEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    db.Configuration.LazyLoadingEnabled = true;

                    if (AreaName=="All")
                    {
                        model.Sources = db.Events.Select(x => x.Source).Distinct().ToList();
                    }
                    else
                    {
                        //var selected = db.Locations.FirstOrDefault(x => x.Lokasyon == AreaName);
                        //string keyword = selected.Keyword;
                        model.Sources = db.Events.Where(x => x.AreaName == AreaName).Select(x => x.Source).Distinct().ToList();
                        //model.Sources = db.Locations.Where(x => x.Lokasyon == AreaName).Select(x => x.Point).ToList();
                    }
                    //var sourcePoints = db.Events.Where(x => x.AreaName == AreaName).AsQueryable();
                    //var sourceList = sourcePoints.Select(x => x.Source).Distinct().ToList();
                    
                    //model.Sources.Add(item.Source);

                    //model.Sources = db.Context.Events.Where(x => x.AreaName == AreaName).ToList();

                }
            }
            catch (Exception ex)
            {

            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEventDetail(DataTableModel<EventTableModel> model, DateTime endDate, DateTime startDate, string areaName, string sourceName, bool allArea, bool allSource, bool isMaxOne, bool isMaxTwo, bool isMinOne,bool isMinTwo, bool ishiRange, bool isloRange, bool isalrmFault, string searchQuery )
        {
            model.iSortingCols = 0;
            try
            {
                using (MassCodeEntities db = new MassCodeEntities())
                {
                    db.Configuration.LazyLoadingEnabled = true;
                    db.Configuration.ProxyCreationEnabled = false;
                    var context = db.Events.AsQueryable();
                    //var selected = db.Locations.FirstOrDefault(x => x.Lokasyon == areaName);
                    //string keyword = selected.Keyword;
                    //context = allArea ? context.Where(x => x.Source.Contains(keyword)) : context;
                    context = allArea ? context.Where(x => x.AreaName == areaName) : context;
                    context =allSource? context.Where(x => x.Source == sourceName):context ;

                    var datesofpoint = context.ToList();
                    if (startDate != null)
                    {
                        context = context.Where(x => x.EventDate >= startDate.Date);
                    }
                    datesofpoint = context.ToList();
                    if (endDate != null)
                    {
                        context = context.Where(x => x.EventDate <= endDate.Date);
                    }
                    datesofpoint = context.ToList();
                    //search
                    if (!(string.IsNullOrEmpty(model.sSearch) || string.IsNullOrWhiteSpace(model.sSearch)))
                    {
                        context = context.Where(x => x.Source.Contains(model.sSearch));
                    }
                    string[] searchQueryStrigns = !(string.IsNullOrEmpty(searchQuery) || string.IsNullOrWhiteSpace(searchQuery))&&searchQuery.Contains(",")?searchQuery.Split(','): searchQuery.Split('.');
                    if (!(string.IsNullOrEmpty(searchQuery)||string.IsNullOrWhiteSpace(searchQuery)))
                    {
                        if (searchQuery.Contains("."))
                        {
                            int realSize = searchQueryStrigns.Length;
                            Array.Resize(ref searchQueryStrigns, 6);
                            for (int i = realSize; i < 6; i++)
                            {
                                searchQueryStrigns[i] = searchQueryStrigns[0];
                            }
                            //queryStrings = true;
                            //andOr = true;
                            string firstStr = searchQueryStrigns[0];
                            string secondStr = searchQueryStrigns[1];
                            string thrdStr = searchQueryStrigns[2];
                            string fourthStr = searchQueryStrigns[3];
                            string fifthStr = searchQueryStrigns[4];
                            string sixthStr = searchQueryStrigns[5];
                            context = context.Where(x => x.Source.Contains(firstStr));
                            context = context.Where(x => x.Source.Contains(secondStr));
                            context = context.Where(x => x.Source.Contains(thrdStr));
                            context = context.Where(x => x.Source.Contains(fourthStr));
                            context = context.Where(x => x.Source.Contains(fifthStr));
                            context = context.Where(x => x.Source.Contains(sixthStr));
                        }
                        else if (searchQuery.Contains(","))
                        {
                            int realSize = searchQueryStrigns.Length;
                            Array.Resize(ref searchQueryStrigns, 6);
                            for (int i = realSize; i < 6; i++)
                            {
                                searchQueryStrigns[i]=searchQueryStrigns[0];
                            }
                            string firstStr = searchQueryStrigns[0];
                            string secondStr = searchQueryStrigns[1];
                            string thrdStr = searchQueryStrigns[2];
                            string fourthStr = searchQueryStrigns[3];
                            string fifthStr = searchQueryStrigns[4];
                            string sixthStr = searchQueryStrigns[5];
                            context = context.Where(x => x.Source.Contains(firstStr) || x.Source.Contains(secondStr) || x.Source.Contains(thrdStr) || x.Source.Contains(fourthStr) || x.Source.Contains(fifthStr) || x.Source.Contains(sixthStr));

                        }
                        else
                        {
                            context = context.Where(x => x.Source.Contains(searchQuery));
                            //queryStrings = false;
                            //andOr = true;
                        }
                    }
                    var list = new List<Event>();
                    var finalEventList = new List<EventTableModel>();

                    #region CreateRangeLists

                    //var data = context.OrderBy(x => x.EventDate).ToList();
                    //var distData = db.Events.Select(y => y.Source).Distinct().ToList();
                    var maxOne =context.Where(x => x.ConditionName.Contains("MAX 1")).OrderBy(y=>y.EventDate).ToList();
                    var maxTwo =context.Where(x => x.ConditionName.Contains("MAX 2")).OrderBy(y => y.EventDate).ToList();
                    var minOne =context.Where(x => x.ConditionName.Contains("MIN 1")).OrderBy(y=>y.EventDate).ToList();
                    var minTwo =context.Where(x => x.ConditionName.Contains("MIN 2")).OrderBy(y => y.EventDate).ToList();
                    var hiRange =context.Where(x => x.ConditionName.Contains("HiRange")||x.ConditionName=="Range").OrderBy(y=>y.EventDate).ToList();
                    var loRange =context.Where(x => x.ConditionName.Contains("LoRange")).OrderBy(y => y.EventDate).ToList();
                    var alarmList =context.Where(x => x.ConditionName == "Alarm" || x.ConditionName == "Return to Normal"||x.ConditionName=="Fault"||x.ConditionName=="Return to Normal (Fault)") .OrderBy(y => y.EventDate).ToList();
                    var maxOneDist = maxOne.Select(x => x.Source).Distinct().ToList();
                    var maxTwoDist = maxTwo.Select(x => x.Source).Distinct().ToList();
                    var minOneDist = minOne.Select(x => x.Source).Distinct().ToList();
                    var minTwoDist = minTwo.Select(x => x.Source).Distinct().ToList();
                    var hiRangeDist = hiRange.Select(x => x.Source).Distinct().ToList();
                    var loRangeDist = loRange.Select(x => x.Source).Distinct().ToList();
                    var alrmDist = alarmList.Select(x => x.Source).Distinct().ToList();

                    #endregion

                    #region FillRangetoList
                    bool addAlarm = true;
                    bool returnTo = false;
                    if (isMaxOne)
                    {
                        foreach (var item in maxOneDist)
                        {
                            addAlarm = true;
                            returnTo = false;
                            foreach (var newItem in maxOne)
                            {

                                if (returnTo && newItem.Source == item && newItem.ConditionName == "MAX 1 normal")
                                {
                                    list.Add(newItem);
                                    addAlarm = true;
                                    returnTo = false;
                                }
                                else if (addAlarm && newItem.Source == item && (newItem.ConditionName == "MAX 1 alarm"))
                                {
                                    list.Add(newItem);
                                    addAlarm = false;
                                    returnTo = true;
                                }
                            }
                        }
                    }
                    if (isMaxTwo)
                    {
                        foreach (var item in maxTwoDist)
                        {
                            addAlarm = true;
                            returnTo = false;
                            foreach (var newItem in maxTwo)
                            {

                                if (returnTo && newItem.Source == item && newItem.ConditionName == "MAX 2 normal")
                                {
                                    list.Add(newItem);
                                    addAlarm = true;
                                    returnTo = false;
                                }
                                else if (addAlarm && newItem.Source == item && (newItem.ConditionName == "MAX 2 alarm"))
                                {
                                    list.Add(newItem);
                                    addAlarm = false;
                                    returnTo = true;
                                }
                            }
                        }
                    }
                    if (isMinOne)
                    {
                        foreach (var item in minOneDist)
                        {
                            addAlarm = true;
                            returnTo = false;
                            foreach (var newItem in minOne)
                            {

                                if (returnTo && newItem.Source == item && newItem.ConditionName == "MIN 1 normal")
                                {
                                    list.Add(newItem);
                                    addAlarm = true;
                                    returnTo = false;
                                }
                                else if (addAlarm && newItem.Source == item && (newItem.ConditionName == "MIN 1 alarm"))
                                {
                                    list.Add(newItem);
                                    addAlarm = false;
                                    returnTo = true;
                                }
                            }
                        }
                    }
                    if (isMinTwo)
                    {
                        foreach (var item in minTwoDist)
                        {
                            addAlarm = true;
                            returnTo = false;
                            foreach (var newItem in minTwo)
                            {

                                if (returnTo && newItem.Source == item && newItem.ConditionName == "MIN 2 normal")
                                {
                                    list.Add(newItem);
                                    addAlarm = true;
                                    returnTo = false;
                                }
                                else if (addAlarm && newItem.Source == item && (newItem.ConditionName == "MIN 2 alarm"))
                                {
                                    list.Add(newItem);
                                    addAlarm = false;
                                    returnTo = true;
                                }
                            }
                        }
                    }
                    if (ishiRange)
                    {
                        foreach (var item in hiRangeDist)
                        {
                            addAlarm = true;
                            returnTo = false;
                            foreach (var newItem in hiRange)
                            {

                                if (returnTo && newItem.Source == item && newItem.ConditionName.StartsWith("Return"))
                                {
                                    list.Add(newItem);
                                    addAlarm = true;
                                    returnTo = false;
                                }
                                else if (addAlarm && newItem.Source == item && !newItem.ConditionName.StartsWith("Return"))
                                {
                                    list.Add(newItem);
                                    addAlarm = false;
                                    returnTo = true;
                                }
                            }
                        }
                    }
                    if (isloRange)
                    {
                        foreach (var item in loRangeDist)
                        {
                            addAlarm = true;
                            returnTo = false;
                            foreach (var newItem in loRange)
                            {

                                if (returnTo && newItem.Source == item && newItem.ConditionName.StartsWith("Return"))
                                {
                                    list.Add(newItem);
                                    addAlarm = true;
                                    returnTo = false;
                                }
                                else if (addAlarm && newItem.Source == item && !newItem.ConditionName.StartsWith("Return"))
                                {
                                    list.Add(newItem);
                                    addAlarm = false;
                                    returnTo = true;
                                }
                            }
                        }
                    }
                    if (isalrmFault)
                    {
                        foreach (var item in alrmDist)
                        {
                            addAlarm = true;
                            returnTo = false;
                            foreach (var newItem in alarmList)
                            {

                                if (returnTo && newItem.Source == item && newItem.ConditionName.StartsWith("Return"))
                                {
                                    list.Add(newItem);
                                    addAlarm = true;
                                    returnTo = false;
                                }
                                else if (addAlarm && newItem.Source == item && !newItem.ConditionName.StartsWith("Return"))
                                {
                                    list.Add(newItem);
                                    addAlarm = false;
                                    returnTo = true;
                                }
                            }
                        }
                    }
                    #endregion


                    #region Eskidata
                    // Bu bölümden itibaren eski sartlar gecerli
                    //foreach (var item in distData)
                    //{
                    //    addAlarm = true;
                    //    returnTo = false;
                    //    foreach (var newItem in data)
                    //    {

                    //        if (returnTo &&newItem.Source==item&& (newItem.ConditionName.StartsWith("Return")|| newItem.ConditionName.Contains("normal")))
                    //        {
                    //            list.Add(newItem);
                    //            addAlarm = true;
                    //            returnTo = false;
                    //        }
                    //        else if (addAlarm &&newItem.Source==item && (!newItem.ConditionName.StartsWith("Return")||!newItem.ConditionName.Contains("normal")))
                    //        {
                    //            list.Add(newItem);
                    //            addAlarm = false;
                    //            returnTo = true;
                    //        }
                    //    }
                    //}
                    #endregion


                    var finalDistdata = list.Select(z => z.Source).Distinct().ToList();

                    
                    foreach (var item in finalDistdata)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            bool recentSelectedItem = list[i].Source == item ? true : false;
                            if (recentSelectedItem)
                            {
                                bool finalOne = list.Count - 1 == i ? true : false;
                                var eventItem = new EventTableModel();
                                var alrm = list[i];
                                eventItem.Bolge = alrm.AreaName;
                                eventItem.Aciklama = alrm.Description;
                                eventItem.BaslamaZamani = alrm.EventDate.Value.ToString();
                                eventItem.Nokta = alrm.Source;
                                bool sameType = false; ;  
                                if (!(i == list.Count - 1))
                                {
                                    i++;
                                    sameType = alrm.ConditionName.Substring(0, 5) == list[i].ConditionName.Substring(0, 5)
                                        || alrm.ConditionName.Contains("Range")&&list[i].ConditionName.Contains("Return")
                                        || alrm.ConditionName.Contains("Alarm")&&list[i].ConditionName.Contains("Return")
                                        || alrm.ConditionName.Contains("Fault")&&list[i].ConditionName.Contains("Return")? true : false;
                                }
                                
                                bool chooseNext=false;
                                if (!(list.Count==i)&& (list[i].Source==item) && sameType)
                                {
                                    chooseNext = true;
                                }
                                 
                                Event rtrn = chooseNext ? list[i]:alrm;
                                string rtrnCond = chooseNext ? rtrn.ConditionName: "Devam Ediyor"  ;
                                string rtrnReason = chooseNext ? rtrn.Reason:"";
                                eventItem.Durum = alrm.ConditionName + "  <br/>  " +
                                    "" + rtrnCond;
                                eventItem.EndDate = rtrn.EventDate.Value;
                                eventItem.BitisZamani = chooseNext ? rtrn.EventDate.Value.ToString(): "Devam Ediyor";
                                eventItem.Sebep = alrm.Reason + " " +
                                    "" + rtrnReason;
                                var dateDiff = rtrn.EventDate - alrm.EventDate;
                                eventItem.TimeElapsed = !(chooseNext) ? "Devam Ediyor" :
                                      dateDiff.Value.Days.ToString() + " Gün <br/>"
                                    + dateDiff.Value.Hours.ToString() + " Saat <br/>"
                                    + dateDiff.Value.Minutes.ToString() + " Dakika <br/>"
                                    + dateDiff.Value.Seconds.ToString() + " Saniye";
                                finalEventList.Add(eventItem);
                                i = chooseNext ? i : i-1;
                                i = finalOne ? i + 1 : i;
                            }
                        }
                    }
                    


                    //var filterData = (from x in data
                    //                  group x by new { x.Source, x.ConditionName } into G
                    //                  select new
                    //                  {
                    //                      Source = G.Key.Source,
                    //                      ConditionName = G.Key.ConditionName,
                    //                      Condition = G.OrderBy(t => t.EventDate).FirstOrDefault()
                    //                  });

                    //toplam kayıt sayısı
                    model.iTotalDisplayRecords = finalEventList.Count();

                    var queryList = finalEventList.AsQueryable();

                    queryList = queryList.OrderBy(x => x.RecordId);

                    //paging
                    if (model.iDisplayLength > 0)
                    {
                        queryList = queryList.OrderBy(x => x.RecordId).Skip(model.iDisplayStart).Take(model.iDisplayLength);
                    }


                  

                    //select
                    model.aaData = queryList.Select(x => new EventTableModel()
                    {
                        //Bolge = db.a.FirstOrDefault(y => y.Point == x.Nokta).Lokasyon,
                        Bolge = x.Bolge,
                        Nokta = x.Nokta,
                        Aciklama = x.Aciklama,
                        Durum = x.Durum,
                        Sebep = x.Sebep,
                        BaslamaZamani=x.BaslamaZamani,
                        BitisZamani=x.BitisZamani=="Devam Ediyor"? "<span class=\"badge badge-danger\">Devam Ediyor.</span>" : x.BitisZamani,
                         StartDate = x.StartDate,
                        TimeElapsed = x.TimeElapsed=="Devam Ediyor"? "<span class=\"badge badge-danger\">Devam Ediyor.</span>":x.TimeElapsed
                        
                    }).ToArray();
                }
            }
            catch (Exception ex)
            {
                
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}