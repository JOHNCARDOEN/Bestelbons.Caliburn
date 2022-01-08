using Caliburn.Micro;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.IO.Image;
using iText.IO.Util;
using System;
using System.IO;
using WPF_Bestelbons.Events;
using iText.IO.Font.Constants;

namespace WPF_Bestelbons.Models
{
    public class PDFCreatorBestelbon
    {
        public string ProjectDirectory { get; set; }

        public BindableCollection<User> UserList { get; set; }

        public void Create(Bestelbon bestelbon, User CurrentnUser, string FilePath)
        {

            string dest = FilePath;
            FileInfo file = new FileInfo(dest);
            if (!file.Directory.Exists) file.Directory.Create();

            PdfWriter pdfwriter = new PdfWriter(dest);
            PdfDocument pdf = new PdfDocument(pdfwriter);
            Document document = new Document(pdf, PageSize.A4);

            //PdfFontFactory.Register("c:/windows/fonts/SegoeUI.ttf", "SegoeUI");

            PdfFont Segoe = PdfFontFactory.CreateFont("c:/windows/fonts/SegoeUI.ttf");

            #region FIRST LAYER

            ImageData imgdata = ImageDataFactory.Create(@"Z:\BESTELBON\DATA\LOGO.png");
            Image img = new Image(imgdata);

            img.ScaleToFit(180, 180);
            img.SetFixedPosition(40.0f, 770.0f);
            document.Add(img);
            #endregion

            #region SECOND LAYER

            // ADRESS REGION ASTRATEC

            float[] columnWidths = { 1.2f, 1.2f, 1f, 1.4f, 1.5f };
            Table Adresstable = new Table(UnitValue.CreatePercentArray(columnWidths)).UseAllAvailableWidth();

            Paragraph pEmpty = new Paragraph("");
            Cell cellEmpty = new Cell();
            cellEmpty.SetBorder(Border.NO_BORDER);


            Cell LegeRegelcell = new Cell(1, 4)
                            .Add(new Paragraph(""))
                            .SetHeight(34)
                            .SetBorder(Border.NO_BORDER);

            Adresstable.AddCell(LegeRegelcell);

            Cell Datumcell = new Cell(1, 3)
                                .Add(new Paragraph(DateTime.Now.ToString()))
                                .SetFont(Segoe)
                                .SetFontSize(10)
                                .SetFontColor(DeviceGray.BLACK)
                                .SetHorizontalAlignment(HorizontalAlignment.RIGHT)
                                .SetVerticalAlignment(VerticalAlignment.BOTTOM)
                                .SetTextAlignment(TextAlignment.RIGHT)
                                .SetBorder(Border.NO_BORDER);

            Adresstable.AddCell(Datumcell);

            Cell Streetcell = new Cell(1, 1)
                    .Add(new Paragraph("Industrielaan"))
                    .SetFont(Segoe)
                    .SetFontSize(12)
                    .SetBold()
                    .SetFontColor(DeviceGray.BLACK)
                    .SetPadding(0)
                    .SetPaddingLeft(5.0f)
                    .SetMinHeight(0)
                    .SetBorder(Border.NO_BORDER);

            Adresstable.AddCell(Streetcell);

            Cell Numbercell = new Cell(1, 1)
                                .Add(new Paragraph("19"))
                                .SetFont(Segoe)
                                .SetFontSize(12)
                                .SetBold()
                                .SetFontColor(DeviceGray.BLACK)
                                .SetPadding(0)
                                .SetTextAlignment(TextAlignment.RIGHT)
                                .SetPaddingRight(15.0f)
                                .SetBorder(Border.NO_BORDER);

            Adresstable.AddCell(Numbercell);

            Cell Dummy1cell = new Cell(1, 3)
                    .Add(new Paragraph(""))
                    .SetBorder(Border.NO_BORDER);

            Adresstable.AddCell(Dummy1cell);


            Cell Postcodecell = new Cell(1, 1)
                    .Add(new Paragraph("8810"))
                    .SetFont(Segoe)
                    .SetFontSize(12)
                    .SetBold()
                    .SetFontColor(DeviceGray.BLACK)
                    .SetPadding(0)
                    .SetPaddingLeft(5.0f)
                    .SetBorder(Border.NO_BORDER);

            Adresstable.AddCell(Postcodecell);

            Cell Citycell = new Cell(1, 1)
                    .Add(new Paragraph("Lichtervelde"))
                    .SetFont(Segoe)
                    .SetFontSize(12)
                    .SetBold()
                    .SetPadding(0)
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetPaddingRight(15.0f)
                    .SetBorder(Border.NO_BORDER);

            Adresstable.AddCell(Citycell);

            Cell Dummy2cell = new Cell(1, 3)
                                 .Add(new Paragraph(""))
                                 .SetBorder(Border.NO_BORDER);

            Adresstable.AddCell(Dummy2cell);

            Cell BTWNrcell = new Cell(1, 1)
                                    .Add(new Paragraph("BE 0455.138.549"))
                                    .SetFont(Segoe)
                                    .SetFontSize(10)
                                    .SetFontColor(DeviceGray.BLACK)
                                    .SetPadding(0)
                                    .SetPaddingLeft(5.0f)
                                    .SetBorder(Border.NO_BORDER);

            Adresstable.AddCell(BTWNrcell);

            Cell Telcell = new Cell(1, 1)
                                .Add(new Paragraph("+32(0)51 72 24 46"))
                                .SetFont(Segoe)
                                .SetFontSize(10)
                                .SetPadding(0)
                                .SetTextAlignment(TextAlignment.RIGHT)
                                .SetPaddingRight(15.0f)
                                .SetBorder(Border.NO_BORDER);

            Adresstable.AddCell(Telcell);

            Cell Dummy3cell = new Cell(1, 1)
                                      .Add(new Paragraph(""))
                                      .SetBorder(Border.NO_BORDER);

            Adresstable.AddCell(Dummy3cell);

            Cell Contactcell = new Cell(1, 1)
                                .Add(new Paragraph("Contact :"))
                                .SetFont(Segoe)
                                .SetFontSize(10)
                                .SetPadding(0)
                                .SetTextAlignment(TextAlignment.RIGHT)
                                .SetPaddingRight(0.0f)
                                .SetBorder(Border.NO_BORDER);

            Adresstable.AddCell(Contactcell);


            Cell ContactEmailcell = new Cell(1, 1)
                    .Add(new Paragraph($"{CurrentnUser.Email}"))
                    .SetFont(Segoe)
                    .SetFontSize(10)
                    .SetPadding(0)
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetPaddingRight(5.0f)
                    .SetBorder(Border.NO_BORDER);

            Adresstable.AddCell(ContactEmailcell);

            Color AstratecGREENColor = new DeviceRgb(21, 94, 103);
            Color AstratecORANGEColor = new DeviceRgb(248, 168, 27);
            Color AstratecGRAYColor = new DeviceRgb(240, 240, 240);

            Cell Websitecell = new Cell(1, 2)
                        .Add(new Paragraph("www.astratec.be"))
                        .SetFont(Segoe)
                        .SetFontSize(12)
                        .SetBold()
                        .SetPadding(0)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontColor(AstratecGREENColor)
                        .SetBorder(Border.NO_BORDER);

            Adresstable.AddCell(Websitecell);

            Cell Dummy4cell = new Cell(1, 2)
                          .Add(new Paragraph(""))
                          .SetBorder(Border.NO_BORDER);

            Adresstable.AddCell(Dummy4cell);

            Cell ContactTelcell = new Cell(1, 1)
                                        .Add(new Paragraph("051 722446 "))
                                        .SetFont(Segoe)
                                        .SetFontSize(10)
                                        .SetPadding(0)
                                        .SetTextAlignment(TextAlignment.RIGHT)
                                        .SetPaddingRight(5.0f)
                                        .SetBorder(Border.NO_BORDER);

            Adresstable.AddCell(ContactTelcell);

            Adresstable.SetMarginBottom(30);

            document.Add(Adresstable);


            Paragraph Bestelbon = new Paragraph("BESTELBON")
                                      .SetFont(Segoe)
                                      .SetFontSize(24)
                                      .SetBold()
                                      .SetMarginLeft(375)
                                      .SetUnderline(1, -4)
                                      .SetMarginBottom(2);
            document.Add(Bestelbon);

            Paragraph Ref = new Paragraph($"Ref : {bestelbon.Name}")
                          .SetFont(Segoe)
                          .SetFontSize(14)
                          .SetBold()
                          .SetTextAlignment(TextAlignment.RIGHT)
                          .SetMarginRight(22)
                          .SetMarginBottom(10);
            document.Add(Ref);

            Paragraph Leverancier = new Paragraph($"Leverancier : {bestelbon.Leverancier.Name}")
                           .SetFont(Segoe)
                           .SetFontSize(12)
                           .SetBold()
                           .SetMarginBottom(20);

            document.Add(Leverancier);

            float[] BestelbontablecolumnWidths = { 0.2f, 0.7f, 1f, 10f, 1.5f, 1.5f };
            Table Bestelbontable = new Table(UnitValue.CreatePercentArray(BestelbontablecolumnWidths)).UseAllAvailableWidth();

            bool colorfill = false;

            for (int i = 0; i < bestelbon.Bestelbonregels.Count; i++)
            {
                if (i % 2 == 0) colorfill = true;
                else colorfill = false;

                // max 12 bestelregels op 1e pagina!!


                Bestelbontable.AddCell(new Cell().Add(new Paragraph(""))
                                                 .SetFont(Segoe)
                                                 .SetFontSize(12)
                                                 .SetPadding(0)
                                               //.SetMaxHeight(19)
                                                 .SetBorder(Border.NO_BORDER)
                                                 .SetMarginTop(0)
                                                 .SetMarginBottom(0)
                                                 .SetBackgroundColor(colorfill ? AstratecGRAYColor : ColorConstants.WHITE)
                                                  );

                Bestelbontable.AddCell(new Cell().Add(new Paragraph(bestelbon.Bestelbonregels[i].Aantal.ToString()))
                                                 .SetFont(Segoe)
                                                 .SetFontSize(12)
                                                 .SetPadding(0)
                                                //.SetMaxHeight(19)
                                                 .SetBorder(Border.NO_BORDER)
                                                 .SetMarginTop(0)
                                                 .SetMarginBottom(0)
                                                 .SetBackgroundColor(colorfill ? AstratecGRAYColor : ColorConstants.WHITE)
                                                                      );

                Bestelbontable.AddCell(new Cell().Add(new Paragraph(bestelbon.Bestelbonregels[i].Eenheid))
                                                 .SetFont(Segoe)
                                                 .SetFontSize(10)
                                                 .SetPaddingTop(2)
                                               //.SetMaxHeight(19)
                                                 .SetBorder(Border.NO_BORDER)
                                                 .SetMarginTop(0)
                                                 .SetMarginBottom(0)
                                                 .SetBackgroundColor(colorfill ? AstratecGRAYColor : ColorConstants.WHITE)
                                                      );


                Bestelbontable.AddCell(new Cell().Add(new Paragraph(bestelbon.Bestelbonregels[i].Bestelregel))
                                                 .SetFont(Segoe)
                                                 .SetFontSize(10)
                                                 .SetPaddingTop(2)
                                               //.SetMaxHeight(19)
                                                 .SetBorder(Border.NO_BORDER)
                                                 .SetMarginTop(0)
                                                 .SetMarginBottom(0)
                                                 .SetBackgroundColor(colorfill ? AstratecGRAYColor : ColorConstants.WHITE)
                                                      );


                Bestelbontable.AddCell(new Cell().Add(new Paragraph(bestelbon.Bestelbonregels[i].Prijs.ToString()))
                                                 .SetFont(Segoe)
                                                 .SetFontSize(12)
                                               //.SetMaxHeight(19)
                                                 .SetPadding(0)
                                                 .SetBorder(Border.NO_BORDER)
                                                 .SetMarginTop(0)
                                                 .SetMarginBottom(0)
                                                 .SetBackgroundColor(colorfill ? AstratecGRAYColor : ColorConstants.WHITE)
                                                      );


                Bestelbontable.AddCell(new Cell().Add(new Paragraph(bestelbon.Bestelbonregels[i].TotalePrijs.ToString()))
                                                 .SetFont(Segoe)
                                                 .SetFontSize(12)
                                                 .SetPadding(0)
                                               //.SetMaxHeight(19)
                                                 .SetBorder(Border.NO_BORDER)
                                                 .SetMarginTop(0)
                                                 .SetMarginBottom(0)
                                                 .SetBackgroundColor(colorfill ? AstratecGRAYColor : ColorConstants.WHITE)
                                                      );

                if (i == 11)
                {
                    document.Add(Bestelbontable);
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                }


            }

            document.Add(Bestelbontable);
            Bestelbontable.SetMarginBottom(20);


            Paragraph TOTAAL = new Paragraph($"TOTAAL :  € {bestelbon.TotalPrice}")
                                  .SetFont(Segoe)
                                  .SetFontSize(16)
                                  .SetBold()
                                  .SetMarginLeft(375)
                                  .SetMarginBottom(10);
            document.Add(TOTAAL);

            Paragraph Opmerking = new Paragraph("Opmerking :")
                                      .SetFont(Segoe)
                                      .SetFontSize(14)
                                      .SetBold()
                                      .SetMarginLeft(3)
                                      .SetMarginBottom(10);

            document.Add(Opmerking);


            Paragraph Opmerkingen = new Paragraph($"{bestelbon.Opmerking} \n")
              .SetFont(Segoe)
              .SetFontSize(10)
              .SetMultipliedLeading(0.9f)
              .SetMarginLeft(3)
              .SetMarginBottom(10);

            document.Add(Opmerkingen);

            Paragraph AutoAddedOpmerkingen = new Paragraph($"Te vermelden bij communicatie : {bestelbon.Name} \n Algemene info en facturatie : boekhouding@astratec.be")
                          .SetFont(Segoe)
                          .SetFontSize(10)
                          .SetMultipliedLeading(0.9f)
                          .SetMarginLeft(3)
                          .SetMarginBottom(10);

            document.Add(AutoAddedOpmerkingen);

            float[] SignaturetablecolumnWidths = { 5f, 1f, 1f, 1f };
            Table Signaturetable = new Table(UnitValue.CreatePercentArray(SignaturetablecolumnWidths)).UseAllAvailableWidth();

            Cell Levvwcell = new Cell(1, 2)
                            .Add(new Paragraph("Leveringsvoorwaarden :"))
                            .SetFont(Segoe)
                            .SetFontSize(14)
                            .SetBold()
                            .SetPadding(0)
                            .SetTextAlignment(TextAlignment.LEFT)
                            .SetPaddingRight(5.0f)
                            .SetBorder(Border.NO_BORDER);

            Signaturetable.AddCell(Levvwcell);

            Cell VoorAkkoordcell = new Cell(1, 2)
                .Add(new Paragraph("Voor Akkoord"))
                .SetFont(Segoe)
                .SetFontSize(14)
                .SetPadding(0)
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetPaddingRight(22.0f)
                .SetBorder(Border.NO_BORDER);

            Signaturetable.AddCell(VoorAkkoordcell);

            Cell Levvwdencell = new Cell(2, 2)
                                   .Add(new Paragraph(bestelbon.Leveringsvoorwaarden).SetMultipliedLeading(0.9f))
                                   .SetFont(Segoe)
                                   .SetFontSize(8)
                                   .SetMarginLeft(3)
                                   .SetBorder(Border.NO_BORDER);
            Signaturetable.AddCell(Levvwdencell);

            Cell LastNamecell = new Cell(1, 1)
                                    .Add(new Paragraph(CurrentnUser.LastName))
                                    .SetFont(Segoe)
                                    .SetFontSize(12)
                                    .SetBold()
                                    .SetPadding(0)
                                    .SetTextAlignment(TextAlignment.RIGHT)
                                    .SetPaddingRight(5.0f)
                                    .SetBorder(Border.NO_BORDER);

            Signaturetable.AddCell(LastNamecell);

            Cell FirstNamecell = new Cell(1, 1)
                                    .Add(new Paragraph(CurrentnUser.FirstName))
                                    .SetFont(Segoe)
                                    .SetFontSize(12)
                                    .SetBold()
                                    .SetPadding(0)
                                    .SetTextAlignment(TextAlignment.LEFT)
                                    .SetPaddingLeft(5.0f)
                                    .SetBorder(Border.NO_BORDER);

            Signaturetable.AddCell(FirstNamecell);

            Image signature = new Image(ImageDataFactory.Create(CurrentnUser.Handtekening));

            Cell sig = new Cell(1, 2)
                           .Add(signature.SetAutoScale(true))
                           .SetBorder(Border.NO_BORDER);

            Signaturetable.AddCell(sig);

            document.Add(Signaturetable);


            #endregion

            document.Close();
            pdf.Close();
            pdfwriter.Dispose();
            pdfwriter.Close();
        }


    }

