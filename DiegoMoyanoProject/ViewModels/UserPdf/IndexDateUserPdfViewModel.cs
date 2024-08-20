using DiegoMoyanoProject.Models;

namespace DiegoMoyanoProject.ViewModels.UserPdf
{
    public class IndexDateUserPdfViewModel
    {
        public byte[]? Pdf { get; set; }
        public List<FileDate> Dates { get; set; }
        public string? SelectedDate { get; set; }

        public IndexDateUserPdfViewModel() { }

        public IndexDateUserPdfViewModel(byte[]? pdf, List<FileDate> dates, DateTime selectedDate)
        {
            Pdf = pdf;
            Dates = dates;
            SelectedDate = selectedDate.ToString("MMMM yyyy");
        }

    }
}
