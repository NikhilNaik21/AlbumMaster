using AWSDataLayers.BLL;
using AWSDataLayers.Models;
using System;
namespace AWSWebsite1
{
    public partial class CreateAlbum : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var member = Session["member"] as Member;
            if (member == null)
            {
                Response.Redirect("Default.aspx");
                return;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            var album = new Album
            {
                AlbumName = txtAlbumName.Text.Trim(),
                MemberID = (Session["member"] as Member).MemberID,
                AlbumUrl = "",
                PhotoCount = 0,
                CreatedDate = DateTime.Now
            };

            new AlbumBAL().SaveAlbum(album);
            Response.Redirect("MyAlbum.aspx");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("MyAlbum.aspx");
        }
    }
}
