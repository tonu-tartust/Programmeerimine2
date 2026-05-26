using Moq;
using KooliProjekt.WindowsForms.Api;
using KooliProjekt.WindowsForms;
using KooliProjekt.WpfApplication;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace KooliProjekt.WpfApplication.UnitTests
{
    public class MainWindowViewModelTests
    {
        private readonly Mock<IApiClient> _apiClientMock;
        private readonly Mock<IDialogProvider> _dialogProviderMock;
        private readonly MainWindowViewModel _viewModel;

        public MainWindowViewModelTests()
        {
            _apiClientMock = new Mock<IApiClient>();
            _dialogProviderMock = new Mock<IDialogProvider>();
            _viewModel = new MainWindowViewModel(_apiClientMock.Object, _dialogProviderMock.Object);
        }

        [Fact]
        public void SelectedItem_should_return_correct_item()
        {
            // Arrange
            var item = new Employees { Id = 1, FirstName = "Test" };

            // Act
            _viewModel.SelectedItem = item;

            // Assert
            Assert.Equal(item, _viewModel.SelectedItem);
        }

        [Fact]
        public void SelectedItem_should_call_notify_property_changed()
        {
            // Arrange
            var item = new Employees { Id = 1, FirstName = "Test" };
            var propertyChangedRaised = false;
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(MainWindowViewModel.SelectedItem))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            _viewModel.SelectedItem = item;

            // Assert
            Assert.True(propertyChangedRaised);
        }

        [Fact]
        public async Task LoadData_should_load_data_from_api_client()
        {
            // Arrange
            var apiResult = new OperationResult<PagedResult<Employees>>
            {
                Value = new PagedResult<Employees>
                {
                    Results = new List<Employees>
                    {
                        new Employees { Id = 1, FirstName = "Test 1" },
                        new Employees { Id = 2, FirstName = "Test 2" }
                    }
                }
            };

            _apiClientMock.Setup(client => client.List(1, 100))
                .ReturnsAsync(apiResult)
                .Verifiable();

            // Act            
            await _viewModel.LoadData();

            // Assert
            _apiClientMock.VerifyAll();
            Assert.Equal(2, _viewModel.Data.Count);
            Assert.Equal(1, _viewModel.Data[0].Id);
            Assert.Equal(2, _viewModel.Data[1].Id);
        }

        [Fact]
        public async Task LoadData_should_show_error_when_api_client_fails()
        {
            // Arrange
            var apiResult = new OperationResult<PagedResult<Employees>>
            {
                Errors = new List<string> { "Error" }
            };

            _apiClientMock.Setup(client => client.List(1, 100))
                .ReturnsAsync(apiResult)
                .Verifiable();

            // Act            
            await _viewModel.LoadData();

            // Assert
            _apiClientMock.VerifyAll();
            Assert.Empty(_viewModel.Data);
        }

        [Fact]
        public void AddNew_Command_Should_Set_Empty_SelectedItem()
        {
            // Arrange
            _viewModel.SelectedItem = new Employees { Id = 1, FirstName = "Old" };
            
            // Act
            _viewModel.AddNewCommand.Execute(null);
            
            // Assert
            Assert.NotNull(_viewModel.SelectedItem);
            Assert.Equal(0, _viewModel.SelectedItem.Id);
            Assert.Null(_viewModel.SelectedItem.FirstName);
        }

        [Fact]
        public void SaveCommand_should_load_data_if_no_errors()
        {
            // Arrange
            var loadDataApiResult = new OperationResult<PagedResult<Employees>>
            {
                Value = new PagedResult<Employees>
                {
                    Results = new List<Employees>
                    {
                        new Employees { Id = 1, FirstName = "Test 1" },
                        new Employees { Id = 2, FirstName = "Test 2" }
                    }
                }
            };
            var saveDataApiResult = new OperationResult();
            var listToSave = new Employees { Id = 1, FirstName = "Test" };

            _apiClientMock.Setup(client => client.Save(It.IsAny<Employees>()))
                .ReturnsAsync(saveDataApiResult)
                .Verifiable();
            _apiClientMock.Setup(client => client.List(1, 100))
                .ReturnsAsync(loadDataApiResult)
                .Verifiable();

            // Act
            _viewModel.SaveCommand.Execute(listToSave);

            // Assert
            _apiClientMock.VerifyAll();
        }

        [Fact]
        public async Task SaveCommand_should_return_when_api_gave_error()
        {
            // Arrange
            var loadDataApiResult = new OperationResult<PagedResult<Employees>>();
            var saveDataApiResult = new OperationResult { Errors = new List<string> { "Error" } };
            var listToSave = new Employees { Id = 1, FirstName = "Test" };

            _apiClientMock.Setup(client => client.Save(It.IsAny<Employees>()))
                .ReturnsAsync(saveDataApiResult)
                .Verifiable();

            // Act
            _viewModel.SaveCommand.Execute(listToSave);

            // Assert
            _apiClientMock.VerifyAll();
            _apiClientMock.Verify(c => c.List(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task SaveCommand_can_execute_when_selected_item_is_not_null()
        {
            // Act & Assert
            _viewModel.SelectedItem = null;
            Assert.False(_viewModel.SaveCommand.CanExecute(null));

            _viewModel.SelectedItem = new Employees();
            Assert.True(_viewModel.SaveCommand.CanExecute(null));
        }

        [Fact]
        public async Task DeleteCommand_should_return_when_no_confirmation()
        {
            // Arrange
            _viewModel.SelectedItem = new Employees { Id = 1 };
            _dialogProviderMock.Setup(d => d.Confirm(It.IsAny<string>())).Returns(false);

            // Act
            _viewModel.DeleteCommand.Execute(null);

            // Assert
            _apiClientMock.Verify(c => c.Delete(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteCommand_should_load_data_if_no_errors()
        {
            // Arrange
            _viewModel.SelectedItem = new Employees { Id = 1 };
            _dialogProviderMock.Setup(d => d.Confirm(It.IsAny<string>())).Returns(true);

            _apiClientMock.Setup(c => c.Delete(1)).ReturnsAsync(new OperationResult()).Verifiable();
            _apiClientMock.Setup(c => c.List(1, 100)).ReturnsAsync(new OperationResult<PagedResult<Employees>> { Value = new PagedResult<Employees>() }).Verifiable();

            // Act
            _viewModel.DeleteCommand.Execute(null);

            // Assert
            _apiClientMock.VerifyAll();
            Assert.Null(_viewModel.SelectedItem);
        }

        [Fact]
        public async Task DeleteCommand_should_return_when_api_gave_error()
        {
            // Arrange
            _viewModel.SelectedItem = new Employees { Id = 1 };
            _dialogProviderMock.Setup(d => d.Confirm(It.IsAny<string>())).Returns(true);

            var deleteResult = new OperationResult { Errors = new List<string> { "Failed" } };
            _apiClientMock.Setup(c => c.Delete(1)).ReturnsAsync(deleteResult).Verifiable();

            // Act
            _viewModel.DeleteCommand.Execute(null);

            // Assert
            _apiClientMock.VerifyAll();
            _apiClientMock.Verify(c => c.List(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            Assert.NotNull(_viewModel.SelectedItem);
        }

        [Fact]
        public async Task DeleteCommand_can_execute_when_selected_item_is_not_null_and_id_is_not_zero()
        {
            // Act & Assert
            _viewModel.SelectedItem = null;
            Assert.False(_viewModel.DeleteCommand.CanExecute(null));

            _viewModel.SelectedItem = new Employees { Id = 0 };
            Assert.False(_viewModel.DeleteCommand.CanExecute(null));

            _viewModel.SelectedItem = new Employees { Id = 1 };
            Assert.True(_viewModel.DeleteCommand.CanExecute(null));
        }
    }
}
