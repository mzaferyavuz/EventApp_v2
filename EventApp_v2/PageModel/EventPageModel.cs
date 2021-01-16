using EventApp_v2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using CustomFramework;

namespace EventApplication.PageModel
{

    public class EventSelectModel
    {
        public List<string> Sources { get; set; }

        public List<string> AreaNames { get; set; }
    }
    public class EventPageModel
    {
        public List<Event> Events { get; set; }
        public List<string> Sources { get; set; }
        public List<string> AreaNames { get; set; }
        public EventPageModel[] aaData { get; set; }
    }

    public class EventTableModel:Event
    {
        public string Bolge { get; set; }

        public string Nokta { get; set; }

        public string Durum { get; set; }
        public string Aciklama { get; set; }
        public DateTime StartDate { get; set; }

        public string BitisZamani { get; set; }
        public string BaslamaZamani { get; set; }
        public DateTime EndDate { get; set; }
        public string TimeElapsed { get; set; }

        public string Sebep { get; set; }
    }
}