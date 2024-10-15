using ProcessScreenCaptureOCR;

namespace MenuScraper;

public partial class Form1 : Form
{
    Scraper scraper;
    public Form1()
    {
        InitializeComponent();
        //ProcessScreenCaptureOCR.Program2.Scraper();
        scraper = new Scraper();
    }

    private void buttonScan_Click(object sender, EventArgs e)
    {
        scraper.Scrape();
        this.pictureBox1.Image = (Image )scraper.CapturE;
        var text = scraper.Ocr.Replace("\n\n", "\n");
        this.textBox1.Text = String.Join("\r\n", text.Split('\n'));
    }
}
