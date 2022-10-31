﻿
using Microsoft.AspNetCore.Mvc;
using projName.DATA.EF.Models;
using Microsoft.AspNetCore.Identity;
using ProjName.UI.MVC.Models;
using Newtonsoft.Json;



namespace ProjName.UI.MVC.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly StoreFrontContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        
        public ShoppingCartController(StoreFrontContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {            
            var sessionCart = HttpContext.Session.GetString("cart");
            
            Dictionary<int, CartItemViewModel> shoppingCart = null;

            
            if (sessionCart == null || sessionCart.Count() == 0)
            {
                ViewBag.Message = "There are no items in your cart";

                shoppingCart = new Dictionary<int, CartItemViewModel>();
            }
            else
            {
                ViewBag.Message = null;

                shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);
            }            
            return View(shoppingCart);
        }


        public IActionResult AddToCart(int id)
        {            
            Dictionary<int, CartItemViewModel> shoppingCart = null;
            
            var sessionCart = HttpContext.Session.GetString("cart");
            
            if (sessionCart == null)
            {
                shoppingCart = new Dictionary<int, CartItemViewModel>();
            }            
            else
            {
                shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);
                
            }
            
            Product product = _context.Products.Find(id);
            
            CartItemViewModel civm = new CartItemViewModel(1, product);

            
            if (shoppingCart.ContainsKey(product.ProductId))
            {
                
                shoppingCart[product.ProductId].Qty++;
            }
            else
            {
                shoppingCart.Add(product.ProductId, civm);
            }

            
            string jsonCart = JsonConvert.SerializeObject(shoppingCart);
            HttpContext.Session.SetString("cart", jsonCart);

            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int id)
        {            
            var sessionCart = HttpContext.Session.GetString("cart");
            
            Dictionary<int, CartItemViewModel> shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);
            
            shoppingCart.Remove(id);
            
            if (shoppingCart.Count == 0)
            {
                HttpContext.Session.Remove("cart");
            }            
            else
            {
                string jsonCart = JsonConvert.SerializeObject(shoppingCart);
                HttpContext.Session.SetString("cart", jsonCart);
            }
            
            return RedirectToAction("Index");
        }

        public IActionResult UpdateCart(int productId, int qty)
        {            
            var sessionCart = HttpContext.Session.GetString("cart");
            
            Dictionary<int, CartItemViewModel> shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);
            
            shoppingCart[productId].Qty = qty;
            
            string jsonCart = JsonConvert.SerializeObject(shoppingCart);
            HttpContext.Session.SetString("cart", jsonCart);

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> SubmitOrder()
        {            
            string? userId = (await _userManager.GetUserAsync(HttpContext.User))?.Id;
            
            UserDetail ud = _context.UserDetails.Find(userId);
            
            Order o = new Order()
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                ShipToName = ud.FirstName,
                ShipCity = ud.City,
                ShipState = ud.State,
                ShipZip = ud.Zip
            };
            
            _context.Orders.Add(o);


            
            var sessionCart = HttpContext.Session.GetString("cart");
            Dictionary<int, CartItemViewModel> shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);
            
            foreach (var item in shoppingCart)
            {
                OrderProduct op = new OrderProduct()
                {
                    OrderId = o.OrderId,
                    ProductId = item.Value.CartProd.ProductId,
                    ProductPrice = item.Value.CartProd.ProductPrice,
                    Quantity = (short?)item.Value.Qty
                };                
                o.OrderProducts.Add(op);

            }

            
            _context.SaveChanges();
            
            HttpContext.Session.Remove("cart");

            return RedirectToAction("Index", "Orders");
        }
    }
}