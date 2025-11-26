using GymProje.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace GymProje.Data
{
    public static class DbInitializer
    {
        public static async Task Seed(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // 1. Önce Roller Var mı? Yoksa Oluştur.
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("Member"))
            {
                await roleManager.CreateAsync(new IdentityRole("Member"));
            }

            // 2. Admin Kullanıcısı Var mı?
            var adminUser = await userManager.FindByEmailAsync("B231210089@sakarya.edu.tr");

            if (adminUser == null)
            {
                // Yoksa Oluşturuyoruz
                var newAdmin = new AppUser
                {
                    UserName = "B231210089@sakarya.edu.tr",
                    Email = "B231210089@sakarya.edu.tr",
                    FirstName = "System",
                    LastName = "Admin",
                    EmailConfirmed = true,
                    BirthDate = DateTime.Now.AddYears(-25)
                };

                // Şifre: sau
                var result = await userManager.CreateAsync(newAdmin, "sau");

                if (result.Succeeded)
                {
                    // Kullanıcı oluştuysa ona "Admin" rolünü ver.
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }
    }
}