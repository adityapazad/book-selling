using Ecommerce.Project._02.DataAccess.Data;
using Ecommerce.Project._02.DataAccess.Repository.IRepository;
using Ecommerce.Project._02.Models;
using Ecommerce.Project._02.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Project._02.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        public UserController(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region APIs
        [HttpGet]

        public IActionResult GetAll()
        {
            var userList = _context.ApplicationUsers.ToList();
            var roles = _context.Roles.ToList();
            var userRoles = _context.UserRoles.ToList();

            foreach (var user in userList)
            {
                var userRole = userRoles.FirstOrDefault(u => u.UserId == user.Id);
                var roleId = userRole != null ? userRole.RoleId : null;

                // Set the role name if roleId is not null
                user.Role = roleId != null ? roles.FirstOrDefault(r => r.Id == roleId)?.Name : "No Role Assigned";

                if (user.CompanyId == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
                if (user.CompanyId != null)
                {
                    user.Company = new Company()
                    {
                        Name = _unitOfWork.Company.Get(Convert.ToInt32(user.CompanyId)).Name,
                    };
                }
            }

            // REMOVE ADMIN USER
            var adminUser = userList.FirstOrDefault(u => u.Role == SD.Role_Admin);
            if (adminUser != null)
            {
                userList.Remove(adminUser);
            }

            return Json(new { data = userList });
        }


        //public IActionResult GetAll()

        //{
        //    var userList = _context.ApplicationUsers.ToList();
        //    var roles = _context.Roles.ToList();
        //    var userRoles = _context.UserRoles.ToList();

        //    foreach (var user in userList)
        //    {
        //        var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
        //        user.Role = roles.FirstOrDefault(r => r.Id == roleId).Name;

        //        if (user.CompanyId == null)
        //        {
        //            user.Company = new Company()
        //            {
        //                Name = ""
        //            };
        //        }
        //        if (user.CompanyId != null)
        //        {
        //            user.Company = new Company()
        //            {
        //                Name = _unitOfWork.Company.Get(Convert.ToInt32(user.CompanyId)).Name,
        //            };
        //        }
        //    }
        //    //REMOVE ADMIN USER
        //    var adminUser = userList.FirstOrDefault(u => u.Role == SD.Role_Admin);
        //    if (adminUser != null)
        //    {
        //        userList.Remove(adminUser);
        //    }
        //    return Json(new { data = userList });
        //}

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            bool isLocked = false;
            var userInDb = _context.ApplicationUsers.FirstOrDefault(au => au.Id == id);

            if (userInDb == null) return Json(new { success = false, message = "SOMETHING WENT WRONG !" });

            if (userInDb != null && userInDb.LockoutEnd > DateTime.Now)
            {
                userInDb.LockoutEnd = DateTime.Now;
                isLocked = false;
            }
            else
            {
                userInDb.LockoutEnd = DateTime.Now.AddYears(100);
                isLocked = true;
            }
            _context.SaveChanges();

            return Json(new { success = isLocked == true ? "User Successfully Locked" : "User Successfully Unlocked" });
        }
            #endregion
    }
}
