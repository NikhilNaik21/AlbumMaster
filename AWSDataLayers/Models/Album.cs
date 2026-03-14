using System;
namespace AWSDataLayers.Models
{
    public class Album
    {
        public int Id { get; set; }
        public int MemberID { get; set; }
        public string AlbumName { get; set; }
        public string AlbumUrl { get; set; } = "";
        public DateTime? CreatedDate { get; set; }
        public short PhotoCount { get; set; }
        public string CoverImageName { get; set; }
    }
}
