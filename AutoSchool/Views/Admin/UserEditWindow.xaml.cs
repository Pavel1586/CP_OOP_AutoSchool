using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AutoSchool.Views.Admin;

public partial class UserEditWindow : Window
{
    public string FirstName => FirstNameBox.Text.Trim();
    public string LastName => LastNameBox.Text.Trim();
    public string Email => EmailBox.Text.Trim();
    public int RoleId
    {
        get
        {
            if (RoleBox.SelectedItem is ComboBoxItem item && int.TryParse(item.Tag?.ToString(), out var id))
                return id;
            return 2;
        }
    }

    public UserEditWindow(string firstName, string lastName, string email, int roleId)
    {
        InitializeComponent();

        FirstNameBox.Text = firstName;
        LastNameBox.Text = lastName;
        EmailBox.Text = email;

        // выбрать роль
        var item = RoleBox.Items.OfType<ComboBoxItem>().FirstOrDefault(i => i.Tag?.ToString() == roleId.ToString());
        RoleBox.SelectedItem = item ?? RoleBox.Items.OfType<ComboBoxItem>().First();

        FirstNameBox.Focus();
        FirstNameBox.SelectAll();
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FirstName) ||
            string.IsNullOrWhiteSpace(LastName) ||
            string.IsNullOrWhiteSpace(Email))
        {
            MessageBox.Show("Заполните все поля.");
            return;
        }

        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}