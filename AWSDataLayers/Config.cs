using System.Configuration;

namespace AWSDataLayers
{
    public static class ConfigInfo
    {
        public static readonly string connectionString = GetConnectionString();// ConfigurationManager.ConnectionStrings["AWSDemoDB"].ConnectionString;
        // added for aws setup
        //public static readonly string AlbumPhotoPath = "AWSWebsite" + ConfigurationManager.AppSettings["AlbumPhotoPath"];
        //public static readonly string ProfileImagePath = "AWSWebsite" + ConfigurationManager.AppSettings["ProfileImagePath"];

        public static readonly string ProfileImagePath = "AWSWebsite" + ConfigurationManager.AppSettings["ProfileImagePath"];

        public static readonly string AWS_AccessKey = ConfigurationManager.AppSettings["AWS_AccessKey"];
        public static readonly string AWS_SecretKey = ConfigurationManager.AppSettings["AWS_SecretKey"];
        private static string GetConnectionString()
        {
            var cs = ConfigurationManager.ConnectionStrings["AWSDemoDB"];
            if (cs == null)
                throw new System.Exception("Connection string 'AWSDemoDB' not found in web.config. Check the name matches exactly.");
            return cs.ConnectionString;
        }
    }
}
 