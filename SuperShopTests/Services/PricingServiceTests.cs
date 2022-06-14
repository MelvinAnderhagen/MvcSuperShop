using System;
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
            new ProductServiceModel
            {
                BasePrice = 180000, 
                Name = "XC60", 
                ManufacturerName = "Volvo", 
                CategoryName = "SUV"
            }
        };

        var customerContext = new CurrentCustomerContext
        {
            Agreements = new List<Agreement>
            {
                new Agreement()
                {
                    ValidFrom = new DateTime(2019,09,25),
                    ValidTo = new DateTime(2024,09,02),

                    AgreementRows = new List<AgreementRow>
                    {
                        new AgreementRow()
                        {
                            PercentageDiscount = 6,
                            ManufacturerMatch = "volvo"
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
    public void When_Valid_To_Is_Lower_Than_DateTime_Now_Should_Return_Product_BasePrice()
    {
        //Arrange
        var productList = new List<ProductServiceModel>
        {
            new ProductServiceModel{BasePrice = 180000, Name = "XC60", ManufacturerName = "Volvo", CategoryName = "SUV"}
        };

        var customerContext = new CurrentCustomerContext
        {
            Agreements = new List<Agreement>
            {
                new Agreement()
                {
                    ValidFrom = new DateTime(2019,09,25),
                    ValidTo = new DateTime(2021,09,02),

                    AgreementRows = new List<AgreementRow>
                    {
                        new AgreementRow()
                        {
                            PercentageDiscount = 6,
                            ManufacturerMatch = "volvo"
                        }
                    }
                }
            }
        };


        //Act
        var result = _sut.CalculatePrices(productList, customerContext);

        //Assert
        Assert.AreEqual(180000, result.First().Price);
    }

    [TestMethod]
    public void When_Two_Agreements_Is_Found_Use_Selected_Agreement()
    {
        //Arrange
        var productList = new List<ProductServiceModel>
        {
            new ProductServiceModel{
                BasePrice = 130000,
                CategoryName = "Volvo",
                Name = "hybrid"

            }
        };

        var customerContext = new CurrentCustomerContext
        {
            Agreements = new List<Agreement>
            {
                new Agreement()
                {
                    ValidTo = new DateTime(2024,09,02),

                    AgreementRows = new List<AgreementRow>
                    {
                        new AgreementRow()
                        {
                            PercentageDiscount = 8,
                            CategoryMatch = "van"
                        },
                        new AgreementRow()
                        {
                            PercentageDiscount = 5,
                            ProductMatch = "hybrid"
                        }
                    }
                },
                new Agreement()
                {
                    ValidTo = new DateTime(2024,09,02),
                    AgreementRows = new List<AgreementRow>
                    {
                        new AgreementRow()
                        {
                            PercentageDiscount = 7,
                            CategoryMatch = "volvo"
                        },
                        new AgreementRow()
                        {
                            PercentageDiscount = 3,
                            ProductMatch = "hybrid"
                        }
                    }
                }

            }
        };


        //Act
        var result = _sut.CalculatePrices(productList, customerContext);

        //Assert
        Assert.AreEqual(120900, result.First().Price);
    }

    [TestMethod]
    public void When_Two_Agreementrows_Are_Found_Should_Return_Discounted_Price_For_Selected_Category()
    {
        //Arrange
        var productList = new List<ProductServiceModel>
        {
            new ProductServiceModel{
                BasePrice = 180000,
                Name = "Mini XL",
                ManufacturerName = "AMC",
                CategoryName = "van"

            }
        };

        var customerContext = new CurrentCustomerContext
        {
            Agreements = new List<Agreement>
            {
                new Agreement()
                {
                    ValidFrom = new DateTime(2019,09,25),
                    ValidTo = new DateTime(2024,09,02),
                    AgreementRows = new List<AgreementRow>
                    {
                        new AgreementRow()
                        {
                            PercentageDiscount = 8,
                            CategoryMatch = "van",
                            ManufacturerMatch = "amc"
                        },
                        new AgreementRow()
                        {
                            PercentageDiscount = 5,
                            CategoryMatch = "hybrid"
                        }
                    }
                }
            }
        };


        //Act
        var result = _sut.CalculatePrices(productList, customerContext);

        //Assert
        Assert.AreEqual(165600, result.First().Price);

    }

}