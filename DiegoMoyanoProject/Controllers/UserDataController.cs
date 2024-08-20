using AutoMapper;
using DiegoMoyanoProject.Exceptions;
using DiegoMoyanoProject.Models;
using DiegoMoyanoProject.Repository;
using DiegoMoyanoProject.ViewModels.UserData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DiegoMoyanoProject.Controllers
{
    public class UserDataController : Controller
    {
        private readonly IImagesRepository _imagesRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<UserDataController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly string folderName;

        public UserDataController(IImagesRepository imagesRepository, IUserRepository userRepository, IWebHostEnvironment webHostEnvironment, ILogger<UserDataController> logger, IMapper mapper)
        {
            _imagesRepository = imagesRepository;
            _userRepository = userRepository;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _mapper = mapper;
            folderName = "usersData";
        }
        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                if (IsNotLogued()) return RedirectToAction("Index", "Login");

                //Si quiero modificar para que se vea el último registro cargado, descomentar y retornar View(IndexUserDataVm)
                //var listImages = _userDataRepository.GetUserImages();
                //var listImagesVm = _mapper.Map<List<ImageDataViewModel>>(listImages);
                var listDates = _imagesRepository.GetAllDatesAndId();
                FileDate? maxDate = null;
                if (listDates.Count() > 0)
                {
                    maxDate = listDates.OrderByDescending(obj => obj.Id).FirstOrDefault();
                }
                else
                {
                    maxDate = new FileDate();
                }
                if (IsOwner()) return RedirectToAction("IndexOwner", new { date = maxDate.Date, id = maxDate.Id });
                return RedirectToAction("IndexDate", new { date = maxDate.Date, id = maxDate.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }
        public IActionResult IndexDate(DateTime date, int id)
        {
            try
            {
                if (IsNotLogued()) return RedirectToAction("Index", "Login");
                if (!ModelState.IsValid) throw (new ModelStateInvalidException());
                var listImages = new List<ImageFile>();
                foreach (var type in ImageData.AllTypes)
                {
                    var img = _imagesRepository.getImage( type, id);
                    if (img != null) listImages.Add(img);
                }
                var listImagesVm = _mapper.Map<List<ImageDataViewModel>>(listImages);
                var listDates = _imagesRepository.GetAllDatesAndId();
                return View(new IndexDateUserDataViewModel(listImagesVm, listDates, date));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }
        [HttpGet]
        public IActionResult IndexOwner(DateTime date, int id)
        {
            try
            {
                if (IsNotLogued()) return RedirectToAction("Index", "Login");
                if (!ModelState.IsValid) throw (new ModelStateInvalidException());
                if (!IsOwner()) return RedirectToAction("IndexDate", new { date = date, id = id });
                var listImages = new List<ImageFile>();
                foreach (var type in ImageData.AllTypes)
                {
                    var img = _imagesRepository.getImage( type, id);
                    if (img == null) img = new ImageFile(type, id);
                    _logger.LogInformation("Imagen obtenida de la DB de " + type.ToString() + " :   " + img.Img + "  .  La fecha en el formato enviado es: " + date.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                    listImages.Add(img);
                }
                var listImagesVm = new List<ImageDataViewModel>();
                //var listImagesVm = _mapper.Map<List<ImageDataViewModel>>(listImages);
                string mensaje = "";
                foreach (var item in listImages)
                {
                    listImagesVm.Add(new ImageDataViewModel(item.Img, item.ImageType, id));
                    mensaje += " - " + item.Img;
                }
                _logger.LogInformation("Rutas obtenidas al mapear: " + mensaje);
                var listDates = _imagesRepository.GetAllDatesAndId();
                return View(new IndexOwnerUserDataViewModel(listImagesVm, listDates, date, id));
            }
            catch (noDateException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("Upload", new { date = DateTime.Today, id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return (BadRequest());
            }
        }
        [HttpGet]
        public IActionResult UpdateImageForm(ImageType type, DateTime date, int id)
        {
            try
            {
                if (IsNotLogued() || !IsOwner()) return RedirectToAction("Index", "Login");
                return View(new UpdateImageFormViewModel(type, date, id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }
        [HttpGet]
        public IActionResult Delete(string date, ImageType type, int id)
        {
            try
            {
                if (IsNotLogued() || !IsOwner()) return RedirectToAction("Index", "Login");
                DateTime dateTime;

                if (DateTime.TryParse(date, out dateTime))
                {
                    _imagesRepository.Delete( type, id);
                }
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }
        [HttpGet]
        public IActionResult DeleteRow(DateTime date, int id)
        {
            try
            {
                if (IsNotLogued() || !IsOwner()) return RedirectToAction("Index", "Login");
                _imagesRepository.DeleteRow(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }

        [HttpGet]
        public IActionResult UploadDate(string date)
        {
            try
            {
                if (IsNotLogued() || !IsOwner()) return RedirectToAction("Index", "Login");
                return View(new UploadDateViewModel(date));
            }
            catch (InconsistenceInTheDBException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }
        [HttpPost]
        public IActionResult Upload(UploadDateViewModel model)
        {
            try
            {
                if (IsNotLogued() || !IsOwner()) return RedirectToAction("Index", "Login");
                if (!ModelState.IsValid) throw (new ModelStateInvalidException());
                    //I consider that if there arent images in the table, the id will not exist.
                int maxId = (_imagesRepository.countImagesAdded() > 0) ? Convert.ToInt32(_imagesRepository.GetMaxId()) : -1;
                _imagesRepository.deleteOlderIfNeeded();
                _imagesRepository.AddDate(model.Date);
                return RedirectToAction("Index");
            }
            catch (InconsistenceInTheDBException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }



        [HttpPost]
        public async Task<IActionResult> Update(UpdateImageFormViewModel model)
        {
            try
            {
                if (IsNotLogued() || !IsOwner()) return RedirectToAction("Index", "Login");
                if (!ModelState.IsValid) throw (new ModelStateInvalidException());
                if (model.InputFile == null) return RedirectToAction("Index");
                string filePath, fileNetworkPath;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    IFormFile file = (IFormFile)model.InputFile;
                    await (file.CopyToAsync(memoryStream));
                    var img = Convert.ToBase64String(memoryStream.ToArray());
                    var type = Path.GetExtension(model.InputFile.FileName).Split('.').Last();
                    _imagesRepository.Update(new ImageFile(type, img, model.ImageType), model.Id);
                }
                return RedirectToAction("IndexOwner", new { date = model.Date, id = model.Id });
            }
            catch (InconsistenceInTheDBException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }

        private void DeleteImagesFolder(string directoryPath)
        {
            Directory.Delete(directoryPath, true);
        }

        private void createPathsForSavingTheImages(ImageType imageType, string fileNameWithExtension, DateTime date, int id, out string filePath, out string fileNetworkPath)
        {
            string rutaGuardar = createFolderForImages(date, id);
            string fileName = getFileNameWithExtension(imageType, fileNameWithExtension, date, id);
            fileNetworkPath = getFileNetworkPath(date, folderName, fileName, id);
            filePath = getLocalFilePath(rutaGuardar, fileName);
        }

        private static string getLocalFilePath(string pathToLocalFolder, string fileNameWithExtension)
        {
            return Path.Combine(pathToLocalFolder, fileNameWithExtension);
        }

        private static string getFileNetworkPath(DateTime date, string folderName, string fileName, int id)
        {
            string networkPath = getNetworkFolder(date, folderName, id);
            string fileNetworkPath = networkPath + "/" + fileName;
            return fileNetworkPath;
        }

        private static string getNetworkFolder(DateTime date, string folderName, int id)
        {
            return "/" + folderName + "/" + date.ToString("ddMMyyyy") + "_" + id;
        }

        private static string getFileNameWithExtension(ImageType imageType, string fileNameWithExtension, DateTime date, int id)
        {
            var fileExtension = Path.GetExtension(fileNameWithExtension);
            string fileNameWithOutExtension = imageType.ToString() + '_' + date.ToString("ddMMyyyy") + "_" + id;
            string fileName = fileNameWithOutExtension + fileExtension;
            return fileName;
        }

        private string createFolderForImages(DateTime date, int id)
        {
            string rutaGuardar = getLocalFolder(date, id);
            if (!Directory.Exists(rutaGuardar)) Directory.CreateDirectory(rutaGuardar);
            return rutaGuardar;
        }

        private string getLocalFolder(DateTime date, int id)
        {
            return Path.Combine(_webHostEnvironment.WebRootPath, folderName, date.ToString("ddMMyyyy") + "_" + id);
        }

        private void DeleteImageFromLocalEnviorment(ImageType imageType, int id)
        {
            var deleteImage = _imagesRepository.getImage(imageType, id);
            if (deleteImage != null && deleteImage.Img != "") System.IO.File.Delete(getLocalPathFromDBReadenPath(deleteImage.Img));
        }

        private string getLocalPathFromDBReadenPath(string DBPath)
        {
            return Path.Combine(_webHostEnvironment.WebRootPath, DBPath.Substring(1));
        }

        private void SaveImageInCreatedFolderAnUpdateInDB(UpdateImageFormViewModel model, string filePath, string fileNetworkPath)
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                model.InputFile.CopyTo(stream);
                ImageFile userData = new ImageFile(fileNetworkPath, model.ImageType);
                _imagesRepository.Update(userData, model.Id);
            }
        }

        [HttpGet]
        public IActionResult ViewInversion(int id)
        {
            try
            {

                if (IsNotLogued() || ((IsOwner() || IsAdmin()) && id == null)) return RedirectToAction("Index", "Login");
                if (IsOwner() && id != null) return RedirectToAction("IndexOwner", new { id = id });
                var currentUserId = IdLoguedUser();
                var isLoguedUser = currentUserId == IdLoguedUser();
                var usu = _userRepository.GetUserById(id);
                return View(new ViewInversionViewModel(usu, isLoguedUser));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }

        public IActionResult RedirectionOfCapitalAndRentability()
        {
            try
            {
                if (IsOperative())
                {
                    return RedirectToAction("ViewInversion", new { id = IdLoguedUser() });
                }
                else
                {
                    return RedirectToRoute(new { Controller = "User", action = "Index" });
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest();
            }
        }

        private int IdLoguedUser()
        {
            return (int)HttpContext.Session.GetInt32("Id");
        }
        private bool IsNotLogued()
        {
            return !HttpContext.Session.IsAvailable || HttpContext.Session.GetString("Mail") == null;
        }
        private Role LoguedUserRole()
        {
            return (Role)HttpContext.Session.GetInt32("Role");
        }
        private bool IsOwner()
        {
            return LoguedUserRole() == Role.Owner;
        }
        private bool IsAdmin()
        {
            return LoguedUserRole() == Role.Admin;
        }
        private bool IsOperative()
        {
            return LoguedUserRole() == Role.Operative;
        }
    }
}

