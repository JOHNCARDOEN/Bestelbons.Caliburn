using Caliburn.Micro;
using MimeKit.Utils;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using WPF_Bestelbons.Events;
using WPF_Bestelbons.Models;
using System.Web;
using System.Text;
using MailKit.Net.Smtp;
using WPF_Bestelbons.Properties;
using System.Reflection;

namespace WPF_Bestelbons.ViewModels
{
    public class SendMailViewModel : Screen, IHandle<BestelbonToMailEvent>, IHandle<PrijsvraagToMailEvent>, IHandle<LeverancierAlteredEvent>, IHandle<MACAdressThisComputerEvent>
    {
        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;

        private DSNSmtpClient _mailclient;

        public BitmapImage[] Bitmaps = new BitmapImage[10];

        public List<String> CCEmailsSelected { get; set; }


        BindableCollection<Leverancier> Leverancierlist { get; set; }

        public Bestelbon Bestelbon { get; set; }
        public Prijsvraag Prijsvraag { get; set; }
        public string ProjectNumber { get; set; }
        public User CurrentUser { get; set; }
        public bool BestelbonOrPrijsvraag { get; set; }   // true bij Bestelbon, false bij Prijsvraag

        public string MACAdressThisComputer { get; set; }

        int PDFPage = 0;

        #region CANEXECUTE
        public bool CanAddCCEmail
        {
            get { return !string.IsNullOrEmpty(SelectedComboboxItem.Alias); }
        }
        public bool CanAddOwnCCEmail
        {
            get { return OwnCCEmailValid; }
        }
        #endregion

        #region  BINDABLE FIELDS    

        private string _fullPathAttachement;

        public string FullPathAttachement
        {
            get { return _fullPathAttachement; }
            set
            {
                _fullPathAttachement = value;
                NotifyOfPropertyChange(() => FullPathAttachement);
            }
        }


        private InternetAddressList _ccEmailsAdressList;

        public InternetAddressList CCEmailsAdressList
        {
            get { return _ccEmailsAdressList; }
            set
            {
                _ccEmailsAdressList = value;
                NotifyOfPropertyChange(() => CCEmailsAdressList);
            }
        }

        private string _ccEmail;

        public string CCEmail
        {
            get { return _ccEmail; }
            set
            {
                _ccEmail = value;
                NotifyOfPropertyChange(() => CCEmail);
            }
        }

        private string _ownCCEmail;

        public string OwnCCEmail
        {
            get { return _ownCCEmail; }
            set
            {
                _ownCCEmail = value;
                NotifyOfPropertyChange(() => OwnCCEmail);
            }
        }

        private CCEmailLeverancier _selectedComboboxItem;

        public CCEmailLeverancier SelectedComboboxItem
        {
            get { return _selectedComboboxItem; }
            set
            {
                _selectedComboboxItem = value;
                NotifyOfPropertyChange(() => SelectedComboboxItem);
                NotifyOfPropertyChange(() => CanAddCCEmail);
            }
        }


        private ObservableCollection<string> _attachments;

        public ObservableCollection<string> Attachments
        {
            get { return _attachments; }
            set
            {
                _attachments = value;
                NotifyOfPropertyChange(() => Attachments);
            }
        }


        private BindableCollection<CCEmailLeverancier> _ccEmails;

        public BindableCollection<CCEmailLeverancier> CCEmails
        {
            get { return _ccEmails; }
            set
            {
                _ccEmails = value;
                NotifyOfPropertyChange(() => CCEmails);
            }
        }

        private bool _mailSended;

        public bool MailSended
        {
            get { return _mailSended; }
            set
            {
                _mailSended = value;
                NotifyOfPropertyChange(() => MailSended);
            }
        }

        private bool _prevNextPDFVisible;

        public bool PrevNextPDFVisible
        {
            get { return _prevNextPDFVisible; }
            set
            {
                _prevNextPDFVisible = value;
                NotifyOfPropertyChange(() => PrevNextPDFVisible);
            }
        }

        private string _emailBody;

