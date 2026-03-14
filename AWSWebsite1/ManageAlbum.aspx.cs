using AWSDataLayers.BLL;
using AWSDataLayers.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;

namespace AWSWebsite1
{
    public partial class ManageAlbum : Page
    {
        private int albumId;

        protected void Page_Load(object sender, EventArgs e)
        {
            var member = Session["member"] as Member;
            if (member == null)
            {
                Response.Redirect("Default.aspx");
                return;
            }

            if (!int.TryParse(Request.QueryString["albumId"], out albumId) || albumId <= 0)
            {
                Response.Redirect("MyAlbum.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadAlbum();
                BindPhotos();
            }
            else
            {
                // Handle photo deletion
                if (!string.IsNullOrEmpty(hfDeleteAction.Value) && hfDeleteAction.Value == "delete")
                {
                    int photoId = Convert.ToInt32(hfDeletePhotoId.Value);
                    string photoName = hfDeletePhotoName.Value;
                    DeletePhoto(photoId, photoName);
                    hfDeleteAction.Value = "";
                    BindPhotos();
                }
            }
        }

        private void LoadAlbum()
        {
            var bal = new AlbumBAL();
            var album = bal.GetAlbum(albumId);
            if (album == null || album.MemberID != (Session["member"] as Member).MemberID)
            {
                Response.Redirect("MyAlbum.aspx");
                return;
            }
            lblAlbumTitle.InnerText = $"Photos in: {album.AlbumName}";
        }

        private void BindPhotos()
        {
            var bal = new AlbumBAL();
            var photos = bal.GetAlbumPhotos(albumId);
            rptPhotos.DataSource = photos;
            rptPhotos.DataBind();
        }

        private void DeletePhoto(int photoId, string photoName)
        {
            try
            {
                var bal = new AlbumBAL();

                // Delete photo from database
                bal.DeletePhoto(photoId);

                // Delete physical file
                string filePath = Server.MapPath("~/AlbumPhotos/" + photoName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                // Update photo count
                var album = bal.GetAlbum(albumId);
                if (album.PhotoCount > 0)
                {
                    short newCount = (short)(album.PhotoCount - 1);
                    bal.UpdatePhotoCount(albumId, newCount);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error deleting photo: " + ex.Message);
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (!fuPhotos.HasFiles)
                return;

            var photoStreams = new List<Stream>();
            foreach (HttpPostedFile file in fuPhotos.PostedFiles)
            {
                if (file.ContentLength > 0)
                    photoStreams.Add(file.InputStream);
            }

            string saveDir = Server.MapPath("~/AlbumPhotos/");
            if (!Directory.Exists(saveDir))
                Directory.CreateDirectory(saveDir);

            var bal = new AlbumBAL();
            var uploadedPhotos = bal.AddPhotosToAlbum(photoStreams, albumId, saveDir);

            // Update photo count correctly
            if (uploadedPhotos.Count > 0)
            {
                var album = bal.GetAlbum(albumId);
                short newCount = (short)(album.PhotoCount + uploadedPhotos.Count);
                bal.UpdatePhotoCount(albumId, newCount);
            }

            BindPhotos();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("MyAlbum.aspx");
        }
    }
}