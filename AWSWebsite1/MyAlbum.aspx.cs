using System;
using AWSDataLayers.BLL;
using AWSDataLayers.Models;

namespace AWSWebsite1
{
    public partial class MyAlbums : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Member member = Session["member"] as Member;

            if (member == null)
            {
                Response.Redirect("Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindAlbums(member.MemberID);
            }
        }

        private void BindAlbums(int memberId)
        {
            AlbumBAL albumBAL = new AlbumBAL();

            gvAlbums.DataSource = albumBAL.GetList(memberId);
            gvAlbums.DataBind();
        }

        protected void btnNewAlbum_Click(object sender, EventArgs e)
        {
            Response.Redirect("CreateAlbum.aspx");
        }

        protected void gvAlbums_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteAlbum")
            {
                try
                {
                    int albumId = Convert.ToInt32(gvAlbums.DataKeys[Convert.ToInt32(e.CommandArgument)].Value);
                    AlbumBAL albumBAL = new AlbumBAL();
                    albumBAL.DeleteAlbum(albumId);

                    Member member = Session["member"] as Member;
                    BindAlbums(member.MemberID);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Delete error: " + ex.Message);
                }
            }
        }
    }
}