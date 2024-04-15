using System;
using System.Collections.Generic;
using System.Text;

namespace BBK.SaaS.Mdls.Profile.Reports.Dto
{
    public class ReportDto
    {
        public class ReportList
        {
            public virtual string Date { get; set; }

            public virtual int CountRecruiment { get; set; }
            public virtual int CountCandidate { get; set; }
        }


        public class ReportListArticle
        {
            public virtual string Cat { get; set; }

            public virtual int CountArticle { get; set; }
        }

        public class ReportListCat
        {
            public virtual string Cat { get; set; }
            public virtual string Date { get; set; }
            public virtual int CountRecruiment { get; set; }
            public virtual int CountJob { get; set;}
        }

        public class ReportArray
        {
            public virtual List<ReportList> ListReport { get; set; }
            public virtual List<ReportListArticle> ReportListArticle { get; set; }
            public virtual List<ReportListCat> ReportListCat { get; set; }
            public ReportArray()
            {
                ListReport = new List<ReportList>();
                ReportListArticle = new List<ReportListArticle>();
                ReportListCat = new List<ReportListCat>();
            }
        }
    }
}
