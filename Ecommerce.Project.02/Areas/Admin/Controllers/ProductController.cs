using Ecommerce.Project._02.DataAccess.Repository;
using Ecommerce.Project._02.DataAccess.Repository.IRepository;
using Ecommerce.Project._02.Models;
using Ecommerce.Project._02.Models.ViewModels;
using Ecommerce.Project._02.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ecommerce.Project._02.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnviroment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnviroment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region APIs

        public IActionResult GetAll(int id)
        {
            return Json(new { data = _unitOfWork.Product.GetAll() });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var productInDb = _unitOfWork.Product.Get(id);
            if (productInDb == null) return Json(new { success = false, message = "Something went wrongs!" });

            //Image Delete
            var webRootPath = _webHostEnviroment.WebRootPath;
            var imagePath = Path.Combine(webRootPath, productInDb.ImageUrl.Trim('/', '\\'));

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            _unitOfWork.Product.Remove(productInDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Data Deleted !" });
        }

        #endregion

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM
            {
                Product = new Product(),

                CategoryList = _unitOfWork.Category.GetAll().Select(cl => new SelectListItem()
                {
                    Text = cl.Name,
                    Value = cl.id.ToString()
                }),

                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(cl => new SelectListItem()
                {
                    Text = cl.Name,
                    Value = cl.id.ToString()
                }),
            };

            if (id == null) return View(productVM);

            productVM.Product = _unitOfWork.Product.Get(id.GetValueOrDefault());
            if (productVM.Product == null) return NotFound();

            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var webRootPath = _webHostEnviroment.WebRootPath;
                var files = HttpContext.Request.Form.Files;


                if (files.Count() > 0)
                {
                    var fileName = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(files[0].FileName);
                    var uploads = Path.Combine(webRootPath, @"images/products");

                    if (productVM.Product.Id != 0)
                    {
                        var imageExists = _unitOfWork.Product.Get(productVM.Product.Id).ImageUrl;
                        productVM.Product.ImageUrl = imageExists;

                    }

                    if (productVM.Product.ImageUrl != null)
                    {
                        var imagePath = Path.Combine(webRootPath, productVM.Product.ImageUrl.Trim
                            ('/', '\\'));

                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"images/products/" + fileName + extension;
                }
                else
                {
                    if (productVM.Product.Id != 0)
                    {
                        var imagePath = _unitOfWork.Product.Get(
                            productVM.Product.Id).ImageUrl;
                        productVM.Product.ImageUrl = imagePath;
                    }
                }
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }
                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                productVM = new ProductVM()
                {
                    Product = new Product(),

                    CategoryList = _unitOfWork.Category.GetAll().Select(cl => new SelectListItem()
                    {
                        Text = cl.Name,
                        Value = cl.id.ToString(),
                    }),

                    CoverTypeList = _unitOfWork.CoverType.GetAll().Select(cl => new SelectListItem()
                    {
                        Text = cl.Name,
                        Value = cl.id.ToString(),
                    })
                };
                if (productVM.Product.Id != 0)
                {
                    productVM.Product = _unitOfWork.Product.Get(productVM.Product.Id);
                }
                return View(productVM);
            }
        }
    }
}
