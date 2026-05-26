using KooliProjekt.WindowsForms.Api;

namespace KooliProjekt.WindowsForms;

public partial class Form1 : Form, IMainView
{
    private MainViewPresenter _presenter;

    public Form1()
    {
        InitializeComponent();

        this.Load += async (s, e) => await _presenter.LoadData();
        
        dataGridView1.SelectionChanged += (s, e) =>
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var item = (Employees)dataGridView1.SelectedRows[0].DataBoundItem;
                _presenter.SetSelection(item);
            }
        };

        saveCommand.Click += async (s, e) => await _presenter.Save();
        deleteCommand.Click += async (s, e) => await _presenter.Delete();
        addCommand.Click += (s, e) => _presenter.SetSelection(null);
    }

    public IList<Employees> DataSource
    {
        get => (IList<Employees>)dataGridView1.DataSource;
        set => dataGridView1.DataSource = value;
    }

    public Employees SelectedItem
    {
        get => dataGridView1.SelectedRows.Count > 0 ? (Employees)dataGridView1.SelectedRows[0].DataBoundItem : null;
        set
        {
            // Do nothing for now
        }
    }

    public void SetPresenter(MainViewPresenter presenter)
    {
        _presenter = presenter;
    }

    public void ShowError(string message, OperationResult result)
    {
        var errors = string.Join("\n", result.Errors ?? new List<string>());
        MessageBox.Show($"{message}\n{errors}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public int CurrentId
    {
        get => int.TryParse(idField.Text, out var id) ? id : 0;
        set => idField.Text = value.ToString();
    }

    public string FirstName
    {
        get => firstNameField.Text;
        set => firstNameField.Text = value ?? "";
    }

    public string LastName
    {
        get => lastNameField.Text;
        set => lastNameField.Text = value ?? "";
    }

    public string Email
    {
        get => emailField.Text;
        set => emailField.Text = value ?? "";
    }

    public string Phone
    {
        get => phoneField.Text;
        set => phoneField.Text = value ?? "";
    }

    public string Role
    {
        get => roleField.Text;
        set => roleField.Text = value ?? "";
    }

    public bool ConfirmDelete()
    {
        var result = MessageBox.Show("Are you sure you want to delete this record?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        return result == DialogResult.Yes;
    }
}