    //public class Footer : PdfPageEventHelper
    //{
    //    public PdfTemplate total { get; set; }

    //    public override void OnOpenDocument(PdfWriter writer, Document document)
    //    {
    //        base.OnOpenDocument(writer, document);
    //        total = writer.DirectContent.CreateTemplate(30, 16); ; // Width and Height    Creating a template as a placeholder in each Page so we can fill it in at the end !!
    //    }

    //    public override void OnEndPage(PdfWriter writer, Document document)
    //    {

    //        try
    //        {
    //            base.OnEndPage(writer, document);
    //            PdfPTable pagetable = new PdfPTable(3);
    //            pagetable.SetWidths(new float[] { 24f, 24f, 2f });
    //            pagetable.TotalWidth = 527;
    //            pagetable.HorizontalAlignment = Element.ALIGN_CENTER;
    //            pagetable.DefaultCell.FixedHeight = 20f;
    //            pagetable.DefaultCell.Border = PdfPCell.TOP_BORDER;

    //            pagetable.AddCell("");
    //            pagetable.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;

    //            pagetable.AddCell($"Page {writer.PageNumber} of ");
    //            PdfPCell cell = new PdfPCell(Image.GetInstance(total));
    //            cell.Border = PdfPCell.TOP_BORDER;
    //            pagetable.AddCell(cell);
    //            pagetable.WriteSelectedRows(0, -1, 34, 34, writer.DirectContent);
    //        }
    //        catch (Exception)
    //        {

    //            throw;
    //        }

    //    }

    //    public override void OnCloseDocument(PdfWriter writer, Document document)
    //    {
    //        base.OnCloseDocument(writer, document);
    //        ColumnText.ShowTextAligned(total, Element.ALIGN_LEFT, new Phrase((writer.PageNumber).ToString()), 2, 2, 0);
    //    }




}
