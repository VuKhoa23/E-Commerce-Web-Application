using DataAccess.Data;
using Models;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Repository.IRepository;
using System.Diagnostics;

namespace E_Commerce_Web_Application.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var categoryList = _unitOfWork.Category.GetAll();
            return View(categoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name.StartsWith(" "))
            {
                ModelState.AddModelError("name", "Category's name cannot start with spaces");
                return View();
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category was successfully created";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }

		public IActionResult Edit(int? id)
		{
            if(id == null || id <= 0){
                return NotFound();
            }
            var category  = _unitOfWork.Category.Get(c => c.Id == id);
            Trace.WriteLine(category.Name);
            if (category == null)
            {
                return NotFound();
            }
			return View(category);
		}

		[HttpPost]
		public IActionResult Edit(Category obj)
		{
            if (obj.Name.StartsWith(" "))
            {
                ModelState.AddModelError("name", "Category's name cannot start with spaces");
                return View();
            }
            if (ModelState.IsValid)
			{
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
				TempData["success"] = "Category was successfully updated";
				return RedirectToAction("Index", "Category");
			}
			return View();
		}

        public IActionResult Delete(int? id)
        {
            if (id == null || id <= 0)
            {
                return NotFound();
            }
            var category = _unitOfWork.Category.Get(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            var category = _unitOfWork.Category.Get(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Delete(category);
            _unitOfWork.Save();
			TempData["success"] = "Category was successfully deleted";
			return RedirectToAction("Index");
        }
    }
}
