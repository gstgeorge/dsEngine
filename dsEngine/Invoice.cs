using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.IO;

namespace dsEngine
{
    internal static class Invoice
    {
        private const double MARGIN = 30;
        private const double SPACER = 15;
        private const double LINE_HEIGHT = 12;
        private static double YPos;

        public static void Generate(Dealer dealer, string writePath)
        {
            YPos = MARGIN;
            
            // Initialize new document
            PdfDocument doc = new PdfDocument();
            doc.Info.Title = "title"; // TODO: change this

            PdfPage page = doc.AddPage();
            page.Size = PdfSharp.PageSize.Letter; // TODO: make this config'able

            XGraphics gfx = XGraphics.FromPdfPage(page);

            XFont bodyFont = new XFont("Arial", 9);

            YPos = PrintHeader(gfx);



            doc.Save(writePath);
        }

        /// <summary>
        /// Prints the page header.
        /// </summary>
        /// <param name="gfx">The drawing surface for the page.</param>
        /// <param name="isWorkSummary">Print the title as "Work Summary" instead of "Invoice".</param>
        /// <returns></returns>
        private static double PrintHeader(XGraphics gfx, bool isWorkSummary=false)
        {
            double headerHeight = 60;
            double headerLineHeight = headerHeight / 5; // max 5 lines next to logo
            double contactInfoStartX = MARGIN;

            XFont font = new XFont("Arial", 9);

            // If a company logo has been set, display that image.
            // Set the X Position of the company contact info to the right of the logo.
            if (File.Exists(Settings.Config.LogoPath))
            {
                XImage logo = XImage.FromFile(Settings.Config.LogoPath);

                double aspectRatio = logo.Size.Width / logo.Size.Height;

                double maxLogoWidth = 150; // Max possible width of logo

                // Scale the image to fit the area
                double logoWidth = aspectRatio >= 1.0f ? maxLogoWidth : headerHeight * aspectRatio;
                double logoHeight = aspectRatio < 1.0f ? headerHeight : maxLogoWidth / aspectRatio;

                gfx.DrawImage(logo, MARGIN, MARGIN + (headerHeight - logoHeight) / 2, logoWidth, logoHeight);

                // Move X position of contact info to the right of the logo.
                contactInfoStartX = MARGIN + logoWidth + SPACER;

                // Center the contact info vertically with the logo.
                YPos += (headerHeight - (Settings.Config.CompanyInfo.Length * headerLineHeight)) / 2;
            }


            // Print each line of company contact info to the right of the logo.
            foreach (string s in Settings.Config.CompanyInfo)
            {
                gfx.DrawString(s, font, XBrushes.Black,
                    new XRect(contactInfoStartX, YPos, 150, headerLineHeight), XStringFormats.CenterLeft);

                YPos += LINE_HEIGHT;
            }


            // Print an appropiate page title based on whether this is a Work Summary or an Invoice.
            double titleWidth = 250;
            double titleHeight = headerHeight;

            double XPos = gfx.PdfPage.Width - MARGIN - titleWidth;
            YPos = MARGIN;

            XFont titleFont;
            XBrush titleBrush = new XSolidBrush(
                XColor.FromArgb(
                    Settings.Config.InvoiceAccentColor.R, 
                    Settings.Config.InvoiceAccentColor.G, 
                    Settings.Config.InvoiceAccentColor.B
                    ));
            
            if (isWorkSummary)
            {
                titleHeight = headerHeight / 2 - headerLineHeight;

                titleFont = new XFont("Arial", 28, XFontStyle.Bold);

                gfx.DrawString("WORK", titleFont, titleBrush,
                    new XRect(XPos, YPos, titleWidth, titleHeight), XStringFormats.CenterRight);
                
                YPos += titleHeight + headerLineHeight;

                gfx.DrawString("SUMMARY", titleFont, titleBrush,
                    new XRect(XPos, YPos, titleWidth, titleHeight), XStringFormats.CenterRight);
            }

            else
            {
                titleFont = new XFont("Arial", 38, XFontStyle.Bold);

                gfx.DrawString("INVOICE", titleFont, titleBrush,
                    new XRect(XPos, YPos, titleWidth, titleHeight), XStringFormats.CenterRight);
            }


            // Return the Y Position to start printing at below the header.
            return headerHeight + MARGIN + SPACER;
        }
    }
}
