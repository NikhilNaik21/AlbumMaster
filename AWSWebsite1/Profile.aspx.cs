using AWSDataLayers;
using AWSDataLayers.BLL;
using AWSDataLayers.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace AWSWebsite1
{
    public partial class Profile : System.Web.UI.Page
    {
        protected string profileImage = ConfigInfo.ProfileImagePath + "noimage.png";
        protected Member member = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            member = Session["member"] as Member;

            if (member == null)
            {
                Response.Redirect("Default.aspx", false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                return;
            }

            // Load profile image
            profileImage = !string.IsNullOrEmpty(member.ProfileImage)
                ? ConfigInfo.ProfileImagePath + member.ProfileImage
                : ConfigInfo.ProfileImagePath + "noimage.png";

            // Handle AJAX upload
            if (Request["method"] == "profileImage")
            {
                SaveProfileImage();
                return;
            }

            dtlProfile.DataSource = new List<object>
            {
                new
                {
                    Name = member.FullName,
                    Email = member.EmailID,
                    MemberID = member.MemberID,
                    Joined = member.CreatedOn.HasValue
                        ? member.CreatedOn.Value.ToString("dd MMM yyyy")
                        : "N/A"
                }
            };

            dtlProfile.DataBind();
        }

        private void SaveProfileImage()
        {
            try
            {
                if (member == null)
                {
                    Response.WriteJson(new { IsSuccess = false, Message = "Session expired. Login again." });
                    return;
                }

                if (Request.Files.Count == 0)
                {
                    Response.WriteJson(new { IsSuccess = false, Message = "No file received" });
                    return;
                }

                HttpPostedFile file = Request.Files["file"];

                if (file == null || file.ContentLength == 0)
                {
                    Response.WriteJson(new { IsSuccess = false, Message = "Empty file" });
                    return;
                }

                string extension = Path.GetExtension(file.FileName).ToLower();

                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

                if (!allowedExtensions.Contains(extension))
                {
                    Response.WriteJson(new { IsSuccess = false, Message = "Invalid file type" });
                    return;
                }

                string saveImageName = DateTime.Now.Ticks + extension;

                string folderPath = Server.MapPath(ConfigInfo.ProfileImagePath);

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string fullPath = Path.Combine(folderPath, saveImageName);

                file.SaveAs(fullPath);

                // Update database
                member.ProfileImage = saveImageName;
                new MemberBAL().SaveMember(member);

                Session["member"] = member;

                Response.WriteJson(new
                {
                    IsSuccess = true,
                    Message = ConfigInfo.ProfileImagePath + saveImageName
                });
            }
            catch (Exception ex)
            {
                Response.WriteJson(new
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
        }
    }
}