        public string EmailBody
        {
            get { return _emailBody; }
            set
            {
                _emailBody = value;
                NotifyOfPropertyChange(() => EmailBody);
            }
        }

        private bool _ownCCemailValid;

        public bool OwnCCEmailValid
        {
            get { return _ownCCemailValid; }
            set
            {
                _ownCCemailValid = value;
                NotifyOfPropertyChange(() => OwnCCEmailValid);
            }
        }

        private string _pDFFile;

        public string PDFFile
        {
            get { return _pDFFile; }
            set
            {
                _pDFFile = value;
                NotifyOfPropertyChange(() => PDFFile);
            }
        }

        private string _progressMail;

        public string ProgressMail
        {
            get { return _progressMail; }
            set
            {
                _progressMail = value;
                NotifyOfPropertyChange(() => ProgressMail);
            }
        }


        private Leverancier leverancier;

        public Leverancier Leverancier
        {
            get { return leverancier; }
            set
            {
                leverancier = value;
                NotifyOfPropertyChange(() => Leverancier);
            }
        }

        private int _mailProgress;

        public int MailProgress
        {
            get { return _mailProgress; }
            set
            {
                _mailProgress = value;
                NotifyOfPropertyChange(() => MailProgress);
            }
        }


        #endregion

        public SendMailViewModel(IEventAggregator eventAggregator, IWindowManager windowmanager, DSNSmtpClient mailclient)
        {
            _windowManager = windowmanager;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _mailclient = mailclient;
            this.CCEmailsAdressList = new InternetAddressList();
            this.CCEmails = new BindableCollection<CCEmailLeverancier>();
            this.Leverancierlist = new BindableCollection<Leverancier>();
            this.CCEmailsSelected = new List<string>();
            this.SelectedComboboxItem = new CCEmailLeverancier();
            //this.Bitmap = new BitmapImage();
            this.Attachments = new ObservableCollection<string>();
            this.Leverancier = new Leverancier();
            ProgressMail = string.Empty;

        }


        public void ViewPDF()
        {

            if (BestelbonOrPrijsvraag)
            {
                try
                {
                    if (File.Exists($"{Properties.Settings.Default.BestelbonsPath}\\{Bestelbon.Name}.pdf"))
                    {
                        PDFFile = $"{Properties.Settings.Default.BestelbonsPath}\\{Bestelbon.Name}.pdf";

                    }
                }
                catch (Exception ex)
                {
                    var dialog = IoC.Get<DialogViewModel>();
                    dialog.Capiton = " Opening PDF";
                    dialog.Message = ex.ToString();
                    _windowManager.ShowDialogAsync(dialog);
                }
            }
            else
            {
                try
                {
                    if (File.Exists($"{Properties.Settings.Default.PrijsvragenPath}\\{Prijsvraag.Name}.pdf"))
                    {
                        PDFFile = $"{Properties.Settings.Default.PrijsvragenPath}\\{Prijsvraag.Name}.pdf";

                    }
                }
                catch (Exception ex)
                {
                    var dialog = IoC.Get<DialogViewModel>();
                    dialog.Capiton = " Opening PDF";
                    dialog.Message = ex.ToString();
                    _windowManager.ShowDialogAsync(dialog);
                }
            }


        }
        public void PrintPDF()
        {
            ProcessStartInfo info;
            if (BestelbonOrPrijsvraag) info = new ProcessStartInfo($"{Properties.Settings.Default.BestelbonsPath}\\{Bestelbon.Name}.pdf");
            else info = new ProcessStartInfo($"{Properties.Settings.Default.PrijsvragenPath}\\{Prijsvraag.Name}.pdf");

            info.Verb = "printto";
            info.CreateNoWindow = false;
            info.UseShellExecute = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;

            try
            {
                Process.Start(info);
                _eventAggregator.PublishOnUIThreadAsync(new RaiseErrorEvent(new Error { Level = ErrorLevel.Error, ErrorMessage = " File to be printed not found !", Active = false }));
            }
            catch (Exception ex)
            {
                _eventAggregator.PublishOnUIThreadAsync(new RaiseErrorEvent(new Error { Level = ErrorLevel.Error, ErrorMessage = " File to be printed not found !", Active = true }));
            }
        }

