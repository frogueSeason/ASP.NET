using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests
    {
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerNotFound_ShouldReturn404()
        {
            // Arrange
            var mockRepo = new Mock<IRepository<Partner>>();
            mockRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Partner)null);

            var controller = new PartnersController(mockRepo.Object);

            // Act
            var result = await controller.SetPartnerPromoCodeLimitAsync(Guid.NewGuid(), new SetPartnerPromoCodeLimitRequest());

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsBlocked_ShouldReturnBadRequest()
        {
            // Arrange
            var blockedPartner = new Partner
            {
                IsActive = false,
                PartnerLimits = new List<PartnerPromoCodeLimit>()
            };

            var mockRepo = new Mock<IRepository<Partner>>();
            mockRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(blockedPartner);

            var controller = new PartnersController(mockRepo.Object);

            // Act
            var result = await controller.SetPartnerPromoCodeLimitAsync(Guid.NewGuid(), new SetPartnerPromoCodeLimitRequest());

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_LimitLessThanOrEqualToZero_ShouldReturnBadRequest()
        {
            // Arrange
            var activePartner = new Partner
            {
                IsActive = true,
                PartnerLimits = new List<PartnerPromoCodeLimit>()
            };

            var mockRepo = new Mock<IRepository<Partner>>();
            mockRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(activePartner);

            var controller = new PartnersController(mockRepo.Object);

            // Act
            var result = await controller.SetPartnerPromoCodeLimitAsync(Guid.NewGuid(), new SetPartnerPromoCodeLimitRequest { Limit = 0 });

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }


        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_NewLimit_ShouldSaveToDatabase()
        {
            // Arrange
            var activePartner = new Partner
            {
                IsActive = true,
                PartnerLimits = new List<PartnerPromoCodeLimit>()
            };

            var mockRepo = new Mock<IRepository<Partner>>();
            mockRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(activePartner);
            mockRepo.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var controller = new PartnersController(mockRepo.Object);

            // Act
            await controller.SetPartnerPromoCodeLimitAsync(Guid.NewGuid(), new SetPartnerPromoCodeLimitRequest { Limit = 10 });

            // Assert
            mockRepo.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_LimitSet_ShouldResetNumberIssuedPromoCodes()
        {
            // Arrange
            var activePartner = new Partner
            {
                IsActive = true,
                NumberIssuedPromoCodes = 5, 
                PartnerLimits = new List<PartnerPromoCodeLimit>
        {
            new PartnerPromoCodeLimit { }
        }
            };

            var mockRepo = new Mock<IRepository<Partner>>();
            mockRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(activePartner);
            mockRepo.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var controller = new PartnersController(mockRepo.Object);

            // Act
            await controller.SetPartnerPromoCodeLimitAsync(Guid.NewGuid(), new SetPartnerPromoCodeLimitRequest { Limit = 10 });

            // Assert
            activePartner.NumberIssuedPromoCodes.Should().Be(0);
        }
        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PreviousLimit_ShouldBeDisabled()
        {
            // Arrange
            var activePartner = new Partner
            {
                IsActive = true,
                PartnerLimits = new List<PartnerPromoCodeLimit>
        {
            new PartnerPromoCodeLimit {  CancelDate = null } 
        }
            };

            var mockRepo = new Mock<IRepository<Partner>>();
            mockRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(activePartner);
            mockRepo.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var controller = new PartnersController(mockRepo.Object);

            // Act
            await controller.SetPartnerPromoCodeLimitAsync(Guid.NewGuid(), new SetPartnerPromoCodeLimitRequest { Limit = 10 });

            // Assert
            activePartner.PartnerLimits.First().IsActive.Should().BeFalse(); 
        }


    }
}