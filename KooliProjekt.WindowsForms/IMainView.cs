using System;
using System.Collections.Generic;
using System.Text;
using KooliProjekt.WindowsForms.Api;

namespace KooliProjekt.WindowsForms
{
    public interface IMainView
    {
        IList<Employees> DataSource { get; set; }
        Employees SelectedItem { get; set; }
        void SetPresenter(MainViewPresenter presenter);
        void ShowError(string message, OperationResult result);
        int CurrentId { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        string Phone { get; set; }
        string Role { get; set; }
        bool ConfirmDelete();

    }
}