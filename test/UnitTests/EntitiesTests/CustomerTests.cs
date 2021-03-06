namespace UnitTests.EntitiesTests
{
    using Domain.Customers;
    using Domain.Customers.ValueObjects;
    using Infrastructure.InMemoryDataAccess;
    using Xunit;

    public class CustomerTests
    {
        [Fact]
        public void Customer_Should_Be_Registered_With_1_Account()
        {
            var entityFactory = new EntityFactory();

            // Arrange
            ICustomer sut = entityFactory.NewCustomer(
                new SSN("198608179922"),
                new Name("Ivan Paulovich"));

            var account = entityFactory.NewAccount(sut.Id);

            // Act
            sut.Register(account.Id);

            // Assert
            Assert.Single(sut.Accounts.GetAccountIds());
        }
    }
}
