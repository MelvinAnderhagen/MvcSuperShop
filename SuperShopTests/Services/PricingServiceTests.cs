using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcSuperShop.Data;
using MvcSuperShop.Infrastructure.Context;
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
            new ProductServiceModel{BasePrice = 180000}
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
        Assert.AreEqual(169200, result.First().Price);

    }

    [TestMethod]
    public void If_Customer_Has_Two_Or_More_Agreements_Should_Return_Best_Discount()
    {

    }

    [TestMethod]
    public void When_Customer_Is_Not_Found_Should_Return_No_Agreement()
    {
        //Arrange
        var productList = new List<ProductServiceModel>
        {
            new ProductServiceModel{BasePrice = 180000}
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
                            PercentageDiscount = 8
                        },
                        new AgreementRow()
                        {
                            PercentageDiscount = 12
                        }
                    }
                }
            }
        };


        //Act
        var result = _sut.CalculatePrices(productList, customerContext);

        //Assert
        Assert.AreEqual(158400, result.First().Price);

    }

}