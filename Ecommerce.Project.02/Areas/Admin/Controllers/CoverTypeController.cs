using Ecommerce.Project._02.DataAccess.Repository.IRepository;
using Ecommerce.Project._02.Models;
using Ecommerce.Project._02.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Project._02.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CoverTypeController(IUnitOfWork unitOfWork)
        { 
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region APIs

        public IActionResult GetAll(int id)
        {
            return Json(new { data = _unitOfWork.CoverType.GetAll() });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var coverTypeInDb = _unitOfWork.CoverType.Get(id);
            if(coverTypeInDb == null) return Json(new { success = false, message = "Something Went Wrong" });

            _unitOfWork.CoverType.Remove(coverTypeInDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Data Delete" });
        }

        #endregion

        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();

            //Create
            if (id == null) return View(coverType);

            //Edit
            coverType = _unitOfWork.CoverType.Get(id.GetValueOrDefault());
            if (id == null) return NotFound();
            return View(coverType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Upsert(CoverType coverType)
        {
            if (coverType == null) return NotFound();
            if (!ModelState.IsValid) return View(coverType);

            if(coverType.id == 0)
            {
                _unitOfWork.CoverType.Add(coverType);
            }
            else
            {
                _unitOfWork.CoverType.Update(coverType);
            }

            _unitOfWork.Save();
            return RedirectToAction("Index");
        }


    }
}
