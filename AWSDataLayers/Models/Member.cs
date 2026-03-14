using System;

namespace AWSDataLayers.Models
{
    public class Member
    {
        public int MemberID { get; set; }

        public string FullName { get; set; }

        public string EmailID { get; set; }


        public DateTime? CreatedOn { get; set; }

        public string Password { get; set; }

        public string ProfileImage { get; set; }
    }
}
