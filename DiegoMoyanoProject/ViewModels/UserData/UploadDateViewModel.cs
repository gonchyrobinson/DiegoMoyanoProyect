using System.ComponentModel.DataAnnotations;

namespace DiegoMoyanoProject.ViewModels.UserData
{
    public class UploadDateViewModel
    {

        [Display(Name = "Fecha")]
        public DateTime Date { get; set; }

        public UploadDateViewModel()
        {
        }
        public UploadDateViewModel(DateTime date)
        {
            this.Date = date;
        } 
        public UploadDateViewModel(string date)
        {
            DateTime dateTime;
            if (DateTime.TryParse(date, out dateTime))
            { 
            this.Date = dateTime; 
            }
        }
    }
}
