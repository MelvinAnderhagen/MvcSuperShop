using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MvcSuperShop.Controllers;
using MvcSuperShop.Data;
using MvcSuperShop.Infrastructure.Context;
using MvcSuperShop.Services;
using MvcSuperShop.ViewModels;

namespace SuperShopTests.Controllers;

[TestClass]
public class HomeControllerTests
{
    private HomeController _sut;
    private Mock<IProductService> _productService;
    private Mock<ICategoryService> _categoryService;
    private Mock<IMapper> _mapper;
    private ApplicationDbContext _context;

    [TestInitialize]
    public void Initialize()
    {
        _productService = new Mock<IProductService>();
        _categoryService = new Mock<ICategoryService>();
        _mapper = new Mock<IMapper>();

        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(contextOptions);
        _context.Database.EnsureCreated();

        _sut = new HomeController(_categoryService.Object, _productService.Object, _mapper.Object, _context);
    }

    // 1 = Index should return correct view assert.isnull viewname

    // 2 = verifiera att new products blir satt - det ska bli 10 stycken

    // 3 = överkurs verifiera att new products blir anropad med korrekta värden i customercontext 

    [TestMethod]
    public void Index_Should_Return_Correct_View_IsNull_ViewName()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Email, "gunnar@somecompany.com")
        }, "TestAuthentication"));

        _sut.ControllerContext = new ControllerContext();
        _sut.ControllerContext.HttpContext = new DefaultHttpContext
        {
            User = user
        };

        var result = _sut.Index() as ViewResult;

        Assert.IsNull(result.ViewName);
    }

    [TestMethod]
    public void NewProduct_Should_Return_10_Products()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Email, "gunnar@somecompany.com")
        }, "TestAuthentication"));

        _sut.ControllerContext = new ControllerContext();
        _sut.ControllerContext.HttpContext = new DefaultHttpContext
        {
            User = user
        };

        var customer = new CurrentCustomerContext
        {
            UserId = Guid.NewGuid(),
            Email = "asdasd@asdas.com"
        };

        _productService.Setup(e => e.GetNewProducts(10, customer))
            .Returns(new List<ProductServiceModel>
            {
                new ProductServiceModel(),
                new ProductServiceModel(),
                new ProductServiceModel(),
                new ProductServiceModel(),
                new ProductServiceModel(),
                new ProductServiceModel(),
                new ProductServiceModel(),
                new ProductServiceModel(),
                new ProductServiceModel(),
                new ProductServiceModel()
            });

        _mapper.Setup(e => e.Map<List<ProductBoxViewModel>>(It.IsAny<IEnumerable<ProductServiceModel>>()))
            .Returns(new List<ProductBoxViewModel>
            {
                new ProductBoxViewModel(),
                new ProductBoxViewModel(),
                new ProductBoxViewModel(),
                new ProductBoxViewModel(),
                new ProductBoxViewModel(),
                new ProductBoxViewModel(),
                new ProductBoxViewModel(),
                new ProductBoxViewModel(),
                new ProductBoxViewModel(),
                new ProductBoxViewModel()
            });

        //Act
        var result = _sut.Index() as ViewResult;

        var model = result.Model as HomeIndexViewModel;

        //Assert
        Assert.AreEqual(10, model.NewProducts.Count);
    }

    [TestMethod]
    public void Index_Should_Show_3_Categories()
    {
        //Arrange

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Email, "gunnar@somecompany.com")
        }, "TestAuthentication"));

        _sut.ControllerContext = new ControllerContext();
        _sut.ControllerContext.HttpContext = new DefaultHttpContext
        {
            User = user
        };

        _categoryService.Setup(e => e.GetTrendingCategories(3)).Returns(new List<Category>
        {
            new Category(),
            new Category(),
            new Category()
        });

        _mapper.Setup(e => e.Map<List<CategoryViewModel>>(It.IsAny<List<Category>>()))
            .Returns(new List<CategoryViewModel>
        {
            new CategoryViewModel(),
            new CategoryViewModel(),
            new CategoryViewModel()
        });

        //Act
        var result = _sut.Index() as ViewResult;

        var model = result.Model as HomeIndexViewModel;

        //Assert
        Assert.AreEqual(3, model.TrendingCategories.Count);
        

    }
}