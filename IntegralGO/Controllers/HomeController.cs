using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntegralGO.Controllers
{
    public class HomeController : Controller
    {
        IntegralDataEntities db = new IntegralDataEntities();
        public ActionResult Index()
        {
            //轮播图片集合
            List<Backstage_RotationChart> RotationChart = (from a in db.Backstage_RotationChart where a.state == 1 select a).ToList();
            //热门兑换集合
            var commodities = (from b in db.Backstage_Commoditie join a in db.Backstage_CommoditiesInformation on b.id equals a.Cid 
                                                      where b.state == 1 orderby b.number select new { b,a.Ctitle }).ToList();
            List<dynamic> comm = new List<dynamic>();
            foreach (var item in commodities.ToList())
            {
                dynamic dyObject = new ExpandoObject();
                dyObject.id = item.b.id;
                dyObject.commoditieName = item.b.commoditieName;
                dyObject.commoditiePath = item.b.commoditiePath;
                dyObject.useIntegral= item.b.useIntegral;
                dyObject.Ctitle = item.Ctitle;
                comm.Add(dyObject);
            }
            ViewData["chartlist"] = RotationChart;
            ViewData["commoditieslist"] = comm;
            ViewBag.chooseTitle = "积分GO";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Myself()
        {
            ViewBag.Message = "Your application description page.";
            ViewBag.chooseTitle = "个人中心";
            return View();
        }

        public ActionResult Myself_Class(string aa)
        {
            ViewBag.Message = "Your application description page.";
            ViewBag.Itemclass = aa;
            ViewBag.chooseTitle = aa;
            return View();
        }

        public ActionResult CommoditiePage(int commonid)
        {
            ViewBag.Message = "Your application description page.";
            var commodities = (from b in db.Backstage_Commoditie
                               join a in db.Backstage_CommoditiesInformation on b.id equals a.Cid
                               where a.Cid == commonid
                               select new { a, b.commoditieName,b.commoditiePath,b.useIntegral }).ToList();
            List<dynamic> comm = new List<dynamic>();
            foreach (var item in commodities.ToList())
            {
                dynamic dyObject = new ExpandoObject();
                dyObject.id = item.a.id;
                dyObject.commoditieName = item.commoditieName;
                dyObject.commoditiePath = item.commoditiePath;
                dyObject.useIntegral = item.useIntegral;
                dyObject.Ctitle = item.a.Ctitle;
                dyObject.explain = item.a.explain;
                dyObject.exchangeInstructions = item.a.exchangeInstructions;
                dyObject.price = item.a.price;
                dyObject.postage = item.a.Postage;
                dyObject.stock = item.a.stock;
                comm.Add(dyObject);
            }
            ViewData["commoditie"] = comm;
            ViewBag.chooseTitle = comm[0].commoditieName;
            return View();
        }
    }
}