﻿using DiegoMoyanoProject.Models;
using System.ComponentModel.DataAnnotations;

namespace DiegoMoyanoProject.ViewModels.UserData
{
    public class UpdateImageFormViewModel
    {

        [Required]
        [Display(Name="Tipo de Imagen")]
        public ImageType ImageType { get; set; }
        [Display(Name = "Imagen")]
        public IFormFile? InputFile { get; set; }
        [Display(Name = "Fecha")]
        public DateTime Date { get; set; }
        public UpdateImageFormViewModel()
        {
        }

        public UpdateImageFormViewModel(ImageType imageType, IFormFile? inputFile, DateTime date)
        {
            ImageType = imageType;
            InputFile = inputFile;
            Date = date;
        }

        public UpdateImageFormViewModel(ImageType imageType, DateTime date)
        {
            ImageType = imageType;
            this.Date = date; 
        }
    }
}
