using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.EAS.Account.Api.Types;
using Xunit;

namespace DfE.EmployerFavourites.Web.UnitTests.Controllers
{
    public class SaveBasket_MultipleAccounts_ApprenticeshipsControllerTests : ApprenticeshipsControllerTestsBase
    {
        [Fact]
        public async Task SaveBasket_with_multiple_accounts_returns_ChooseAccount_view()
        {
            var result = await Sut.SaveBasket(Guid.NewGuid());

            Assert.IsType<ViewResult>(result);
            
            Assert.Equal("ChooseAccount", (result as ViewResult).ViewName); 
        }
        
        [Fact]
        public async Task SaveBasket_with_multiple_accounts_returns_ChooseAccount_ViewResult_With_Accounts_Model()
        {
            var result = await Sut.SaveBasket(Guid.NewGuid());

            Assert.IsType<List<AccountDetailViewModel>>((result as ViewResult).Model);
            
            var accounts  = ((ViewResult)result).Model as List<AccountDetailViewModel>;
            
            Assert.Equal(4, accounts.Count);
        }


        [Fact]
        public async Task ChooseAccount_with_no_account_selected_returns_ChooseAccount_view_and_populated_ModelState()
        {
            var result = await Sut.ChooseAccount(null, Guid.NewGuid());
            
            Assert.IsType<ViewResult>(result);
            
            Assert.Equal("ChooseAccount", (result as ViewResult).ViewName);
            
            Assert.Equal("Please choose an account to continue", ((ViewResult)result).ViewData.ModelState.Values.First().Errors.First().ErrorMessage);
            Assert.Equal("chosenHashedAccountId", ((ViewResult)result).ViewData.ModelState.Keys.First());
        }

        [Fact]
        public async Task ChooseAccount_with_selected_account_uses_selected_accountId()
        {
            await Sut.ChooseAccount("AAA123", Guid.NewGuid());
            
            _mockFavouritesReadRepository.Verify(r => r.GetApprenticeshipFavourites("AAA123"), Times.Once);
        }
        

        protected override void SetupUserInContext()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("http://das/employer/identity/claims/id", USER_ID_WITH_MULTIPLE_ACCOUNTS)
            }));

            Sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }
    }
}