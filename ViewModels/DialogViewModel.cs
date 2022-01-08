using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WPF_Bestelbons.ViewModels
{
    public class DialogViewModel : Caliburn.Micro.Screen
    {
        public DialogResult MyDialogResult { get; private set; }

        #region BINDABLE FIELDS
        private string _capiton;

        public string Capiton
        {
            get { return _capiton; }
            set
            {
                _capiton = value;
                NotifyOfPropertyChange(() => Capiton);
            }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                NotifyOfPropertyChange(() => Message);
            }
        }

        private int _fontsizeMessage;

        public int FontsizeMessage
        {
            get { return _fontsizeMessage; }
            set
            {
                _fontsizeMessage = value;
                NotifyOfPropertyChange(() => FontsizeMessage);
            }
        }

        private bool _yesVisible;

        public bool YesVisible
        {
            get { return _yesVisible; }
            set
            {
                _yesVisible = value;
                NotifyOfPropertyChange(() => YesVisible);
            }
        }

        private bool _noVisible;

        public bool NoVisible
        {
            get { return _noVisible; }
            set
            {
                _noVisible = value;
                NotifyOfPropertyChange(() => NoVisible);
            }
        }

        private bool _okVisible;

        public bool OkVisible
        {
            get { return _okVisible; }
            set
            {
                _okVisible = value;
                NotifyOfPropertyChange(() => OkVisible);
            }
        }

        private bool _cancelVisible;

        public bool CancelVisible
        {
            get { return _cancelVisible; }
            set
            {
                _cancelVisible = value;
                NotifyOfPropertyChange(() => CancelVisible);
            }
        }

        private DialogStyle dialogStyle;

        public DialogStyle DialogStyle
        {
            get { return dialogStyle; }
            set
            {
                dialogStyle = value;
                switch (value)
                {
                    case DialogStyle.Yes:
                        YesVisible = true;
                        break;
                    case DialogStyle.No:
                        NoVisible = true;
                        break;
                    case DialogStyle.YesNo:
                        YesVisible = true;
                        NoVisible = true;
                        break;
                    case DialogStyle.Ok:
                        OkVisible = true;
                        break;
                    case DialogStyle.Cancel:
                        CancelVisible = true;
                        break;
                    case DialogStyle.OkCancel:
                        OkVisible = true;
                        CancelVisible = true;
                        break;
                    case DialogStyle.NoButtons:
                        OkVisible = false;
                        CancelVisible = false;
                        break;
                    default:
                        break;
                }
                NotifyOfPropertyChange(() => DialogStyle);
            }
        }

        private int _height;

        public int Height
        {
            get { return _height; }
            set
            {
                _height = value;
                NotifyOfPropertyChange(() => Height);
            }
        }

        private int _width;

        public int Width
        {
            get { return _width; }
            set
            {
                _width = value;
                NotifyOfPropertyChange(() => Width);
            }
        }

        #endregion

        public DialogViewModel()
        {
            NotifyOfPropertyChange(() => DialogStyle);
        }
        public void YES()
        {
            MyDialogResult = DialogResult.Yes;
            TryCloseAsync();
        }

        public void NO()
        {
            MyDialogResult = DialogResult.No;
            TryCloseAsync();
        }

        public void OK()
        {
            MyDialogResult = DialogResult.OK;
            TryCloseAsync();
        }
        public void CANCEL()
        {
            MyDialogResult = DialogResult.Cancel;
            TryCloseAsync();
        }

        public void CloseButton()
        {
            TryCloseAsync(false);
        }
    }

    public enum DialogStyle
    {
        Yes,
        No,
        YesNo,
        Ok,
        Cancel,
        OkCancel,
        NoButtons
    }
}
