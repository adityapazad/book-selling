using Ecommerce.Project._02.DataAccess.Repository.IRepository;
using Ecommerce.Project._02.Models;
using Ecommerce.Project._02.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace Ecommerce.Project._02.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork) 
        { 
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region APIs
        public IActionResult GetAll()
        {
            return Json(new { data = _unitOfWork.Category.GetAll() });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var categoryInDb = _unitOfWork.Category.Get(id);
            if (categoryInDb == null) return Json(new { success = false, Message = "Something went wrong!" });

            _unitOfWork.Category.Remove(categoryInDb);
            _unitOfWork.Save();

            return Json(new { success = true, Message = "Data Deleted" });
        }
        #endregion

        public IActionResult Upsert(int? id)
        { 
            Category category = new Category();

            //Create
            if(id == null) return View(category);

            //Edit
            category = _unitOfWork.Category.Get(id.GetValueOrDefault());
            if(category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (category == null) return NotFound();
            if (!ModelState.IsValid) return View(category);

            if(category.id == 0)
            {
                _unitOfWork.Category.Add(category);
            }
            else
            {
                _unitOfWork.Category.Update(category);
            }

            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}
