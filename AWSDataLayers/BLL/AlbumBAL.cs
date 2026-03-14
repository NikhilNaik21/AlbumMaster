using AWSDataLayers.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace AWSDataLayers.BLL
{
    public class AlbumBAL
    {
        public List<Album> GetList(int memberID)
        {
            var list = new List<Album>();

            using (SqlConnection cn = Common.GetAlbumDBConnection())
            {
                cn.Open();

                using (var cmd = new SqlCommand(
                    "SELECT * FROM AlbumMaster WHERE MemberID = @memberID", cn))
                {
                    cmd.Parameters.AddWithValue("@memberID", memberID);

                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                            list.Add(MapAlbum(r));
                    }
                }
            }

            return list;
        }

        private Album MapAlbum(SqlDataReader r)
        {
            return new Album
            {
                Id = Convert.ToInt32(r["Id"]),
                MemberID = Convert.ToInt32(r["MemberID"]),
                AlbumName = r["AlbumName"] as string,
                AlbumUrl = r["AlbumUrl"] as string,
                PhotoCount = Convert.ToInt16(r["PhotoCount"]),
                CoverImageName = r["CoverImageName"] as string,
                CreatedDate = r["CreatedDate"] == DBNull.Value
                                ? (DateTime?)null
                                : Convert.ToDateTime(r["CreatedDate"])
            };
        }

        public int SaveAlbum(Album album)
        {
            using (SqlConnection cn = Common.GetAlbumDBConnection())
            {
                cn.Open();

                if (album.Id == 0)
                {
                    using (var cmd = new SqlCommand(
                        @"INSERT INTO AlbumMaster 
                          (AlbumName, AlbumUrl, PhotoCount, CoverImageName, MemberID, CreatedDate)
                          VALUES 
                          (@AlbumName, @AlbumUrl, @PhotoCount, @CoverImageName, @MemberID, @CreatedDate)", cn))
                    {
                        cmd.Parameters.AddWithValue("@AlbumName", album.AlbumName ?? "");
                        cmd.Parameters.AddWithValue("@AlbumUrl", album.AlbumUrl ?? "");
                        cmd.Parameters.AddWithValue("@PhotoCount", album.PhotoCount);
                        cmd.Parameters.AddWithValue("@CoverImageName", (object)album.CoverImageName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@MemberID", album.MemberID);
                        cmd.Parameters.AddWithValue("@CreatedDate", (object)album.CreatedDate ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (var cmd = new SqlCommand(
                        @"UPDATE AlbumMaster 
                          SET AlbumName=@AlbumName, 
                              AlbumUrl=@AlbumUrl,
                              PhotoCount=@PhotoCount, 
                              CoverImageName=@CoverImageName
                          WHERE Id=@Id", cn))
                    {
                        cmd.Parameters.AddWithValue("@AlbumName", album.AlbumName ?? "");
                        cmd.Parameters.AddWithValue("@AlbumUrl", album.AlbumUrl ?? "");
                        cmd.Parameters.AddWithValue("@PhotoCount", album.PhotoCount);
                        cmd.Parameters.AddWithValue("@CoverImageName", (object)album.CoverImageName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Id", album.Id);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public List<AlbumPhoto> GetAlbumPhotos(int albumID)
        {
            var list = new List<AlbumPhoto>();

            using (SqlConnection cn = Common.GetAlbumDBConnection())
            {
                cn.Open();

                using (var cmd = new SqlCommand(
                    "SELECT * FROM AlbumPhoto WHERE AlbumId = @albumID", cn))
                {
                    cmd.Parameters.AddWithValue("@albumID", albumID);

                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new AlbumPhoto
                            {
                                Id = Convert.ToInt32(r["Id"]),
                                AlbumId = Convert.ToInt32(r["AlbumId"]),
                                PhotoName = r["PhotoName"] as string,
                                UploadDate = r["UploadDate"] == DBNull.Value
                                                ? (DateTime?)null
                                                : Convert.ToDateTime(r["UploadDate"])
                            });
                        }
                    }
                }
            }

            return list;
        }

        public List<AlbumPhoto> AddPhotosToAlbum(List<Stream> photos, int albumID, string saveDirectory)
        {
            var savedList = new List<AlbumPhoto>();

            using (SqlConnection cn = Common.GetAlbumDBConnection())
            {
                cn.Open();

                int ownerID = 0;

                using (var cmd = new SqlCommand(
                    "SELECT MemberID FROM AlbumMaster WHERE Id=@albumID", cn))
                {
                    cmd.Parameters.AddWithValue("@albumID", albumID);

                    var result = cmd.ExecuteScalar();
                    ownerID = result == null ? 0 : Convert.ToInt32(result);
                }

                if (ownerID > 0 && photos != null)
                {
                    // Ensure directory exists
                    if (!Directory.Exists(saveDirectory))
                        Directory.CreateDirectory(saveDirectory);

                    foreach (Stream image in photos)
                    {
                        if (image == null || !image.CanRead)
                            continue;

                        string saveImageName = DateTime.Now.Ticks + ".jpg";
                        string filePath = Path.Combine(saveDirectory, saveImageName);

                        using (var fs = File.Create(filePath))
                        {
                            image.Seek(0, SeekOrigin.Begin);
                            image.CopyTo(fs);
                        }

                        using (var cmd = new SqlCommand(
                            "INSERT INTO AlbumPhoto (AlbumId, PhotoName) VALUES (@AlbumId, @PhotoName)", cn))
                        {
                            cmd.Parameters.AddWithValue("@AlbumId", albumID);
                            cmd.Parameters.AddWithValue("@PhotoName", saveImageName);
                            cmd.ExecuteNonQuery();
                        }

                        savedList.Add(new AlbumPhoto
                        {
                            AlbumId = albumID,
                            PhotoName = saveImageName
                        });
                    }
                }
            }

            return savedList;
        }

        public Album GetAlbum(int albumId)
        {
            using (SqlConnection cn = Common.GetAlbumDBConnection())
            {
                cn.Open();

                using (var cmd = new SqlCommand(
                    "SELECT * FROM AlbumMaster WHERE Id = @id", cn))
                {
                    cmd.Parameters.AddWithValue("@id", albumId);

                    using (var r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                            return MapAlbum(r);
                    }
                }
            }

            return null;
        }

        public void UpdatePhotoCount(int albumId, short newCount)
        {
            using (SqlConnection cn = Common.GetAlbumDBConnection())
            {
                cn.Open();

                using (var cmd = new SqlCommand(
                    "UPDATE AlbumMaster SET PhotoCount = @count WHERE Id = @id", cn))
                {
                    cmd.Parameters.AddWithValue("@count", newCount);
                    cmd.Parameters.AddWithValue("@id", albumId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteAlbum(int albumId)
        {
            using (SqlConnection cn = Common.GetAlbumDBConnection())
            {
                cn.Open();
                SqlTransaction transaction = cn.BeginTransaction();

                try
                {
                    // First, delete all photos related to this album from AlbumPhoto table
                    using (var cmd = new SqlCommand(
                        "DELETE FROM AlbumPhoto WHERE AlbumId = @albumId", cn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@albumId", albumId);
                        cmd.ExecuteNonQuery();
                    }

                    // Then, delete the album from AlbumMaster table
                    using (var cmd = new SqlCommand(
                        "DELETE FROM AlbumMaster WHERE Id = @albumId", cn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@albumId", albumId);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Error deleting album: " + ex.Message);
                }
            }
        }

        // Add this method to your existing AlbumBAL class

        public void DeletePhoto(int photoId)
        {
            using (SqlConnection cn = Common.GetAlbumDBConnection())
            {
                cn.Open();

                using (var cmd = new SqlCommand(
                    "DELETE FROM AlbumPhoto WHERE Id = @photoId", cn))
                {
                    cmd.Parameters.AddWithValue("@photoId", photoId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}