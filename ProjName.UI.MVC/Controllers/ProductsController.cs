using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using projName.DATA.EF.Models;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;
using ProjName.UI.MVC.Utilities;


namespace ProjName.UI.MVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly StoreFrontContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(StoreFrontContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Products
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var products = _context.Products.Where(p => p.ProductStatusId > 0)

            .Include(p => p.Category).Include(p => p.OrderProducts);
            return View(await products.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ProductStatus()
        {
            var products = _context.Products.Where(p => p.ProductStatusId > 0)

            .Include(p => p.Category).Include(p => p.OrderProducts);
            return View(await products.ToListAsync());
            //to create this view, we added the action, right click -> add View and selected the List template
        }


        public async Task<IActionResult> TiledProducts(string searchTerm, int categoryId = 0)
        {
            var products = _context.Products.Where(p => p.ProductStatusId != 3)
                .Include(p => p.Category).Include(p => p.OrderProducts).ToList();


            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");

            
            #region Optional Category Filter

            if (categoryId != 0)
            {
                products = products.Where(p => p.CategoryId == categoryId).ToList();                
                ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", categoryId);
            }


            if (!String.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p =>
                                    p.ProductName.ToLower().Contains(searchTerm.ToLower())                                    
                                    || p.ProductDescription.ToLower().Contains(searchTerm.ToLower())
                                    || p.Category.CategoryName.ToLower().Contains(searchTerm.ToLower())).ToList();

                ViewBag.SearchTerm = searchTerm;
                ViewBag.NbrResults = products.Count;
            }
            else
            {
                ViewBag.SearchTerm = null;
                ViewBag.NbrResults = null;
            }

            #endregion

            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductStatus)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["ProductStatusId"] = new SelectList(_context.ProductStatuses, "ProductStatusId", "ProductStatusName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ProductPrice,QtyPerUnit,ProductDescription,ProductStatusId,CategoryId,ProductImage,Image")] Product product)
        {
            if (ModelState.IsValid)
            {

                #region File Upload - CREATE
                
                if (product.Image != null)
                {
                    
                    string ext = Path.GetExtension(product.Image.FileName);

                    
                    string[] validExts = { ".jpeg", ".jpg", ".gif", ".png" };
                    
                    if (validExts.Contains(ext.ToLower()) && product.Image.Length < 4_194_303) 
                    {                        
                        product.ProductImage = Guid.NewGuid() + ext;
                        
                        string webRootPath = _webHostEnvironment.WebRootPath;
                        
                        string fullImagePath = webRootPath + "/assets/img/";

                        
                        using (var memoryStream = new MemoryStream())
                        {
                            await product.Image.CopyToAsync(memoryStream);
                            using (var img = Image.FromStream(memoryStream))
                            {                                
                                int maxImageSize = 500;
                                int maxThumbSize = 100;

                                ImageUtility.ResizeImage(fullImagePath, product.ProductImage, img, maxImageSize, maxThumbSize);
                                

                            }
                        }
                    }
                }
                else
                {                   
                    product.ProductImage = "noimage.png";
                }

                #endregion

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);            
            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["ProductStatusId"] = new SelectList(_context.ProductStatuses, "ProductStatusId", "ProductStatusName", product.ProductStatusId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,QtyPerUnit,ProductStatusId,ProductPrice,ProductDescription,CategoryId,ProductImage,Image")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                #region EDIT File Upload
                
                string oldImageName = product.ProductImage;

                
                if (product.Image != null)
                {
                    
                    string ext = Path.GetExtension(product.Image.FileName);

                    
                    string[] validExts = { ".jpeg", ".jpg", ".png", ".gif" };

                    
                    if (validExts.Contains(ext.ToLower()) && product.Image.Length < 4_194_303)
                    {                        
                        product.ProductImage = Guid.NewGuid() + ext;
                        
                        string webRootPath = _webHostEnvironment.WebRootPath;
                        string fullPath = webRootPath + "/assets/img/";

                        
                        if (oldImageName != "noimage.png")
                        {
                            ImageUtility.Delete(fullPath, oldImageName);
                        }
                        
                        using (var memoryStream = new MemoryStream())
                        {
                            await product.Image.CopyToAsync(memoryStream);
                            using (var img = Image.FromStream(memoryStream))
                            {
                                int maxImageSize = 500;
                                int maxThumbSize = 100;
                                ImageUtility.ResizeImage(fullPath, product.ProductImage, img, maxImageSize, maxThumbSize);
                            }
                        }

                    }
                }
                #endregion

                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);            
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductStatus)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'StoreFrontContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
