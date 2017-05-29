using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _1361071.Models;

namespace _1361071.Controllers
{
    public class ProductController : Controller
    {
        //
        // GET: /Product/ByCat
        public ActionResult ByCat(int? id, int page = 1)
        {
            if(id.HasValue == false)
            {
                return RedirectToAction("Index","Home");
            }
            
            using (var ctx = new QLBHEntities())
            {

                int n = ctx.Products
                    .Where(p => p.CatID == id)
                    .Count();

                int recordPerPage = 4;
                int nPages = n / recordPerPage;
                int m = n % recordPerPage;
                if (m > 0)
                {
                    nPages++;
                }
                ViewBag.Pages = nPages;

                ViewBag.CurPage = page;

                //var list = ctx.Products
                //    .Where(p => p.CatID == id)
                //    .ToList();

                var list = ctx.Products
                    .Where(p => p.CatID == id)
                    .OrderBy(p => p.ProID)
                    .Skip((page - 1) * recordPerPage)
                    .Take(recordPerPage)
                    .ToList();

                return View(list);
            }
        }

        //
        // GET: /Product/Detail
        public ActionResult Detail(int? id)
        {
            if (id.HasValue == false)
            {
                return RedirectToAction("Index", "Home");
            }

            using (var ctx = new QLBHEntities())
            {
                var model = ctx.Products
                    .Where(p => p.ProID == id)
                    .FirstOrDefault();

                return View(model);
            }
        }
	}
}