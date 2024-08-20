using System.ComponentModel.DataAnnotations;

namespace DiegoMoyanoProject.ViewModels.UserPdf
{
    public class UploadDatePDFViewModel
    {

        [Display(Name = "Fecha")]
        public DateTime Date { get; set; }

        public UploadDatePDFViewModel()
        {
        }
        public UploadDatePDFViewModel(DateTime date)
        {
            this.Date = date;
        } 
        public UploadDatePDFViewModel(string date)
        {
            DateTime dateTime;
            if (DateTime.TryParse(date, out dateTime))
            { 
            this.Date = dateTime; 
            }
        }
    }
}
