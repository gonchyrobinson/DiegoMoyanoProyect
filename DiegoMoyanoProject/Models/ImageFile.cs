namespace DiegoMoyanoProject.Models
{
    public class ImageFile
    {
        private string? img;
        private ImageType _ImageType;
        private int id;
        public ImageFile(string? blob64img, ImageType type)
        {
            this.img = blob64img;
            _ImageType = type;
        }
        public ImageFile(ImageType type)
        {
            this.img = null;
            _ImageType = type;
        }
        public ImageFile(ImageType type, int id)
        {
            this.img = null;
            _ImageType = type;
            this.id = id;
        }

        public ImageFile()
        {
        }
        public ImageFile(string type, string blob64string, ImageType t)
        {
            this.img = "data:image/"+type+";base64,"+blob64string;
            this._ImageType = t;
        }

        public ImageFile(string? path, ImageType imageType, int id) : this(path, imageType)
        {
            this.id = id;
        }

        public string? Img { get => img; set => img = value; }
        public ImageType ImageType { get => _ImageType; set => _ImageType = value; }
        public int Id { get => id; set => id = value; }
    }
}