using System.Windows.Controls;

namespace AutoSchool.Services.Navigation;

public interface IPageNavigationService
{
    void Navigate(Page page);
}