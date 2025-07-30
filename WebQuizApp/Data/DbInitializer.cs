// Data/DbInitializer.cs
using Microsoft.AspNetCore.Identity;
using System;
using WebQuizApp.Data;
using WebQuizApp.Models;

public static class DbInitializer
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {

        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<ApplicationDbContext>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        
        // Ensure database is created
        context.Database.EnsureCreated();


        // Seed roles
        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Seed admin user
        var adminEmail = "admin@testapp.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "User"
            };
            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }


        if (!context.Tests.Any())
        {
            var tests = new Test[]
            {
                new Test
                {
                    Id = 1,
                    Name = "C# Basics",
                    Description = "Test your knowledge of basic C# concepts",
                    PassingScore = 70,
                    Questions = new List<Question>
                    {
                        new Question
                        {
                            Content = "What will be the output of this code?",
                            CodeSnippet = "int x = 5;\nint y = 2;\nConsole.WriteLine(x / y);",
                            CorrectAnswer = "2"
                        },
                        new Question
                        {
                            Content = "Which keyword is used to create a class that cannot be inherited?",
                            CodeSnippet = "",
                            CorrectAnswer = "sealed"
                        }
                    }
                },
                new Test
                {
                    Id = 2,
                    Name = "JavaScript Fundamentals",
                    Description = "Test your JavaScript knowledge",
                    PassingScore = 75,
                    Questions = new List<Question>
                    {
                        new Question
                        {
                            Content = "What is the result of '5' + 3 in JavaScript?",
                            CodeSnippet = "",
                            CorrectAnswer = "53"
                        },
                        new Question
                        {
                            Content = "What does the 'this' keyword refer to in a method?",
                            CodeSnippet = "",
                            CorrectAnswer = "The object that owns the method"
                        }
                    }
                }
            };

            context.Tests.AddRange(tests);
            context.SaveChanges();
        }
    }
}