        public void CloseButton()
        {
            TryCloseAsync();
            MailSended = false;
        }

        public void AddCCEmail()
        {
            CCEmailsAdressList.Add(new MailboxAddress(SelectedComboboxItem.Alias, SelectedComboboxItem.CCEmail));
            CCEmail += SelectedComboboxItem.CCEmail + "; ";
            CCEmailsSelected.Add(SelectedComboboxItem.Alias);
        }

        public void ClearCCEmail()
        {
            CCEmailsAdressList.Clear();
            CCEmail = "";
        }

        public void AddOwnCCEmail()
        {
            CCEmailsAdressList.Add(new MailboxAddress("", OwnCCEmail));
            CCEmail += OwnCCEmail + "; ";
            CCEmailsSelected.Add(OwnCCEmail);
        }

        public void ClearOwnCCEmail()
        {
            OwnCCEmail = "";
            OwnCCEmailValid = false;
        }

        public void KeyUpOwnCCEmail()
        {
            if (!string.IsNullOrEmpty(OwnCCEmail))
            {
                OwnCCEmailValid = EmailValidate(OwnCCEmail);
                NotifyOfPropertyChange(() => CanAddOwnCCEmail);
            }
        }

        #region FILE DRAG & DROP
        public void FilePreviewDragEnter(DragEventArgs e)
        {
            // TODO check if dragged item is a drive !!!
            e.Handled = true;
        }
        public void FileDropped(DragEventArgs e)
        {
            if (true)
            {
                string[] data = ((IDataObject)e.Data).GetData("FileName") as string[];
                FullPathAttachement = data[0];
                Attachments.Add(FullPathAttachement);
            }
        }
        #endregion



        public bool EmailValidate(string email)
        {
            Regex regex = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            Match match = regex.Match(email);
            return match.Success;
        }

        public async void SendEmail()
        {
            string Attachmentfilename;
            string Subject;

            if (BestelbonOrPrijsvraag)
            {
                Attachmentfilename = $"{Properties.Settings.Default.BestelbonsPath}\\{Bestelbon.Name}.pdf";
                Subject = $"Bestelbon  {Bestelbon.Name}";
            }
            else
            {
                Attachmentfilename = $"{Properties.Settings.Default.PrijsvragenPath}\\{Prijsvraag.Name}.pdf";
                Subject = $"Prijsvraag  {Prijsvraag.Name}";
            }

            string AllAliasses = string.Empty;
            for (int j = 0; j < Leverancier.CCEmails.Count; j++)
            {
                for (int k = 0; k < CCEmailsSelected.Count; k++)
                {
                    if (Leverancier.CCEmails[j].CCEmail == CCEmailsSelected[k])
                    {
                        AllAliasses += $" {Leverancier.CCEmails[j].Alias}";
                    }
                }
            }



            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(CurrentUser.FirstName, CurrentUser.Email));
            message.To.Add(new MailboxAddress(Leverancier.Name, Leverancier.Email));
            message.Subject = Subject;
            message.Cc.AddRange(CCEmailsAdressList);

            var builder = new BodyBuilder();
            MimeEntity image;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WPF_Bestelbons.Resources.LOGO_275.png"))
            {
                image = builder.LinkedResources.Add("LOGO_275.png", stream);
                image.ContentId = MimeUtils.GenerateMessageId();
            }


            string footer = string.Format(@"

                   <p><br></p>
                   <left><img src=""cid:{0}""></left>
                   <p>Industrielaan 19,zone C2 - B-8810 Lichtervelde<br>
                   Tel. +32 (0)51/72.24.46   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp      <a href=""http://www.astratec.be""> www.astratec.be</a> "
                 , image.ContentId);


            builder.HtmlBody = StringToHTML(EmailBody, false) + footer;

            builder.Attachments.Add(Attachmentfilename);
            message.Body = builder.ToMessageBody();

