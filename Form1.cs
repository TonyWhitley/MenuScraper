namespace MenuScraper;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        ProcessScreenCaptureOCR.Program2.Scraper();
    }
}
