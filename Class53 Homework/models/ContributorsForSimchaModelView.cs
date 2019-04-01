using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Class53.Data;

namespace Class53_Homework.Models
{
    public class ContributorsForSimchaModelView
    {
        public IEnumerable<Contributor> Contributors { get; set; }
        public IEnumerable<SimchaContributor> SimchaContributors { get; set; }
        public string SimchaName { get; set; }
        public int SimchaId { get; set; }
    }
}