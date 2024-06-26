﻿using DataAccess.Data;
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
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var productList = _unitOfWork.Product.GetAll("Category");
            return View(productList);
        }

        public IActionResult Upsert(int? id) // update and insert
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

            // insert
            if (id == null || id == 0)
            {
                return View(productVM);
            }
            // update
            var product = _unitOfWork.Product.Get(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            productVM.Product = product;
            return View(productVM);
        }

        [HttpPost]
        public IActionResult Upsert(ProductViewModel productVM, IFormFile? file)
        {
            if (productVM.Product.Name.StartsWith(" "))
            {
                ModelState.AddModelError("Product.Name", "Product's name cannot start with spaces");
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
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\products");
                    
                    if (!string.IsNullOrEmpty(productVM.Product.ImgUrl))
                    {
                        var oldImgPath = Path.Combine(wwwRootPath, productVM.Product.ImgUrl.TrimStart('\\'));

                        if(System.IO.File.Exists(oldImgPath))
                        {
                            System.IO.File.Delete(oldImgPath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImgUrl = @"\images\products\" + fileName;
                }
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
					TempData["success"] = "Product was successfully created";
				}
				else
                {
                    _unitOfWork.Product.Update(productVM.Product);
					TempData["success"] = "Product was successfully updated";
				}
				_unitOfWork.Save();
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

        #region REST API
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll("Category");
            return Json(new
            {
                data = productList
            });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var toBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (toBeDeleted == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Not found"
                });
            }

            if (!string.IsNullOrEmpty(toBeDeleted.ImgUrl))
            {
                var oldImgPath = Path.Combine(_webHostEnvironment.WebRootPath, toBeDeleted.ImgUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImgPath))
                {
                    System.IO.File.Delete(oldImgPath);
                }
            }
            _unitOfWork.Product.Delete(toBeDeleted);
            _unitOfWork.Save();
            return Json(new
            {
                success = true,
                message = "Deleted product"
            });
        }
        #endregion
    }
}
