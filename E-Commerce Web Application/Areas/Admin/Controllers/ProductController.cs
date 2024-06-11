using DataAccess.Data;
using Models;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Repository.IRepository;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.ViewModels;

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
            IEnumerable<SelectListItem> categoryList = _unitOfWork.Category.GetAll().Select(c =>
            {
                return new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                };
            });

            ProductViewModel productVM = new()
            {
                CategoryList = categoryList,
                Product = new Product()
            };

            return View(productVM);
        }

        [HttpPost]
        public IActionResult Create(ProductViewModel productVM)
        {
            if (productVM.Product.Name.StartsWith(" "))
            {
                ModelState.AddModelError("name", "Product's name cannot start with spaces");
                return View();
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(productVM.Product);
                _unitOfWork.Save();
                TempData["success"] = "Product was successfully created";
                return RedirectToAction("Index", "Product");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(c =>
                {
                    return new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString(),
                    };
                });
                return View(productVM);
            }
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
