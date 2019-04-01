using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class53.Data
{
    public class Contributor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Cell { get; set; }
        public bool AlwaysInclude { get; set; }
        public decimal Balance { get; set; }
        //public bool GivenSimcha { get; set; }
        //public bool GivenSimchaTemp { get; set; }
        //public int SimchaId { get; set; }
        //public decimal Contribution { get; set; }
    }

    public class SimchaContributor
    {
        public int ContributorId { get; set; }
        public bool Include { get; set; }
        public decimal Amount { get; set; }
    }

    //public class ContributionToSimcha
    //{
    //    public int ContributorId { get; set; }
    //    public int SimchaId { get; set; }
    //    //public DateTime ContributionDate { get; set; }
    //    public decimal Amount { get; set; }
    //} 
}
