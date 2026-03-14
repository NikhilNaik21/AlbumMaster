using AWSDataLayers;
using AWSDataLayers.BLL;
using AWSDataLayers.Models;
using System;
using System.IO;
using System.Web;
using System.Web.SessionState;

namespace AWSWebsite1
{
    public class UploadHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                HttpResponse response = context.Response;
                HttpRequest request = context.Request;
                HttpSessionState session = context.Session;

                response.ContentType = "application/json";
                response.BufferOutput = true;

                // Validate session
                Member loggedInMember = session["member"] as Member;
                if (loggedInMember == null)
                {
                    response.Write("{\"IsSuccess\":false,\"Message\":\"Not logged in\"}");
                    return;
                }

                if (request.Files.Count == 0)
                {
                    response.Write("{\"IsSuccess\":false,\"Message\":\"No file\"}");
                    return;
                }

                HttpPostedFile file = request.Files[0];
                if (file == null || file.ContentLength == 0)
                {
                    response.Write("{\"IsSuccess\":false,\"Message\":\"Empty file\"}");
                    return;
                }

                if (file.ContentLength > 50 * 1024 * 1024)
                {
                    response.Write("{\"IsSuccess\":false,\"Message\":\"Too large\"}");
                    return;
                }

                string ext = Path.GetExtension(file.FileName).ToLower();
                string fileName = Guid.NewGuid().ToString() + ext;
                string folderPath = context.Server.MapPath(ConfigInfo.ProfileImagePath);

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string fullPath = Path.Combine(folderPath, fileName);

                file.SaveAs(fullPath);

                // Save to database
                loggedInMember.ProfileImage = fileName;
                new MemberBAL().SaveMember(loggedInMember);

                // Update session
                session["member"] = loggedInMember;

                response.Write("{\"IsSuccess\":true,\"Message\":\"" + ConfigInfo.ProfileImagePath + fileName + "\"}");
                response.Flush();
            }
            catch (Exception ex)
            {
                try
                {
                    context.Response.Write("{\"IsSuccess\":false,\"Message\":\"" + ex.Message.Replace("\"", "'") + "\"}");
                }
                catch { }
            }
        }

        public bool IsReusable => false;
    }
}