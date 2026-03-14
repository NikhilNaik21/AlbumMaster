using AWSDataLayers.BLL;
using AWSDataLayers.Models;
using System;
using System.Web.UI;


namespace AWSWebsite1
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["method"] == "login") { Login(); return; }
            else if (Request["method"] == "signup") { Register(); return; }

        }

        private void WriteJson(object obj)
        {
            Response.ContentType = "application/json";
            Response.Clear();
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            Response.Write(json);
           // HttpContext.Current.ApplicationInstance.CompleteRequest(); //sp
        }

        //perp
        public void Login()
        {
            string emailID = Request["username"];   // ✅ matches login form name="username"
            string password = Request["password"];  // ✅ matches login form name="password"

            ClientResponse resp = new ClientResponse();

            if (!IsValidEmail(emailID))
            {
                resp.Message = "Please enter a valid email address.";
                WriteJson(resp);
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                resp.Message = "Password is required.";
                WriteJson(resp);
                return;
            }


           
            Member member = new MemberBAL().GetMember(emailID);

            if (member == null)
                resp.Message = "This Email ID has not been registered with us.";
            else if (member.Password == Request["Password"])
            {
                resp.IsSuccess = true;
                resp.Message = "login success";
                Session["member"] = member;
            }
            else
                resp.Message = "Invalid password.";

            WriteJson(resp); // ✅ use this
        }

        public void Register()
        {
            ClientResponse resp = new ClientResponse();

            string fullName = Request["FullName"];
            string email = Request["EmailID"];
            string password = Request["Password"];

            if (string.IsNullOrWhiteSpace(fullName))
            {
                resp.Message = "Name is required.";
                WriteJson(resp); return;
            }
            if (!IsValidEmail(email))
            {
                resp.Message = "Please enter a valid email address.";
                WriteJson(resp); return;
            }
            if (!IsStrongPassword(password))
            {
                resp.Message = "Password must be at least 8 chars, include upper, lower and digit.";
                WriteJson(resp); return;
            }



            MemberBAL memberHelper = new MemberBAL();
            Member member = memberHelper.GetMember(Request["EmailID"]);

            if (member != null)
            {
                resp.Message = "This Email ID has already been registered with us.";
            }
            else
            {
                member = new Member()
                {
                    FullName = Request["FullName"],
                    EmailID = Request["EmailID"],
                    Password = Request["Password"],
                    CreatedOn = DateTime.Now
                };

                int result = memberHelper.SaveMember(member);
                if (result > 0)
                {
                    resp.IsSuccess = true;
                    resp.Message = "Registration successful.";
                }
                else
                    resp.Message = "Registration failed. Please try again.";
            }

            WriteJson(resp); // ✅ use this
        }


        protected override void Render(HtmlTextWriter writer)
        {
            // If it's an API call, don't render any HTML
            if (Request["method"] == "login" || Request["method"] == "signup")
                return;

            base.Render(writer);
        }


        private bool IsValidEmail(string email)
        {
            try { var addr = new System.Net.Mail.MailAddress(email); return addr.Address == email; }
            catch { return false; }
        }

        private bool IsStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;
            if (password.Length < 8) return false;
            bool hasUpper = false, hasLower = false, hasDigit = false;
            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                else if (char.IsLower(c)) hasLower = true;
                else if (char.IsDigit(c)) hasDigit = true;
            }
            return hasUpper && hasLower && hasDigit;
        }
    }
}