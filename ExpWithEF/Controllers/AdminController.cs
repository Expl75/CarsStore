using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ExpWithEF.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExpWithEF.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly CarContext context;
        private readonly UserManager<User> userManager;

        public AdminController(UserManager<User> userManager, CarContext ctx)
        {
            this.userManager = userManager;
            context = ctx;
        }

        public IActionResult Index()
        {
            return View(userManager.Users);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    RedirectToAction("Index");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "User not found");
            }
            return View("Index", userManager.Users);
        }

        [HttpGet]
        public IActionResult UserOrders(string id)
        {
            IEnumerable<Order> orders = context.Orders.Include(c => c.Car).Where(w => w.UserId == id).ToList();

            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            Order order = await context.Orders.FindAsync(id);
            if(order != null)
            {
                context.Orders.Remove(order);
                context.SaveChanges();
                return RedirectToAction("UserOrders");
            }
            else
            {
                ModelState.AddModelError("", "Order not found");
                return RedirectToAction("UserOrders");
            }
        }

        [HttpGet]
        public IActionResult EditOrder(string id)
        {
            Order order = context.Orders.Include(w => w.Car).Where(c => c.OrderId == id).First();
            if(order != null)
            {
                EditOrderModel editOrder = new EditOrderModel();
                editOrder.OrderId = order.OrderId;
                editOrder.UserId = order.UserId;
                editOrder.CarId = order.Car.Id;
                editOrder.Address = order.Address;
                editOrder.Phone = order.Phone;
                editOrder.UserName = order.UserName;
                editOrder.Cars = context.Cars.ToList();
                return View(editOrder);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditOrder(EditOrderModel order)
        {
            if(ModelState.IsValid)
            {
                Car car = await context.Cars.FindAsync(order.CarId);
                User user = await userManager.FindByIdAsync(order.UserId);
                if (car != null)
                {
                    if (user != null)
                    {
                        Order newOrder = await context.Orders.FindAsync(order.OrderId);
                        if(newOrder != null)
                        {
                            newOrder.Car = car;
                            newOrder.UserId = user.Id;
                            newOrder.UserName = order.UserName;
                            newOrder.Phone = order.Phone;
                            newOrder.Address = order.Address;

                            await context.SaveChangesAsync();
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Order not found");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "User not found");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Car not found");
                }
            }
            return View(order);
        }

        public void AddErrorsFromResult(IdentityResult result)
        {
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}
