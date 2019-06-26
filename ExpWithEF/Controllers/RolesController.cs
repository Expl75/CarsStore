using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ExpWithEF.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExpWithEF.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        UserManager<User> userManager;
        RoleManager<IdentityRole> roleManager;

        public RolesController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public IActionResult Index() => View(roleManager.Roles.ToList());

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create([Required]string name)
        {
            if(ModelState.IsValid)
            {
                var result = await roleManager.CreateAsync(new IdentityRole(name));
                if(result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddError(result);
                }
            }
            return View(name);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string Id)
        {
            var role = await roleManager.FindByIdAsync(Id);
            if(role != null)
            {
                var result = await roleManager.DeleteAsync(role);
                if(result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddError(result);
                }
            }
            ModelState.AddModelError("", "Role not found");
            return View("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if(user != null)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var allRoles = roleManager.Roles.ToList();
                ChangeRoleModel model = new ChangeRoleModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    UserRoles = userRoles.ToList(),
                    AllRoles = allRoles
                };
                return View(model);
            }
            ModelState.AddModelError("", "User not found");
            return View("Index", "Admin");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, List<string> roles)
        {
            User user = await userManager.FindByIdAsync(id);
            if(user != null)
            {
                var userRoles = await userManager.GetRolesAsync(user);

                var addedRoles = roles.Except(userRoles);

                var removedRoles = userRoles.Except(roles);

                await userManager.AddToRolesAsync(user, addedRoles);

                await userManager.RemoveFromRolesAsync(user, removedRoles);

                return RedirectToAction("Index", "Admin");
            }
            ModelState.AddModelError("", "User not found");
            return View();
        }

        public void AddError(IdentityResult result)
        {
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}
