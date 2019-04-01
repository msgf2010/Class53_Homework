using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Class53.Data;
using Class53_Homework.Models;

namespace Class53_Homework.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Manager mgr = new Manager(Properties.Settings.Default.ConStr);
            SimchaModelView mv = new SimchaModelView();
            mv.Simchas = mgr.GetSimchas();
            foreach (Simcha s in mv.Simchas)
            {
                s.Total = mgr.GetTotalAmountForSimcha(s.Id);
                s.ContributorCount = mgr.GetTotalContributorsForSimcha(s.Id);
            }
            mv.TotalContributors = mgr.GetAllContributors();

            return View(mv);
        }

        public ActionResult Contributions(int simchaId)
        {
            Manager mgr = new Manager(Properties.Settings.Default.ConStr);
            ContributorsForSimchaModelView mv = new ContributorsForSimchaModelView();
            mv.Contributors = mgr.GetContributors();
            foreach (Contributor c in mv.Contributors)
            {
                c.Balance = mgr.GetDeposits(c.Id) - mgr.GetContributions(c.Id);
            }

            mv.SimchaContributors = mgr.GetContributorsForSimcha(simchaId);

            foreach (SimchaContributor sc in mv.SimchaContributors)
            {
                sc.Include = mgr.DidPersonContribute(simchaId, sc.ContributorId);
                sc.Amount = mgr.GetIndividualAmountForSimcha(simchaId, sc.ContributorId);
            }

            mv.SimchaName = mgr.GetSimchaName(simchaId);
            mv.SimchaId = simchaId;

            return View(mv);
        }

        [HttpPost]
        public ActionResult AddSimcha(Simcha simcha)
        {
            Manager mgr = new Manager(Properties.Settings.Default.ConStr);
            mgr.AddSimcha(simcha);

            return Redirect("/home/index");
        }

        [HttpPost]
        public ActionResult AddContributors(List<SimchaContributor> contributors, int simchaId)
        {
            Manager mgr = new Manager(Properties.Settings.Default.ConStr);
            foreach (SimchaContributor sc in contributors)
            {
                if (sc.Include && !mgr.DidPersonContribute(simchaId,sc.ContributorId))
                {
                    mgr.AddContribution(sc.ContributorId, simchaId, sc.Amount);
                }
            }

            return Redirect("/home/index");
        }
    }
}