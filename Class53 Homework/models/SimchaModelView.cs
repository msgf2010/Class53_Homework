using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Class53.Data;

namespace Class53_Homework.Models
{
    public class SimchaModelView
    {
        public IEnumerable<Simcha> Simchas { get; set; }
        public int TotalContributors { get; set; }
    }
}