            if (BestelbonOrPrijsvraag)
            {
                if (!Bestelbon.BestelbonSend && CurrentUser.CanApprove && (MACAdressThisComputer == CurrentUser.MACAdress))
                {
                    MailProgress = 0;
                    ProgressMail = "Start sending email ....";
                    MailProgress = 15;

                    try
                    {
                        _mailclient.CheckCertificateRevocation = false;   // Probleem TOM proberen oplossen !
                        await _mailclient.ConnectAsync("smtp.office365.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                        ProgressMail = "Connected";
                        MailProgress = 30;
                        await _mailclient.AuthenticateAsync(CurrentUser.Email, CurrentUser.EmailAuthentication);
                        ProgressMail = "Authenticated";
                        MailProgress = 60;
                        // Check DNS capabilities
                        var supportsDsn = _mailclient.Capabilities.HasFlag(SmtpCapabilities.Dsn);

                        await _mailclient.SendAsync(message);
                        await _mailclient.DisconnectAsync(true);
                        ProgressMail = "Mail sucessfully send ";
                        MailProgress = 100;
                        MailSended = true;

                        if (BestelbonOrPrijsvraag)
                        {
                            Bestelbon.BestelbonSend = true;
                            Bestelbon.Approved = false;
                            Bestelbon.AskForApproval = false;

                            string FilePath = $"{Properties.Settings.Default.BestelbonsPath}\\{Bestelbon.Name}.abb";

                            using (var writer = new System.IO.StreamWriter(FilePath))
                            {
                                var serializer = new XmlSerializer(typeof(Bestelbon));
                                serializer.Serialize(writer, Bestelbon);
                                writer.Flush();

                            }

                            _eventAggregator.PublishOnUIThreadAsync(new BestelbonSendEvent(Bestelbon));
                        }

                    }
                    catch (Exception ex)
                    {
                        var dialogViewModel = IoC.Get<DialogViewModel>();
                        dialogViewModel.Capiton = "Send Mail Error";
                        dialogViewModel.Message = ex.Message.ToString();
                        var result = _windowManager.ShowDialogAsync(dialogViewModel);
                    }

                }
                else
                {
                    var dialogViewModel = IoC.Get<DialogViewModel>();
                    dialogViewModel.Capiton = "Authentication Error";
                    dialogViewModel.Message = "Mail reeds verzonden             OF \n\nMACAdress van deze laptop of PC komt niet overeen met User MACAdress\nMail moet verzonden worden van eigen laptop of PC";
                    var result = _windowManager.ShowDialogAsync(dialogViewModel);
                }
            }
            else 
            {
                MailProgress = 0;
                ProgressMail = "Start sending email ....";
                MailProgress = 15;

                try
                {
                    _mailclient.CheckCertificateRevocation = false;   // Probleem TOM proberen oplossen !
                    await _mailclient.ConnectAsync("smtp.office365.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    ProgressMail = "Connected";
                    MailProgress = 30;
                    await _mailclient.AuthenticateAsync(CurrentUser.Email, CurrentUser.EmailAuthentication);
                    ProgressMail = "Authenticated";
                    MailProgress = 60;
                    // Check DNS capabilities
                    var supportsDsn = _mailclient.Capabilities.HasFlag(SmtpCapabilities.Dsn);

                    await _mailclient.SendAsync(message);
                    await _mailclient.DisconnectAsync(true);
                    ProgressMail = "Mail sucessfully send ";
                    MailProgress = 100;
                    MailSended = true;
                }
                catch (Exception ex)
                {
                    var dialogViewModel = IoC.Get<DialogViewModel>();
                    dialogViewModel.Capiton = "Send Mail Error";
                    dialogViewModel.Message = ex.Message.ToString();
                    var result = _windowManager.ShowDialogAsync(dialogViewModel);
                }
            }

        }

        public string StringToHTML(string s, bool nofollow)
        {
            s = HttpUtility.HtmlEncode(s);
            string[] paragraphs = s.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
            StringBuilder sb = new StringBuilder();
            foreach (string par in paragraphs)
            {
                sb.AppendLine("<p>");
                string p = par.Replace(Environment.NewLine, "<br />\r\n");
                if (nofollow)
                {
                    p = Regex.Replace(p, @"\[\[(.+)\]\[(.+)\]\]", "<a href=\"$2\" rel=\"nofollow\">$1</a>");
                    p = Regex.Replace(p, @"\[\[(.+)\]\]", "<a href=\"$1\" rel=\"nofollow\">$1</a>");
                }
                else
                {
                    p = Regex.Replace(p, @"\[\[(.+)\]\[(.+)\]\]", "<a href=\"$2\">$1</a>");
                    p = Regex.Replace(p, @"\[\[(.+)\]\]", "<a href=\"$1\">$1</a>");
                    sb.AppendLine(p);
                }
                sb.AppendLine("</p>");
            }
            return sb.ToString();
        }


        #region EVENTHANDLERS

        public Task HandleAsync(BestelbonToMailEvent message, CancellationToken cancellationToken)
        {
            BestelbonOrPrijsvraag = true;
            Bestelbon = message.BestelbonToMail;
            CurrentUser = message.CurrentUser;
            ProjectNumber = message.ProjectNumber;
            Leverancier = message.Leverancier;
            CCEmails = Leverancier.CCEmails;
            MailProgress = 0;
            ProgressMail = string.Empty;
            if (CCEmails.Count > 0) SelectedComboboxItem = CCEmails[0];





            if (!string.IsNullOrEmpty(ProjectNumber)) EmailBody = $"Beste, " +
                                                                  $"\r\n\r\n Hier bijgevoegd onze bestelbon voor project {ProjectNumber}" +
                                                                  $" \r\n\r\nIndien mogelijk : opgave van vermoedelijke leverdatum" +
                                                                  $"\r\n\r\nAlgemene Info & Facturatie : boekhouding @astratec.be" +
                                                                  $"\r\n\r\n\r\n\r\nMet vriendelijke groeten" +
                                                                  $"\r\n {CurrentUser.LastName}  { CurrentUser.FirstName}";

            else EmailBody = $" Beste,  \r\n\r\nHier bijgevoegd onze bestelbon.  \r\n\r\n\r\n\r\n {CurrentUser.LastName}  { CurrentUser.FirstName} ";
            ViewPDF();
            CCEmailsSelected.Clear();
            return Task.CompletedTask;
        }

        public Task HandleAsync(PrijsvraagToMailEvent message, CancellationToken cancellationToken)
        {
            BestelbonOrPrijsvraag = false;
            Prijsvraag = message.PrijsvraagToMail;
            CurrentUser = message.CurrentUser;
            ProjectNumber = message.ProjectNumber;
            Leverancier = message.Leverancier;
            CCEmails = Leverancier.CCEmails;
            if (!string.IsNullOrEmpty(ProjectNumber)) EmailBody = $" Beste, \r\n\r\nHier bijgevoegd onze prijsvraag voor project {ProjectNumber} \r\n\r\n\r\n\r\n {CurrentUser.LastName}  { CurrentUser.FirstName}";
            else EmailBody = $" Beste,  \r\n\r\n\r\n\r\nHier bijgevoegd onze prijsvraag.  \r\n\r\n\r\n\r\n {CurrentUser.LastName}  { CurrentUser.FirstName}";
            ViewPDF();
            CCEmailsSelected.Clear();
            return Task.CompletedTask;
        }

        public Task HandleAsync(LeverancierAlteredEvent message, CancellationToken cancellationToken)
        {
            if (Leverancier.Name == message.Leverancier.Name)
            {
                CCEmails = message.Leverancier.CCEmails;
            }
            return Task.CompletedTask;
        }

        public Task HandleAsync(MACAdressThisComputerEvent message, CancellationToken cancellationToken)
        {
            MACAdressThisComputer = message.MACAdress;
            return Task.CompletedTask;
        }
        #endregion

    }
}
