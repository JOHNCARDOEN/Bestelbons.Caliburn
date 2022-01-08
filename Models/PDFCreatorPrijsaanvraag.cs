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
    public class PDFCreatorPrijsaanvraag
    {
        public string ProjectDirectory { get; set; }


        public void Create(Prijsvraag prijsvraag, User CurrentnUser, string FilePath)
        {

            string dest = FilePath;
            FileInfo file = new FileInfo(dest);
            if (!file.Directory.Exists) file.Directory.Create();

            PdfWriter pdfwriter = new PdfWriter(dest);
            PdfDocument pdf = new PdfDocument(pdfwriter);
            Document document = new Document(pdf, PageSize.A4);

            //PdfFontFactory.Register("c:/windows/fonts/Segoe UI.ttf", "SegoeUI");

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


            Paragraph Bestelbon = new Paragraph("PRIJSVRAAG")
                                      .SetFont(Segoe)
                                      .SetFontSize(24)
                                      .SetBold()
                                      .SetMarginLeft(375)
                                      .SetUnderline(1, -4)
                                      .SetMarginBottom(2);
            document.Add(Bestelbon);

            Paragraph Ref = new Paragraph($"Ref : {prijsvraag.Name}")
                          .SetFont(Segoe)
                          .SetFontSize(14)
                          .SetBold()
                          .SetTextAlignment(TextAlignment.RIGHT)
                          .SetMarginRight(22)
                          .SetMarginBottom(10);
            document.Add(Ref);

            Paragraph Leverancier = new Paragraph($"Leverancier : {prijsvraag.Leverancier.Name}")
                           .SetFont(Segoe)
                           .SetFontSize(12)
                           .SetBold()
                           .SetMarginBottom(20);

            document.Add(Leverancier);

            float[] BestelbontablecolumnWidths = { 0.2f, 0.7f, 1f, 10f, 1.5f, 1.5f };
            Table Bestelbontable = new Table(UnitValue.CreatePercentArray(BestelbontablecolumnWidths)).UseAllAvailableWidth();

            bool colorfill = false;

            for (int i = 0; i < prijsvraag.Prijsvraagregels.Count; i++)
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

                Bestelbontable.AddCell(new Cell().Add(new Paragraph(prijsvraag.Prijsvraagregels[i].Aantal.ToString()))
                                                 .SetFont(Segoe)
                                                 .SetFontSize(12)
                                                 .SetPadding(0)
                                               //.SetMaxHeight(19)
                                                 .SetBorder(Border.NO_BORDER)
                                                 .SetMarginTop(0)
                                                 .SetMarginBottom(0)
                                                 .SetBackgroundColor(colorfill ? AstratecGRAYColor : ColorConstants.WHITE)
                                                                      );

                Bestelbontable.AddCell(new Cell().Add(new Paragraph(prijsvraag.Prijsvraagregels[i].Eenheid))
                                                 .SetFont(Segoe)
                                                 .SetFontSize(10)
                                                 .SetPaddingTop(2)
                                               //.SetMaxHeight(19)
                                                 .SetBorder(Border.NO_BORDER)
                                                 .SetMarginTop(0)
                                                 .SetMarginBottom(0)
                                                 .SetBackgroundColor(colorfill ? AstratecGRAYColor : ColorConstants.WHITE)
                                                      );


                Bestelbontable.AddCell(new Cell().Add(new Paragraph(prijsvraag.Prijsvraagregels[i].Prijsregel))
                                                 .SetFont(Segoe)
                                                 .SetFontSize(10)
                                                 .SetPaddingTop(2)
                                               //.SetMaxHeight(19)
                                                 .SetBorder(Border.NO_BORDER)
                                                 .SetMarginTop(0)
                                                 .SetMarginBottom(0)
                                                 .SetBackgroundColor(colorfill ? AstratecGRAYColor : ColorConstants.WHITE)
                                                      );


                //Bestelbontable.AddCell(new Cell().Add(new Paragraph(prijsvraag.Prijsvraagregels[i].Prijs.ToString()))
                //                                 .SetFont(Segoe)
                //                                 .SetFontSize(12)
                //                                 .SetMaxHeight(19)
                //                                 .SetPadding(0)
                //                                 .SetBorder(Border.NO_BORDER)
                //                                 .SetMarginTop(0)
                //                                 .SetMarginBottom(0)
                //                                 .SetBackgroundColor(colorfill ? AstratecGRAYColor : ColorConstants.WHITE)
                //                                      );

                Bestelbontable.AddCell(new Cell().Add(new Paragraph(" "))
                                 .SetFont(Segoe)
                                 .SetFontSize(12)
                                 .SetMaxHeight(19)
                                 .SetPadding(0)
                                 .SetBorder(Border.NO_BORDER)
                                 .SetMarginTop(0)
                                 .SetMarginBottom(0)
                                 .SetBackgroundColor(colorfill ? AstratecGRAYColor : ColorConstants.WHITE)
                                      );

                //Bestelbontable.AddCell(new Cell().Add(new Paragraph(prijsvraag.Prijsvraagregels[i].TotalePrijs.ToString()))
                //                                 .SetFont(Segoe)
                //                                 .SetFontSize(12)
                //                                 .SetPadding(0)
                //                                 .SetMaxHeight(19)
                //                                 .SetBorder(Border.NO_BORDER)
                //                                 .SetMarginTop(0)
                //                                 .SetMarginBottom(0)
                //                                 .SetBackgroundColor(colorfill ? AstratecGRAYColor : ColorConstants.WHITE)
                //                                      );

                Bestelbontable.AddCell(new Cell().Add(new Paragraph(""))
                                 .SetFont(Segoe)
                                 .SetFontSize(12)
                                 .SetPadding(0)
                                 .SetMaxHeight(19)
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


            //Paragraph TOTAAL = new Paragraph($"TOTAAL :  € {prijsvraag.TotalPrice}")
            //                      .SetFont(Segoe)
            //                      .SetFontSize(16)
            //                      .SetBold()
            //                      .SetMarginLeft(375)
            //                      .SetMarginBottom(10);
            //document.Add(TOTAAL);


            Paragraph TOTAAL = new Paragraph(" ")
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


            Paragraph Opmerkingen = new Paragraph($"{prijsvraag.Opmerking} \n")
              .SetFont(Segoe)
              .SetFontSize(10)
              .SetMultipliedLeading(0.9f)
              .SetMarginLeft(3)
              .SetMarginBottom(10);

            document.Add(Opmerkingen);

            Paragraph AutoAddedOpmerkingen = new Paragraph($"Te vermelden bij communicatie : {prijsvraag.Name} \n Algemene info en facturatie : boekhouding@astratec.be")
                          .SetFont(Segoe)
                          .SetFontSize(10)
                          .SetMultipliedLeading(0.9f)
                          .SetMarginLeft(3)
                          .SetMarginBottom(10);

            document.Add(AutoAddedOpmerkingen);


            float[] SignaturetablecolumnWidths = { 5f, 1f, 1f, 1f };
            Table Signaturetable = new Table(UnitValue.CreatePercentArray(SignaturetablecolumnWidths)).UseAllAvailableWidth();

            //Cell Levvwcell = new Cell(1, 2)
            //                .Add(new Paragraph("Leveringsvoorwaarden :"))
            //                .SetFont(Segoe)
            //                .SetFontSize(14)
            //                .SetBold()
            //                .SetPadding(0)
            //                .SetTextAlignment(TextAlignment.LEFT)
            //                .SetPaddingRight(5.0f)
            //                .SetBorder(Border.NO_BORDER);

            //Signaturetable.AddCell(Levvwcell);

            Cell Levvwcell = new Cell(1, 2)
                .Add(new Paragraph(""))
                .SetFont(Segoe)
                .SetFontSize(14)
                .SetBold()
                .SetPadding(0)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetPaddingRight(5.0f)
                .SetBorder(Border.NO_BORDER);

            Signaturetable.AddCell(Levvwcell);

            //Cell VoorAkkoordcell = new Cell(1, 2)
            //    .Add(new Paragraph("Voor Akkoord"))
            //    .SetFont(Segoe)
            //.SetFontSize(14)
            //.SetPadding(0)
            //.SetTextAlignment(TextAlignment.RIGHT)
            //    .SetPaddingRight(22.0f)
            //    .SetBorder(Border.NO_BORDER);

            //Signaturetable.AddCell(VoorAkkoordcell);

            Cell VoorAkkoordcell = new Cell(1, 2)
                 .Add(new Paragraph(""))
                 .SetFont(Segoe)
                 .SetFontSize(14)
                 .SetPadding(0)
                 .SetTextAlignment(TextAlignment.RIGHT)
                 .SetPaddingRight(22.0f)
                 .SetBorder(Border.NO_BORDER);

            Signaturetable.AddCell(VoorAkkoordcell);

            //Cell Levvwdencell = new Cell(2, 2)
            //                       .Add(new Paragraph(prijsvraag.Leveringsvoorwaarden).SetMultipliedLeading(0.9f))
            //                       .SetFont(Segoe)
            //                       .SetFontSize(8)
            //                       .SetMarginLeft(3)
            //                       .SetBorder(Border.NO_BORDER);
            //Signaturetable.AddCell(Levvwdencell);


            Cell Levvwdencell = new Cell(2, 2)
                       .Add(new Paragraph(" "))
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

        //        Document document = new Document(PageSize.A4, 25, 25, 30, 30);   // Margin Left, Margin Right, Margin Top , Margin Bottom
        //        PdfWriter writer = PdfWriter.GetInstance(document, fs);
        //        writer.PageEvent = new Footer();
        //        document.Open();
        //        PageFillLevel = 0.0f;

        //        FontFactory.Register("c:/windows/fonts/SegoeUI.ttf", "SegoeUI");

        //        Font Segoe08 = FontFactory.GetFont("SegoeUI", 8F);
        //        Font Segoe10 = FontFactory.GetFont("SegoeUI", 10F);
        //        Font Segoe12 = FontFactory.GetFont("SegoeUI", 12F);
        //        Font Segoe14 = FontFactory.GetFont("SegoeUI", 14F);
        //        Font Segoe16 = FontFactory.GetFont("SegoeUI", 16F);
        //        Font Segoe18 = FontFactory.GetFont("SegoeUI", 18F);
        //        Font Segoe20 = FontFactory.GetFont("SegoeUI", 20F);
        //        Font Segoe24 = FontFactory.GetFont("SegoeUI", 24F);
        //        Font Segoe32 = FontFactory.GetFont("SegoeUI", 32F);
        //        Font Segoe36 = FontFactory.GetFont("SegoeUI", 36F);
        //        Font Segoe72 = FontFactory.GetFont("SegoeUI", 72F);
        //        BaseColor LightGray = new BaseColor(192, 192, 192, 64);

        //        #region FIRST LAYER


        //        Image img = Image.GetInstance(Properties.Settings.Default.DataDirectory +"/LOGO.png");



        //        //Image img = Image.GetInstance("F:/Desktop/BestelbonsDATA/LOGOORANGE.png");
        //        img.ScaleToFit(150, 150);
        //        img.SetAbsolutePosition(55.0f, 770.0f);
        //        document.Add(img);

        //        #endregion
        //        #region SECOND LAYER

        //        // ADRESS REGION ASTRATEC

        //        PdfPTable Adresstable = new PdfPTable(5);
        //        Adresstable.DefaultCell.Border = PdfPCell.RIGHT_BORDER;
        //        Adresstable.SetWidths(new float[] { 1.5f, 1f, 1f, 1f, 1.7f });
        //        Adresstable.WidthPercentage = 100;
        //        Adresstable.HorizontalAlignment = Element.ALIGN_LEFT;

        //        Paragraph pEmpty = new Paragraph("", Segoe14);
        //        PdfPCell cellEmpty = new PdfPCell(pEmpty);
        //        cellEmpty.Border = PdfPCell.NO_BORDER;


        //        Paragraph pLegeRegel1 = new Paragraph("  ", Segoe32);
        //        PdfPCell cellLegeRegel1 = new PdfPCell(pLegeRegel1);
        //        cellLegeRegel1.Colspan = 5;
        //        cellLegeRegel1.Border = PdfPCell.NO_BORDER;
        //        Adresstable.AddCell(cellLegeRegel1);

        //        Paragraph pFirma = new Paragraph("", Segoe36);
        //        PdfPCell cellFirma = new PdfPCell(pFirma);
        //        cellFirma.Colspan = 3;
        //        cellFirma.Border = PdfPCell.NO_BORDER;
        //        Adresstable.AddCell(cellFirma);

        //        Adresstable.AddCell(cellEmpty);
        //        Paragraph pDatum = new Paragraph(DateTime.Now.ToString(), Segoe10);
        //        PdfPCell datumCell = new PdfPCell(pDatum);
        //        datumCell.Border = PdfPCell.NO_BORDER;
        //        datumCell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //        datumCell.VerticalAlignment = Element.ALIGN_BOTTOM;
        //        Adresstable.AddCell(datumCell);

        //        Paragraph pStraat = new Paragraph("Industrielaan", Segoe12);
        //        PdfPCell cellStraat = new PdfPCell(pStraat);
        //        cellStraat.Border = PdfPCell.NO_BORDER;
        //        cellStraat.HorizontalAlignment = Element.ALIGN_LEFT;
        //        Adresstable.AddCell(cellStraat);

        //        Paragraph pNummer = new Paragraph("19", Segoe12);
        //        PdfPCell cellNummer = new PdfPCell(pNummer);
        //        cellNummer.Border = PdfPCell.NO_BORDER;
        //        cellNummer.HorizontalAlignment = Element.ALIGN_RIGHT;
        //        Adresstable.AddCell(cellNummer);

        //        Adresstable.AddCell(cellEmpty);
        //        Adresstable.AddCell(cellEmpty);
        //        Adresstable.AddCell(cellEmpty);

        //        Paragraph pPostCode = new Paragraph("8810", Segoe12);
        //        PdfPCell cellPostCode = new PdfPCell(pPostCode);
        //        cellPostCode.Border = PdfPCell.NO_BORDER;
        //        cellPostCode.HorizontalAlignment = Element.ALIGN_LEFT;
        //        Adresstable.AddCell(cellPostCode);

        //        Paragraph pStad = new Paragraph("Lichtervelde", Segoe12);
        //        PdfPCell cellStad = new PdfPCell(pStad);
        //        cellStad.Border = PdfPCell.NO_BORDER;
        //        cellStad.HorizontalAlignment = Element.ALIGN_RIGHT;
        //        Adresstable.AddCell(cellStad);

        //        Adresstable.AddCell(cellEmpty);
        //        Adresstable.AddCell(cellEmpty);
        //        Adresstable.AddCell(cellEmpty);

        //        Paragraph pBTWnr = new Paragraph("BE 0455.138.549", Segoe12);
        //        PdfPCell cellBTWnr = new PdfPCell(pBTWnr);
        //        cellBTWnr.Border = PdfPCell.NO_BORDER;
        //        cellBTWnr.HorizontalAlignment = Element.ALIGN_LEFT;
        //        Adresstable.AddCell(cellBTWnr);

        //        Adresstable.AddCell(cellEmpty);
        //        Adresstable.AddCell(cellEmpty);
        //        Adresstable.AddCell(cellEmpty);
        //        Paragraph pContact = new Paragraph("Contact :    ", Segoe12);
        //        PdfPCell contactCell = new PdfPCell(pContact);
        //        contactCell.Border = PdfPCell.NO_BORDER;
        //        contactCell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //        Adresstable.AddCell(contactCell);

        //        Paragraph pTEL = new Paragraph("+32 (0) 51 72 24 46", Segoe12);
        //        PdfPCell cellTEL = new PdfPCell(pTEL);
        //        cellTEL.Border = PdfPCell.NO_BORDER;
        //        cellTEL.HorizontalAlignment = Element.ALIGN_LEFT;
        //        Adresstable.AddCell(cellTEL);

        //        Adresstable.AddCell(cellEmpty);
        //        Adresstable.AddCell(cellEmpty);
        //        Adresstable.AddCell(cellEmpty);
        //        Paragraph pUser = new Paragraph(CurrentnUser.Email, Segoe12);
        //        PdfPCell userCell = new PdfPCell(pUser);
        //        userCell.Border = PdfPCell.NO_BORDER;
        //        userCell.Colspan = 2;
        //        userCell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //        Adresstable.AddCell(userCell);

        //        Chunk chunkWWW = new Chunk("www.astratec.be");
        //        chunkWWW.SetAnchor("http://www.astratec.be");
        //        Paragraph pWWW = new Paragraph();
        //        pWWW.Add(chunkWWW);
        //        PdfPCell cellWWW = new PdfPCell(pWWW);
        //        cellWWW.Colspan = 2;
        //        cellWWW.Border = PdfPCell.NO_BORDER;
        //        cellWWW.HorizontalAlignment = Element.ALIGN_LEFT;
        //        Adresstable.AddCell(cellWWW);

        //        Adresstable.AddCell(cellEmpty);
        //        Adresstable.AddCell(cellEmpty);
        //        Paragraph pUsertel = new Paragraph(CurrentnUser.Tel, Segoe12);
        //        PdfPCell usertelCell = new PdfPCell(pUsertel);
        //        usertelCell.Border = PdfPCell.NO_BORDER;
        //        usertelCell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //        Adresstable.AddCell(usertelCell);
        //        PageFillLevel += CalculatePdfPTableHeight(Adresstable);


        //        Adresstable.SpacingBefore = 15f;
        //        Adresstable.SpacingAfter = 30f;
        //        PageFillLevel += 45;            // 15 + 30 as the previous set SpacingBefore and SpacingAfter


        //        document.Add(Adresstable);

        //        Paragraph pBESTELBON = new Paragraph("                                                           PRIJSVRAAG", Segoe24);
        //        pBESTELBON.SpacingAfter = 10f;
        //        PageFillLevel += 10;            // 15 + 30 as the previous set SpacingAfter
        //        document.Add(pBESTELBON);


        //        document.Add(new Paragraph("Leverancier : " + prijsvraag.Leverancier.Name.ToString(), Segoe12));

        //        //float[] widthstable = { 1.5f, 1f, 15f, 2f, 2f };
        //        float[] widthstable = { 1.5f, 1f, 15f };
        //        PdfPTable table = new PdfPTable(widthstable);
        //        table.DefaultCell.Border = PdfPCell.NO_BORDER;
        //        table.WidthPercentage = 100;
        //        int j = 0;
        //        bool grayfill = false;

        //        foreach (Prijsvraagregel regel in prijsvraag.Prijsvraagregels)
        //        {
        //            if (j % 2 == 0) grayfill = true;
        //            else grayfill = false;

        //            Paragraph pAantal = new Paragraph(regel.Aantal.ToString(), Segoe12);
        //            PdfPCell cellAantal = new PdfPCell(pAantal);
        //            cellAantal.Border = PdfPCell.NO_BORDER;
        //            cellAantal.FixedHeight = 16f;
        //            if (grayfill) cellAantal.BackgroundColor = LightGray;
        //            table.AddCell(cellAantal);

        //            Paragraph pEenheid = new Paragraph(regel.Eenheid.ToString(), Segoe08);
        //            PdfPCell cellEenheid = new PdfPCell(pEenheid);
        //            cellEenheid.Border = PdfPCell.NO_BORDER;
        //            cellEenheid.PaddingTop = 7f;
        //            cellEenheid.VerticalAlignment = Element.ALIGN_TOP;
        //            if (grayfill) cellEenheid.BackgroundColor = LightGray;
        //            table.AddCell(cellEenheid);

        //            Paragraph pBestelregel = new Paragraph(regel.Prijsregel.ToString(), Segoe12);
        //            PdfPCell cellBestelregel = new PdfPCell(pBestelregel);
        //            cellBestelregel.Border = PdfPCell.NO_BORDER;
        //            if (grayfill) cellBestelregel.BackgroundColor = LightGray;
        //            table.AddCell(cellBestelregel);

        //            //Paragraph pPrice = new Paragraph(regel.Prijs.ToString("0000.00").TrimStart('0'), Segoe12);
        //            //PdfPCell cellPrice = new PdfPCell(pPrice);
        //            //cellPrice.HorizontalAlignment = Element.ALIGN_RIGHT;
        //            //cellPrice.Border = PdfPCell.NO_BORDER;
        //            //if (grayfill) cellPrice.BackgroundColor = LightGray;
        //            //table.AddCell(cellEmpty);

        //            //Paragraph pTotalPrice = new Paragraph(regel.TotalePrijs.ToString("0000.00").TrimStart('0'), Segoe12);
        //            //PdfPCell cellTotalPrice = new PdfPCell(pTotalPrice);
        //            //cellTotalPrice.HorizontalAlignment = Element.ALIGN_RIGHT;
        //            //cellTotalPrice.Border = PdfPCell.NO_BORDER;
        //            //if (grayfill) cellTotalPrice.BackgroundColor = LightGray;
        //            //table.AddCell(cellEmpty);

        //            j++;
        //            PageFillLevel += 16;  //   cellAantal.FixedHeight = 16f;
        //            //if (PageFillLevel >= 872 - 225 - 60 - 60)         // 2e 60 is Top and bottom margin of Page !!
        //            //{
        //            //    table.SpacingBefore = 30f;
        //            //    document.Add(table);
        //            //    document.NewPage();                          // Opmerkingen en Handtekening tabellen zijn 225 hoog + A4 size is 872 !! + 60 spacing after en before !
        //            //    PageFillLevel = 0;
        //            //    table.DeleteBodyRows();
        //            //}


        //        }


        //        table.SpacingBefore = 30f;
        //        PageFillLevel += 30;
        //        table.SpacingAfter = 50f;
        //        document.Add(table);
        //        PageFillLevel += 30;           

        //        //document.Add(table);
        //        //Paragraph pTotaal = new Paragraph("                                                                                                           TOTAAL :    " + bestelbon.TotalPrice.ToString("0000.00").TrimStart('0'), Segoe14);
        //        //pTotaal.SpacingAfter = 20f;
        //        //PageFillLevel += 34;            // 20 as the previous set SpacingAfter + 14 for the TOTAAL string
        //        //document.Add(pTotaal);

        //        // TABLE MET OPMERKINGEN

        //        PdfPTable Opmerkingentable = new PdfPTable(2);
        //        Opmerkingentable.DefaultCell.Border = PdfPCell.NO_BORDER;
        //        Opmerkingentable.SetWidths(new float[] { 30.0f, 0.5f });
        //        Opmerkingentable.WidthPercentage = 100;
        //        Opmerkingentable.HorizontalAlignment = Element.ALIGN_CENTER;


        //        Paragraph pOpmerkingText = new Paragraph("Opmerking", Segoe12);
        //        PdfPCell celllev1 = new PdfPCell(pOpmerkingText);
        //        celllev1.FixedHeight = 20f;
        //        celllev1.HorizontalAlignment = Element.ALIGN_LEFT;
        //        celllev1.Border = PdfPCell.NO_BORDER;
        //        Opmerkingentable.AddCell(celllev1);

        //        Opmerkingentable.AddCell(cellEmpty);

        //        Paragraph pOpmerking = new Paragraph(prijsvraag.Opmerking, Segoe08);
        //        PdfPCell celllev2 = new PdfPCell(pOpmerking);
        //        celllev2.NoWrap = false;
        //        celllev2.HorizontalAlignment = Element.ALIGN_LEFT;
        //        celllev2.Border = PdfPCell.NO_BORDER;
        //        Opmerkingentable.AddCell(celllev2);

        //        Opmerkingentable.AddCell(cellEmpty);



        //        //Paragraph pAutoOpmerking = new Paragraph(prijsvraag.AutoAddedOpmerking, FontFactory.GetFont("Segoe", 08, Font.BOLD, new Color(252, 3, 252)));
        //        //PdfPCell celllev3 = new PdfPCell(pAutoOpmerking);
        //        //celllev3.FixedHeight = 20f;
        //        //celllev3.HorizontalAlignment = Element.ALIGN_LEFT;
        //        //celllev3.Border = PdfPCell.NO_BORDER;
        //        Opmerkingentable.AddCell(cellEmpty);

        //        Opmerkingentable.AddCell("");

        //        Opmerkingentable.SpacingAfter = 10f;
        //        PageFillLevel += 10;            // 10 as the previous set SpacingAfter

        //        document.Add(Opmerkingentable);

        //        PageFillLevel += CalculatePdfPTableHeight(Opmerkingentable);

        //        if (PageFillLevel >= 872 - 115 - 60)  // 60 is Top and bottom margin of Page !!
        //        {
        //            document.NewPage();        // 115 = height of handtekeningenTABLE !!
        //            PageFillLevel = 0;
        //        }




        //        // TABLE MET LEVERINGSVOORWAARDEN EN HANDTEKENING

        //        PdfPTable Leveringsvwdntable = new PdfPTable(3);
        //        Leveringsvwdntable.DefaultCell.Border = PdfPCell.NO_BORDER;
        //        Leveringsvwdntable.SetWidths(new float[] { 3.0f, 0.5f, 0.5f });
        //        Leveringsvwdntable.WidthPercentage = 100;
        //        Leveringsvwdntable.HorizontalAlignment = Element.ALIGN_CENTER;

        //        //Paragraph pLeveringsvoorwaardenText = new Paragraph("Leveringsvoorwaarden", Segoe12);
        //        //PdfPCell cell1 = new PdfPCell(pLeveringsvoorwaardenText);
        //        //cell1.Border = PdfPCell.NO_BORDER;
        //        //cell1.FixedHeight = 20f;
        //        //cell1.HorizontalAlignment = Element.ALIGN_LEFT;
        //        Leveringsvwdntable.AddCell(cellEmpty);

        //        Paragraph pVoorAkkoordText = new Paragraph("Voor Akkoord", Segoe12);
        //        PdfPCell cell2 = new PdfPCell(pVoorAkkoordText);
        //        cell2.Colspan = 2;
        //        cell2.Border = PdfPCell.NO_BORDER;
        //        cell2.FixedHeight = 20f;
        //        cell2.HorizontalAlignment = Element.ALIGN_CENTER;
        //        Leveringsvwdntable.AddCell(cell2);

        //        //Paragraph pLeveeringsvoorwaarden = new Paragraph(prijsvraag.Leveringsvoorwaarden, Segoe08);
        //        //PdfPCell cell3 = new PdfPCell(pLeveeringsvoorwaarden);
        //        //cell3.Rowspan = 2;
        //        //cell3.Border = PdfPCell.NO_BORDER;
        //        //cell3.HorizontalAlignment = Element.ALIGN_LEFT;
        //        Leveringsvwdntable.AddCell(cellEmpty);


        //        Paragraph pLastName = new Paragraph(CurrentnUser.LastName, Segoe10);
        //        PdfPCell cell4 = new PdfPCell(pLastName);
        //        cell4.Border = PdfPCell.NO_BORDER;
        //        cell4.HorizontalAlignment = Element.ALIGN_CENTER;
        //        cell4.FixedHeight = 20f;
        //        Leveringsvwdntable.AddCell(cell4);

        //        Paragraph pFirstName = new Paragraph(CurrentnUser.FirstName, Segoe10);
        //        PdfPCell cell5 = new PdfPCell(pFirstName);
        //        cell5.Border = PdfPCell.NO_BORDER;
        //        cell5.HorizontalAlignment = Element.ALIGN_CENTER;
        //        cell5.FixedHeight = 20f;
        //        Leveringsvwdntable.AddCell(cell5);

        //        Leveringsvwdntable.AddCell(cellEmpty);


        //        Image sigimage = Image.GetInstance(CurrentnUser.Handtekening);
        //        sigimage.ScaleToFit(75.0f, 75.0f);
        //        PdfPCell sig = new PdfPCell(sigimage);
        //        sig.Border = PdfPCell.NO_BORDER;
        //        sig.Colspan = 2;
        //        sig.HorizontalAlignment = Element.ALIGN_CENTER;
        //        Leveringsvwdntable.AddCell(sig);


        //        document.Add(Leveringsvwdntable);
        //        PageFillLevel += CalculatePdfPTableHeight(Leveringsvwdntable);

        //        int NumberOfPages = writer.PageNumber;

        //        #endregion
        //        document.Close();
        //        writer.Close();

        //    }

        //}




    }
}
