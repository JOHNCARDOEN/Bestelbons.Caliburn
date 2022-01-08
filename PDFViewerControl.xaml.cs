using PdfiumViewer.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace WPF_Bestelbons
{
    /// <summary>
    /// Interaction logic for PDFViewerControl.xaml
    /// </summary>
    public partial class PDFViewerControl : UserControl

    {

        public static event PropertyChangedEventHandler StaticPropertyChanged;

        private static string FileName;

        private static int AantalPages;
        private static float pdfwidth;
        private static float pdfheight;
        private static int CurrentPage;

        private static double Magnify;



        private static bool _visibilityNextPage;

        public static bool VisibilityNextPage
        {
            get { return _visibilityNextPage; }
            set
            {
                _visibilityNextPage = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(VisibilityNextPage)));
            }
        }


        private static bool _visibilityPrevPage;

        public static bool VisibilityPrevPage
        {
            get { return _visibilityPrevPage; }
            set
            {
                _visibilityPrevPage = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(VisibilityPrevPage)));
            }
        }

        private static bool _visibilityHundredPercent;

        public static bool VisibilityHundredPercent
        {
            get { return _visibilityHundredPercent; }
            set
            {
                _visibilityHundredPercent = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(VisibilityHundredPercent)));
            }
        }

        private static BitmapImage _bitmapImagePDF;
        public static BitmapImage BitmapImagePDF
        {
            get { return _bitmapImagePDF; }
            set
            {
                _bitmapImagePDF = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(BitmapImagePDF)));
            }
        }


        private static PdfDocument PdfDocument;

        private static System.Drawing.Image PdfImage;


        public string PDFFile
        {
            get { return (string)GetValue(PDFFileProperty); }
            set { SetValue(PDFFileProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PDFFile.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PDFFileProperty =
            DependencyProperty.Register("PDFFile", typeof(string), typeof(PDFViewerControl), new PropertyMetadata("", OnPDFFilePropertyChanged));


        private static void OnPDFFilePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileName = (string)e.NewValue;
            if (FileName != null)
            {
                if (FileName == "")
                {
                    if (PdfDocument != null) PdfDocument.Dispose();
                }

                else
                {
                    try
                    {
                        PdfDocument = PdfDocument.Load(FileName);
                        AantalPages = PdfDocument.PageCount - 1;
                        VisibilityNextPage = true;
                        VisibilityPrevPage = false;

                        if (AantalPages == 0)
                        {
                            VisibilityNextPage = false;
                            VisibilityPrevPage = false;
                        }
                        pdfwidth = PdfDocument.PageSizes[0].Width;
                        pdfheight = PdfDocument.PageSizes[0].Height;
                        Magnify = 2.8;
                        CurrentPage = 0;

                        VisibilityHundredPercent = false;

                        BitmapImagePDF = RenderPage(CurrentPage);

                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        public void ChangePDFFile(string file)
        {

        }

        public PDFViewerControl()
        {

            InitializeComponent();
        }

        private static BitmapImage RenderPage(int page)
        {
            PdfImage = PdfDocument.Render(page, (int)(pdfwidth * Magnify), (int)(pdfheight * Magnify), 300f, 300f, false);
            return BitmapImagePDF = Convert(PdfImage);
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            return null;
        }

        private static BitmapImage Convert(System.Drawing.Image img)
        {
            using (var memory = new MemoryStream())
            {
                //ImageCodecInfo pngEncoder = GetEncoder(ImageFormat.Png);
                //System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                //EncoderParameters myEncoderParameters = new EncoderParameters(1);

                //EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L);
                //myEncoderParameters.Param[0] = myEncoderParameter;

                //img.Save(memory, pngEncoder, myEncoderParameters);
                img.Save(memory, ImageFormat.Png);

                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }



        private void ZoomIn(object sender, RoutedEventArgs e)
        {
            Magnify += 1;
            if (Magnify != 2.8) VisibilityHundredPercent = true;
            else VisibilityHundredPercent = false;
            BitmapImagePDF = RenderPage(CurrentPage);
        }

        private void ZoomOut(object sender, RoutedEventArgs e)
        {
            if (Magnify >= 2) Magnify -= 1;
            if (Magnify != 2.8) VisibilityHundredPercent = true;
            else VisibilityHundredPercent = false;
            BitmapImagePDF = RenderPage(CurrentPage);
        }

        private void HundredPercent(object sender, RoutedEventArgs e)
        {
            Magnify = 2.8;
            VisibilityHundredPercent = false;
            BitmapImagePDF = RenderPage(CurrentPage);
        }


        private void NextPage(object sender, RoutedEventArgs e)
        {
            if (CurrentPage <= AantalPages - 1) CurrentPage += 1;
            if (CurrentPage == AantalPages) VisibilityNextPage = false;
            if (CurrentPage > 0) VisibilityPrevPage = true;
            BitmapImagePDF = RenderPage(CurrentPage);
        }

        private void PrevPage(object sender, RoutedEventArgs e)
        {
            if (CurrentPage >= 1) CurrentPage -= 1;
            if (CurrentPage == 0) VisibilityPrevPage = false;
            if (CurrentPage < AantalPages) VisibilityNextPage = true;
            BitmapImagePDF = RenderPage(CurrentPage);
        }
    }

    public class BoolToHiddenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value) return Visibility.Visible;
            else return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

