using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Class53.Data;
using Class53_Homework.Models;

namespace Class53_Homework.Controllers
{
    public class SubPageController : Controller
    {
        public ActionResult Index()
        {
            Manager mgr = new Manager(Properties.Settings.Default.ConStr);

            return View();
        }

        public ActionResult Contributors()
        {
            Manager mgr = new Manager(Properties.Settings.Default.ConStr);
            ContributorViewModel vm = new ContributorViewModel();
            vm.Contributors = mgr.GetContributors();
            foreach (Contributor c in vm.Contributors)
            {
                c.Balance = mgr.GetDeposits(c.Id) - mgr.GetContributions(c.Id);
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult AddContributor(Contributor contributor, decimal amount, DateTime date)
        {
            Manager mgr = new Manager(Properties.Settings.Default.ConStr);
            mgr.AddContributor(contributor);
            Deposit deposit = new Deposit();
            deposit.Amount = amount;
            deposit.Date = date;
            deposit.PersonId = contributor.Id;
            mgr.AddDeposit(deposit);

            return Redirect("/subpage/Contributors");
        }

        [HttpPost]
        public ActionResult AddDeposit(Deposit deposit)
        {
            Manager mgr = new Manager(Properties.Settings.Default.ConStr);
            mgr.AddDeposit(deposit);

            return Redirect("/subpage/Contributors");
        }

        public ActionResult History(int id)
        {
            Manager mgr = new Manager(Properties.Settings.Default.ConStr);

            return View();
        }
    }
}