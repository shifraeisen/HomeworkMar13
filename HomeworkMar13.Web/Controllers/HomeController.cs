using HomeworkMar13.Data;
using HomeworkMar13.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HomeworkMar13.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _conStr = @"Data Source=.\sqlexpress;Initial Catalog=GiveAndTake; Integrated Security=true;";
        public IActionResult Index()
        {
            var repo = new SimpleAdRepository(_conStr);
            var userRepo = new UserRepository(_conStr);
            var vm = new IndexViewModel
            {
                Ads = repo.GetAllAds()
            };
            if(User.Identity.IsAuthenticated)
            {
                var user = userRepo.GetByEmail(User.Identity.Name);
                vm.CurrentUserID = user.Id;
            }
            return View(vm);
        }
        [Authorize]
        public IActionResult NewAd()
        {
            return View();
        }
        [HttpPost]
        public IActionResult NewAd(SimpleAd a)
        {
            var repo = new UserRepository(_conStr);
            var user = repo.GetByEmail(User.Identity.Name);

            var adRepo = new SimpleAdRepository(_conStr);
            a.Name = user.Name;
            adRepo.NewAd(a, user.Id);
            return RedirectToAction("Index");
        }
        [Authorize]
        public IActionResult MyAccount()
        {
            var repo = new UserRepository(_conStr);
            var user = repo.GetByEmail(User.Identity.Name);

            var adRepo = new SimpleAdRepository(_conStr);
            var list = adRepo.GetAllAds();
            list.RemoveAll(a => a.UserID != user.Id);
            return View(new IndexViewModel
            {
                Ads = list
            });
        }
        [HttpPost]
        public IActionResult DeleteAd(int id)
        {
            var repo = new UserRepository(_conStr);
            var adRepo = new SimpleAdRepository(_conStr);
            if(repo.GetByEmail(User.Identity.Name).Id == adRepo.GetByID(id).UserID)
            {
                adRepo.DeleteAd(id);
            }
            return RedirectToAction("Index");
        }
    }
}