using DataAccess.Data;
using Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Web_Application.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var categoryList = _db.Categories.ToList();
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
                _db.Categories.Add(obj);
                _db.SaveChanges();
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
            var category  = _db.Categories.FirstOrDefault(c => c.Id == id);
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
				_db.Categories.Update(obj);
				_db.SaveChanges();
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
            var category = _db.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            var category = _db.Categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(category);
            _db.SaveChanges();
			TempData["success"] = "Category was successfully deleted";
			return RedirectToAction("Index");
        }
    }
}
