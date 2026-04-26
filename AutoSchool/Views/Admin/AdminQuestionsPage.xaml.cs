using System;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;

namespace AutoSchool.Views.Admin
{
    public partial class AdminQuestionsPage : Page
    {
        public AdminQuestionsPage()
        {
            InitializeComponent();
        }

        // НУЖНО для вызовов new AdminQuestionsPage(arg1, arg2)
        public AdminQuestionsPage(object? arg1, object? arg2) : this()
        {
            InitFromArgs(arg1, arg2);
        }

        private void InitFromArgs(object? arg1, object? arg2)
        {
            // 1) Если передали ViewModel — просто используем её
            if (LooksLikeViewModel(arg1)) { DataContext = arg1; return; }
            if (LooksLikeViewModel(arg2)) { DataContext = arg2; return; }

            // 2) Если передали nav + ticketId (в любом порядке) — попробуем создать VM автоматически
            var nav = arg1 ?? arg2;
            int? ticketId = null;

            if (arg1 is int i1) ticketId = i1;
            if (arg2 is int i2) ticketId = i2;

            if (ticketId.HasValue && nav != null)
                TryCreateAdminQuestionsViewModel(nav, ticketId.Value);
        }

        private static bool LooksLikeViewModel(object? obj)
        {
            if (obj == null) return false;
            var t = obj.GetType();
            return t.Name.EndsWith("ViewModel", StringComparison.OrdinalIgnoreCase);
        }

        private void TryCreateAdminQuestionsViewModel(object nav, int ticketId)
        {
            // Пытаемся найти AutoSchool.ViewModels.Admin.AdminQuestionsViewModel
            var asm = Assembly.GetExecutingAssembly();
            var vmType = asm.GetType("AutoSchool.ViewModels.Admin.AdminQuestionsViewModel");
            if (vmType == null) return;

            // Ищем конструктор (nav, int) или (int, nav)
            var ctors = vmType.GetConstructors();

            foreach (var ctor in ctors)
            {
                var p = ctor.GetParameters();
                if (p.Length != 2) continue;

                // (nav, int)
                if (p[0].ParameterType.IsInstanceOfType(nav) && p[1].ParameterType == typeof(int))
                {
                    DataContext = ctor.Invoke(new object[] { nav, ticketId });
                    return;
                }

                // (int, nav)
                if (p[0].ParameterType == typeof(int) && p[1].ParameterType.IsInstanceOfType(nav))
                {
                    DataContext = ctor.Invoke(new object[] { ticketId, nav });
                    return;
                }
            }

            // Если нужного конструктора нет — ничего не делаем (VM может задаваться снаружи)
        }

        // Если в XAML стоит SelectionChanged="DataGrid_SelectionChanged" — метод должен существовать
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Можно оставить пустым (SelectedItem уже биндингом уходит в VM)
        }
    }
}