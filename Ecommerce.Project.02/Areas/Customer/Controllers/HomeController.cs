using Ecommerce.Project._02.DataAccess.Repository.IRepository;
using Ecommerce.Project._02.Models;
using Ecommerce.Project._02.Models.ViewModels;
using Ecommerce.Project._02.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using System.Security.Claims;

namespace Ecommerce.Project._02.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity) User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim != null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll
                    (sc => sc.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);
            }
            var productList = _unitOfWork.Product.GetAll
                (includeProperties: "Category,CoverType");
            return View(productList);
        }

        public IActionResult Detail(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim != null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);
            }
            //***
            var productInDb = _unitOfWork.Product.FirstOrDefault(p => p.Id == id,
                includeProperties: "Category,CoverType");
            if (productInDb == null) return NotFound();
            var shoppingCart = new ShoppingCart()
            {
                Product = productInDb,
                ProductId = productInDb.Id
            };
            return View(shoppingCart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]

        public IActionResult Detail(ShoppingCart shoppingCart)
        {
            shoppingCart.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claims == null) return NotFound();
                shoppingCart.ApplicationUserId = claims.Value;

                var shoppingCartInDb = _unitOfWork.ShoppingCart.FirstOrDefault(sc => sc.ApplicationUserId == claims.Value && sc.ProductId == shoppingCart.ProductId);

                if (shoppingCartInDb == null)
                    _unitOfWork.ShoppingCart.Add(shoppingCart);
                else
                    shoppingCartInDb.Count += shoppingCart.Count;

                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var productInDb = _unitOfWork.Product.FirstOrDefault(p => p.Id == shoppingCart.ProductId, includeProperties: "Category,CoverType");
                if(productInDb == null) return NotFound();

                var shoppingCartEdit = new ShoppingCart()
                {
                    Product = productInDb,
                    ProductId = productInDb.Id
                };
                return View(shoppingCartEdit);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
