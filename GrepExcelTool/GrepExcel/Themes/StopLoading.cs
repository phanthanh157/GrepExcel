using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GrepExcel.Themes
{
    public class StopLoading : Control
    {
        public static readonly DependencyProperty RefreshCommandProperty =
           DependencyProperty.Register("RefreshCommand", typeof(ICommand), typeof(StopLoading),
               new PropertyMetadata(null));

        public ICommand RefreshCommand
        {
            get { return (ICommand)GetValue(RefreshCommandProperty); }
            set { SetValue(RefreshCommandProperty, value); }
        }

        public static readonly DependencyProperty StopLoadingCommandProperty =
           DependencyProperty.Register("StopLoadingCommand", typeof(ICommand), typeof(StopLoading),
               new PropertyMetadata(null));

        public ICommand StopLoadingCommand
        {
            get { return (ICommand)GetValue(StopLoadingCommandProperty); }
            set { SetValue(StopLoadingCommandProperty, value); }
        }


        public static readonly DependencyProperty IsLoadingProperty =
          DependencyProperty.Register("IsLoading", typeof(bool), typeof(StopLoading),
              new PropertyMetadata(false));

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }
        static StopLoading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StopLoading), new FrameworkPropertyMetadata(typeof(StopLoading)));
        }

    }
}
