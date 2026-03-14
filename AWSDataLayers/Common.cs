using System.Data.SqlClient;

namespace AWSDataLayers
{
    public class Common
    {
        public static SqlConnection GetAlbumDBConnection()
        {
            return new SqlConnection(ConfigInfo.connectionString);
        }
    }
}
