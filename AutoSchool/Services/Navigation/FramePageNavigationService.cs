using System.Windows.Controls;

namespace AutoSchool.Services.Navigation;

public class FramePageNavigationService : IPageNavigationService
{
    private readonly Frame _frame;

    public FramePageNavigationService(Frame frame)
    {
        _frame = frame;
    }

    public void Navigate(Page page) => _frame.Navigate(page);
}