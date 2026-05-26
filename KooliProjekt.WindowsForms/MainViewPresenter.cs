using KooliProjekt.WindowsForms.Api;

namespace KooliProjekt.WindowsForms

{

    public class MainViewPresenter

    {

        private readonly IApiClient _apiClient;

        private readonly IMainView _mainView;

        private Employees _selectedList;

        public MainViewPresenter(IApiClient apiClient, IMainView mainView)

        {

            _apiClient = apiClient;

            _mainView = mainView;

            _mainView.SetPresenter(this);

        }

        public async Task LoadData()

        {

            var response = await _apiClient.List(1, 100);

            if (response.HasErrors)

            {

                _mainView.ShowError("Viga andmete laadimisel", response);

                _mainView.DataSource = null;

                return;

            }

            _mainView.DataSource = response.Value.Results;

        }

        public void SetSelection(Employees selectedList)

        {

            _selectedList = selectedList;

            if (_selectedList == null)

            {

                _mainView.CurrentId = 0;

                _mainView.FirstName = "";

                _mainView.LastName = "";

                _mainView.Email = "";

                _mainView.Phone = "";

                _mainView.Role = "";

            }

            else

            {

                _mainView.CurrentId = _selectedList.Id;

                _mainView.FirstName = _selectedList.FirstName;

                _mainView.LastName = _selectedList.LastName;

                _mainView.Email = _selectedList.Email;

                _mainView.Phone = _selectedList.Phone;

                _mainView.Role = _selectedList.Role;




            }
        }
        public async Task Save()
        {
            var employee = new Employees();
            employee.Id = _mainView.CurrentId;
            employee.FirstName = _mainView.FirstName;
            employee.LastName = _mainView.LastName;
            employee.Email = _mainView.Email;
            employee.Phone = _mainView.Phone;
            employee.Role = _mainView.Role;

            var result = await _apiClient.Save(employee);
            if (result.HasErrors)
            {
                _mainView.ShowError("Viga salvestamisel", result);
                return;
            }
            await LoadData();
        }
        public async Task Delete()
        {
            if (!_mainView.ConfirmDelete())
            {
                return;
            }

            var result = await _apiClient.Delete(_mainView.CurrentId);
            if (result.HasErrors)
            {
                _mainView.ShowError("Viga kustutamisel", result);
                return;
            }

            await LoadData();
        }
    }
}
