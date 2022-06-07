using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcSuperShop.Data;
using MvcSuperShop.Infrastructure.Context;
using MvcSuperShop.Migrations;
using MvcSuperShop.Services;

namespace SuperShopTests.Services;

[TestClass]
public class PricingServiceTests
{
    private PricingService _sut;

    [TestInitialize]
    public void Initialize()
    {
        _sut = new PricingService();
    }

    [TestMethod]
    public void When_agreement_is_not_found_product_baseprice_is_used()
    {
        
        //Arrange
        var productList = new List<ProductServiceModel>
        {
            new ProductServiceModel{BasePrice = 129342},
            new ProductServiceModel{BasePrice = 530000},
            new ProductServiceModel{BasePrice = 349238}
        };

        var customerContext = new CurrentCustomerContext
        {
            Agreements = new List<Agreement>()
        };

        //Act
        var result = _sut.CalculatePrices(productList, customerContext);

        //Assert
        Assert.AreEqual(129342, result.First().Price);

    }

    [TestMethod]
    public void When_agreement_is_found_product_price_is_used()
    {
        //Arrange
        var productList = new List<ProductServiceModel>
        {
            new ProductServiceModel{BasePrice = 530000}
        };

        var customerContext = new CurrentCustomerContext
        {
            Agreements = new List<Agreement>
            {
                new Agreement()
                {
                    AgreementRows = new List<AgreementRow>
                    {
                        new AgreementRow()
                        {
                        PercentageDiscount = 6
                        }
                    }
                }
            }
        };
        

        //Act
        var result = _sut.CalculatePrices(productList, customerContext);

        //Assert
        Assert.AreEqual(498200, result.First().Price);

    }


}