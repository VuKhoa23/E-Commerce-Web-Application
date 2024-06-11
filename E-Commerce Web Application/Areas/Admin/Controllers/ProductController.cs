using DataAccess.Data;
using Models;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Repository.IRepository;
using System.Diagnostics;

namespace E_Commerce_Web_Application.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var productList = _unitOfWork.Product.GetAll();
            return View(productList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product obj)
        {
            if (obj.Name.StartsWith(" "))
            {
                ModelState.AddModelError("name", "Product's name cannot start with spaces");
                return View();
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Product was successfully created";
                return RedirectToAction("Index", "Product");
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id <= 0)
            {
                return NotFound();
            }
            var product = _unitOfWork.Product.Get(c => c.Id == id);
            Trace.WriteLine(product.Name);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product obj)
        {
            if (obj.Name.StartsWith(" "))
            {
                ModelState.AddModelError("name", "Product's name cannot start with spaces");
                return View();
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Product was successfully updated";
                return RedirectToAction("Index", "Product");
            }
            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id <= 0)
            {
                return NotFound();
            }
            var product = _unitOfWork.Product.Get(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            var product = _unitOfWork.Product.Get(c => c.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            _unitOfWork.Product.Delete(product);
            _unitOfWork.Save();
            TempData["success"] = "Product was successfully deleted";
            return RedirectToAction("Index");
        }
    }
}
