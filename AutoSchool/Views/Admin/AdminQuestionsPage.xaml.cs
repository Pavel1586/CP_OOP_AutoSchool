using System.Windows.Controls;
using AutoSchool.ViewModels.Admin;

namespace AutoSchool.Views.Admin
{
    public partial class AdminQuestionsPage : Page
    {
        public AdminQuestionsPage()
        {
            InitializeComponent();
            DataContext ??= new AdminQuestionsEditorViewModel(null, null);
        }

        // Совместимость: страницу могут создавать как new AdminQuestionsPage(arg1, arg2)
        public AdminQuestionsPage(object? arg1, object? arg2) : this()
        {
            // Если явно передали наш новый VM — используем его
            if (arg1 is AdminQuestionsEditorViewModel vm1) { DataContext = vm1; return; }
            if (arg2 is AdminQuestionsEditorViewModel vm2) { DataContext = vm2; return; }

            // Иначе пытаемся извлечь nav и ticketId и создаём НАШ VM
            var (nav, ticketId) = ExtractNavAndTicketId(arg1, arg2);
            DataContext = new AdminQuestionsEditorViewModel(nav, ticketId);
        }

        private static (object? nav, int? ticketId) ExtractNavAndTicketId(object? a, object? b)
        {
            object? nav = null;
            int? ticketId = null;

            if (a is int ia) ticketId = ia; else nav = a;
            if (b is int ib) ticketId = ib; else if (nav == null) nav = b;

            return (nav, ticketId);
        }
    }
}