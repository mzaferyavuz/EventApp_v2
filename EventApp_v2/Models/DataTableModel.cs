using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventApplication.Models
{
    public class DataTableModel<T>
    {
        public int sEcho { get; set; }
        public int iSortingCols { get; set; }
        public int iDisplayStart { get; set; }
        public int iDisplayLength { get; set; }
        public string sSearch { get; set; }
        public T[] aaData { get; set; }
        public int iTotalDisplayRecords { get; set; }
        public int iTotalRecords { get; set; }

    }
}