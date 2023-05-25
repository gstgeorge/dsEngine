using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace dsEngine
{
    internal static class Invoice
    {
        public static void Generate(Dealer dealer, string writePath)
        {
            const double MARGIN = 30;

            // Initialize new document
            PdfDocument doc = new PdfDocument();
            doc.Info.Title = "title"; // TODO: change this

            PdfPage page = doc.AddPage();
            page.Size = PdfSharp.PageSize.Letter;
            XGraphics gfx = XGraphics.FromPdfPage(page);


            // Draw company logo
            XImage logo = XImage.FromFile(Settings.UserConfig.LogoFilename);

            double aspectRatio = logo.Size.Width / logo.Size.Height; // Get aspect ratio of logo

            double maxLogoWidth = 150; // Max possible width of logo
            double maxLogoHeight = 50; // Max possible height of logo

            double logoWidth = aspectRatio > 1.0f ? maxLogoWidth : maxLogoHeight / aspectRatio;
            double logoHeight = aspectRatio <= 1.0f ? maxLogoHeight : maxLogoWidth / aspectRatio;
            
            gfx.DrawImage(logo, MARGIN, MARGIN, logoWidth, logoHeight);

            // Draw company contact info








            doc.Save(writePath);
        }
    }
}
