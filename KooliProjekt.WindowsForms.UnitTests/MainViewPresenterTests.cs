using KooliProjekt.WindowsForms.Api;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace KooliProjekt.WindowsForms.UnitTests
{
    public class MainViewPresenterTests
    {
        private Mock<IMainView> _viewMock;
        private Mock<IApiClient> _apiMock;
        private MainViewPresenter _presenter;

        public MainViewPresenterTests()
        {
            _viewMock = new Mock<IMainView>();
            _apiMock = new Mock<IApiClient>();
        }

        [Fact]
        public void LoadData_ShouldPopulateView()
        {
            // Arrange
            var expectedEmployees = new List<Employees> { new Employees { Id = 1, FirstName = "Test" } };
            
            _apiMock.Setup(x => x.List(1, 100))
                   .ReturnsAsync(new OperationResult<PagedResult<Employees>> { Value = new PagedResult<Employees> { Results = expectedEmployees } });
                   
            _presenter = new MainViewPresenter(_apiMock.Object, _viewMock.Object);
            
            // Act
            _presenter.LoadData().Wait();
            
            // Assert
            _viewMock.VerifySet(v => v.DataSource = expectedEmployees);
        }

        [Fact]
        public void LoadData_ShouldShowError_WhenApiFails()
        {
            // Arrange
            var result = new OperationResult<PagedResult<Employees>> { Errors = new List<string> { "Error" } };
            _apiMock.Setup(x => x.List(1, 100)).ReturnsAsync(result);

            _presenter = new MainViewPresenter(_apiMock.Object, _viewMock.Object);

            // Act
            _presenter.LoadData().Wait();

            // Assert
            _viewMock.Verify(v => v.ShowError("Viga andmete laadimisel", result), Times.Once);
            _viewMock.VerifySet(v => v.DataSource = null);
        }

        [Fact]
        public void SetSelection_ShouldClearForm_WhenNull()
        {
            // Arrange
            _presenter = new MainViewPresenter(_apiMock.Object, _viewMock.Object);

            // Act
            _presenter.SetSelection(null);

            // Assert
            _viewMock.VerifySet(v => v.CurrentId = 0);
            _viewMock.VerifySet(v => v.FirstName = "");
            _viewMock.VerifySet(v => v.LastName = "");
            _viewMock.VerifySet(v => v.Email = "");
            _viewMock.VerifySet(v => v.Phone = "");
            _viewMock.VerifySet(v => v.Role = "");
        }

        [Fact]
        public void SetSelection_ShouldPopulateForm_WhenItemProvided()
        {
            // Arrange
            _presenter = new MainViewPresenter(_apiMock.Object, _viewMock.Object);
            var item = new Employees { Id = 1, FirstName = "John", LastName = "Doe", Email = "j@d", Phone = "123", Role = "R" };

            // Act
            _presenter.SetSelection(item);

            // Assert
            _viewMock.VerifySet(v => v.CurrentId = 1);
            _viewMock.VerifySet(v => v.FirstName = "John");
            _viewMock.VerifySet(v => v.LastName = "Doe");
            _viewMock.VerifySet(v => v.Email = "j@d");
            _viewMock.VerifySet(v => v.Phone = "123");
            _viewMock.VerifySet(v => v.Role = "R");
        }

        [Fact]
        public void Save_ShouldCallApi_AndReload()
        {
            // Arrange
            _presenter = new MainViewPresenter(_apiMock.Object, _viewMock.Object);
            
            _viewMock.Setup(v => v.CurrentId).Returns(1);
            _viewMock.Setup(v => v.FirstName).Returns("Test");

            _apiMock.Setup(a => a.Save(It.IsAny<Employees>())).ReturnsAsync(new OperationResult());
            _apiMock.Setup(a => a.List(1, 100)).ReturnsAsync(new OperationResult<PagedResult<Employees>> { Value = new PagedResult<Employees>() });

            // Act
            _presenter.Save().Wait();

            // Assert
            _apiMock.Verify(a => a.Save(It.IsAny<Employees>()), Times.Once);
            _apiMock.Verify(a => a.List(1, 100), Times.Once);
        }

        [Fact]
        public void Save_ShouldShowError_WhenApiFails()
        {
            // Arrange
            _presenter = new MainViewPresenter(_apiMock.Object, _viewMock.Object);
            
            var errResult = new OperationResult { Errors = new List<string> { "Err" } };
            _apiMock.Setup(a => a.Save(It.IsAny<Employees>())).ReturnsAsync(errResult);

            // Act
            _presenter.Save().Wait();

            // Assert
            _viewMock.Verify(v => v.ShowError("Viga salvestamisel", errResult), Times.Once);
            _apiMock.Verify(a => a.List(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Delete_ShouldReturn_IfNoConfirm()
        {
            // Arrange
            _presenter = new MainViewPresenter(_apiMock.Object, _viewMock.Object);
            _viewMock.Setup(v => v.ConfirmDelete()).Returns(false);

            // Act
            _presenter.Delete().Wait();

            // Assert
            _apiMock.Verify(a => a.Delete(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Delete_ShouldCallApi_AndReload()
        {
            // Arrange
            _presenter = new MainViewPresenter(_apiMock.Object, _viewMock.Object);
            _viewMock.Setup(v => v.ConfirmDelete()).Returns(true);
            _viewMock.Setup(v => v.CurrentId).Returns(1);

            _apiMock.Setup(a => a.Delete(1)).ReturnsAsync(new OperationResult());
            _apiMock.Setup(a => a.List(1, 100)).ReturnsAsync(new OperationResult<PagedResult<Employees>> { Value = new PagedResult<Employees>() });

            // Act
            _presenter.Delete().Wait();

            // Assert
            _apiMock.Verify(a => a.Delete(1), Times.Once);
            _apiMock.Verify(a => a.List(1, 100), Times.Once);
        }
    }
}
