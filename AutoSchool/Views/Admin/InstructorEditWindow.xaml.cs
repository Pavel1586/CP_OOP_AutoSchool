using AutoSchool.Models;
using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace AutoSchool.Views.Admin;

public partial class InstructorEditWindow : Window
{
    public string FirstName => FirstNameBox.Text.Trim();
    public string LastName => LastNameBox.Text.Trim();
    public string Phone => PhoneBox.Text.Trim();
    public string Email => EmailBox.Text.Trim();
    public string VehicleBrand => BrandBox.Text.Trim();
    public string VehicleModel => ModelBox.Text.Trim();
    public string VehicleCategory => CategoryBox.Text.Trim();

    public TransmissionType Transmission
    {
        get
        {
            if (TransmissionBox.SelectedItem is ComboBoxItem item && item.Tag?.ToString() == "Automatic")
                return TransmissionType.Automatic;
            return TransmissionType.Manual;
        }
    }

    public byte[]? PhotoBytes { get; private set; }

    public InstructorEditWindow(Instructor? existing = null)
    {
        InitializeComponent();

        TransmissionBox.SelectedIndex = 0;

        if (existing != null)
        {
            FirstNameBox.Text = existing.FirstName;
            LastNameBox.Text = existing.LastName;
            PhoneBox.Text = existing.Phone;
            EmailBox.Text = existing.Email;
            BrandBox.Text = existing.VehicleBrand;
            ModelBox.Text = existing.VehicleModel;
            CategoryBox.Text = existing.VehicleCategory;

            TransmissionBox.SelectedIndex = existing.Transmission == TransmissionType.Automatic ? 1 : 0;

            PhotoBytes = existing.Photo;
            SetPreview(PhotoBytes);
        }
    }

    private void LoadPhoto_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Filter = "Images (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
        };

        if (dlg.ShowDialog() != true) return;

        PhotoBytes = File.ReadAllBytes(dlg.FileName);
        SetPreview(PhotoBytes);
    }

    private void ClearPhoto_Click(object sender, RoutedEventArgs e)
    {
        PhotoBytes = null;
        PhotoPreview.Source = null;
    }

    private void SetPreview(byte[]? bytes)
    {
        if (bytes == null || bytes.Length == 0)
        {
            PhotoPreview.Source = null;
            return;
        }

        using var ms = new MemoryStream(bytes);
        var bmp = new BitmapImage();
        bmp.BeginInit();
        bmp.CacheOption = BitmapCacheOption.OnLoad;
        bmp.StreamSource = ms;
        bmp.EndInit();
        bmp.Freeze();
        PhotoPreview.Source = bmp;
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
        {
            MessageBox.Show("Введите имя и фамилию.");
            return;
        }

        if (string.IsNullOrWhiteSpace(VehicleBrand) || string.IsNullOrWhiteSpace(VehicleModel))
        {
            MessageBox.Show("Введите марку и модель транспортного средства.");
            return;
        }

        if (string.IsNullOrWhiteSpace(VehicleCategory))
        {
            MessageBox.Show("Введите категорию (например B или A).");
            return;
        }

        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}