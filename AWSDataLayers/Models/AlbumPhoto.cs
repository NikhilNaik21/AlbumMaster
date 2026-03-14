
using System;

namespace AWSDataLayers.Models
{
    public class AlbumPhoto
    {
        public int Id { get; set; }

        public int AlbumId { get; set; }

        public string PhotoName { get; set; }

        public DateTime? UploadDate { get; set; }
    }
}
