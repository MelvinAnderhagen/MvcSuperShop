using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcSuperShop.Data;
using MvcSuperShop.Services;
using MvcSuperShop.ViewModels;

namespace SuperShopTests.Services;

[TestClass]
public class CategoryServiceTests
{
    private CategoryService _categoryService;
    private ApplicationDbContext _context;

    [TestInitialize]
    public void Initialize()
    {
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(contextOptions);
        _context.Database.EnsureCreated();

        _categoryService = new CategoryService(_context);
    }

    [TestMethod]
    public void When_Getting_Trends_Is_Runned_It_Should_Return_An_Int()
    {
        //Arrange
        var categoryList = new List<Category>
        {
            new Category
            {
                Name = "Van",
                Icon = "test"
            },
            new Category
            {
                Name = "Coupe",
                Icon = "test"
            },
            new Category
            {
                Name = "Hybrid",
                Icon = "test"
            }
        };

        _context.AddRange(categoryList);
        _context.SaveChanges();

        //Act
        var result = _categoryService.GetTrendingCategories(3);

        //Assert
        Assert.AreEqual(3, result.Count());
        
        
    }
}