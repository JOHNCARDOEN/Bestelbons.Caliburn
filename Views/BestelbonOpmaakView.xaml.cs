using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF_Bestelbons.ViewModels;

namespace WPF_Bestelbons.Views
{
    /// <summary>
    /// Interaction logic for BestelbonOpmaakView.xaml
    /// </summary>
    public partial class BestelbonOpmaakView : UserControl
    {

        BestelbonOpmaakViewModel vm { get; set; }

        public BestelbonOpmaakView()
        {
            InitializeComponent();
            vm = IoC.Get<BestelbonOpmaakViewModel>();
        }

        //private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    String sSelectedText = mytextbox.SelectedText;
        //}


        private void Aantal_MouseEnter(object sender, MouseEventArgs e)
        {
            var tb = (TextBox)sender;
            if (tb != null)
            {
                tb.SelectAll();
                tb.Focus();

            }
        }

        private void Aantal_MouseLeave(object sender, MouseEventArgs e)
        {
            var tb = (TextBox)sender;
            if (tb != null)
            {
                tb.Select(0, 0);
                //tb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                Keyboard.ClearFocus();
            }
        }

        private void Aantal_Regel_MouseEnter(object sender, MouseEventArgs e)
        {
            var tb = (TextBox)sender;
            if (tb != null)
            {
                tb.SelectAll();
                tb.Focus();

            }
        }

        private void Aantal_Regel_MouseLeave(object sender, MouseEventArgs e)
        {
            var tb = (TextBox)sender;
            if (tb != null)
            {
                tb.Select(0,0);
                //tb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                Keyboard.ClearFocus();
            }
        }

        private void Prijs_MouseEnter(object sender, MouseEventArgs e)
        {
            var tb = (TextBox)sender;
            if (tb != null)
            {
                tb.SelectAll();
                tb.Focus();

            }
        }

        private void Prijs_MouseLeave(object sender, MouseEventArgs e)
        {
            var tb = (TextBox)sender;
            if (tb != null)
            {
                tb.Select(0, 0);
                //tb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                Keyboard.ClearFocus();
            }
        }

        private void Prijs_Regel_MouseEnter(object sender, MouseEventArgs e)
        {
            var tb = (TextBox)sender;
            if (tb != null)
            {
                tb.SelectAll();
                tb.Focus();

            }
        }

        private void Prijs_Regel_MouseLeave(object sender, MouseEventArgs e)
        {
            var tb = (TextBox)sender;
            if (tb != null)
            {
                tb.Select(0, 0);
                //tb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                Keyboard.ClearFocus();
            }
        }

        public void ScrollUp(object sender, EventArgs e)
        {
            Bestelbonregels.UpdateLayout();
            ScrollViewer sv = GetChildOfType<ScrollViewer>(Bestelbonregels);  // GET the internal scrollvieuwer of the listbox !!
            if (sv != null) sv.ScrollToBottom();
        }

        private static T GetChildOfType<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null)
                return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }

    }
}
