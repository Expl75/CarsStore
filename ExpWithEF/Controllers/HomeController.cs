using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ExpWithEF.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace ExpWithEF.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly CarContext context;

        public HomeController(CarContext ctx)
        {
            context = ctx;
        }

        [AllowAnonymous]
        public IActionResult Index() => View(context.Cars.ToList());


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ViewResult Create() => View();

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Car car)
        {
            if (ModelState.IsValid)
            {
                await context.AddAsync(car);
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(car);
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Buy(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            Car car = await context.Cars.FindAsync(id);
            if(car != null)
            {
                ViewBag.UserId = userId;
                ViewBag.CarId = car.Id;
                return View();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Buy(Order order, string CarId)
        {
            Order newOrder = new Order();
            Car car = await context.Cars.FindAsync(CarId);
            if (order != null && car != null)
            {
                newOrder.UserId = order.UserId;
                newOrder.UserName = order.UserName;
                newOrder.Phone = order.Phone;
                newOrder.Address = order.Address;
                newOrder.Car = car;

                if (ModelState.IsValid)
                {
                    await context.AddAsync(newOrder);
                    await context.SaveChangesAsync();
                    return View("SuccesfulOrder");
                }
            }
            return View(order);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            Car car = await context.Cars.FindAsync(id);
            Order order = context.Orders.Include(c => c.Car).Where(p => p.Car.Id == id).FirstOrDefault();
            if(car != null)
            {
                if (order == null)
                {
                    context.Cars.Remove(car);
                    context.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "This car is included in the order!");
                    RedirectToAction("Index");
                }
            }
            return View("Index", context.Cars);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            Car car = await context.Cars.FindAsync(id);
            if (car != null)
            {
                return View(car);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Car car)
        {
            if(ModelState.IsValid)
            {
                Car editCar = await context.Cars.FindAsync(car.Id);
                if(editCar != null)
                {
                    editCar.company = car.company;
                    editCar.model = car.model;
                    editCar.color = car.color;
                    editCar.price = car.price;

                    context.SaveChanges();
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Car not found");
            }
            return View(car);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AllOrders() => View(context.Orders.Include(c => c.Car).ToList());
    }
}
