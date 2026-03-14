using AWSDataLayers.Models;
using System;
using System.Data.SqlClient;

namespace AWSDataLayers.BLL
{
    public class MemberBAL
    {
        public Member GetMember(int memberID)
        {
            using (SqlConnection cn = Common.GetAlbumDBConnection())
            {
                try
                {
                    cn.Open();
                    using (var cmd = new SqlCommand(
                        "SELECT * FROM Member WHERE MemberID = @MemberID", cn))
                    {
                        cmd.Parameters.AddWithValue("@MemberID", memberID);
                        using (var r = cmd.ExecuteReader())
                        {
                            if (!r.Read()) return null;
                            return MapMember(r);
                        }
                    }
                }
                finally
                {
                    if (cn != null && cn.State == System.Data.ConnectionState.Open)
                        cn.Close();
                }
            }
        }

        public Member GetMember(string emailID)
        {
            using (SqlConnection cn = Common.GetAlbumDBConnection())
            {
                try
                {
                    cn.Open();
                    using (var cmd = new SqlCommand(
                        "SELECT * FROM Member WHERE EmailID = @EmailID", cn))
                    {
                        cmd.Parameters.AddWithValue("@EmailID", emailID);
                        using (var r = cmd.ExecuteReader())
                        {
                            if (!r.Read()) return null;
                            return MapMember(r);
                        }
                    }
                }
                finally
                {
                    if (cn != null && cn.State == System.Data.ConnectionState.Open)
                        cn.Close();
                }
            }
        }

        private Member MapMember(SqlDataReader r)
        {
            return new Member
            {
                MemberID = (int)r["MemberID"],
                FullName = r["FullName"] as string,
                EmailID = r["EmailID"] as string,
                Password = r["Password"] as string,
                ProfileImage = r["ProfileImage"] as string,
                CreatedOn = r["CreatedOn"] == DBNull.Value
                               ? (DateTime?)null
                               : (DateTime)r["CreatedOn"]
            };
        }

        public int SaveMember(Member member)
        {
            SqlConnection cn = null;
            try
            {
                cn = Common.GetAlbumDBConnection();
                cn.Open();

                int result = 0;

                if (member.MemberID == 0)
                {
                    using (var cmd = new SqlCommand(
                        @"INSERT INTO Member (FullName, EmailID, Password, ProfileImage, CreatedOn)
                          VALUES (@FullName, @EmailID, @Password, @ProfileImage, @CreatedOn)", cn))
                    {
                        cmd.CommandTimeout = 30;
                        cmd.Parameters.AddWithValue("@FullName", member.FullName ?? "");
                        cmd.Parameters.AddWithValue("@EmailID", member.EmailID ?? "");
                        cmd.Parameters.AddWithValue("@Password", member.Password ?? "");
                        cmd.Parameters.AddWithValue("@ProfileImage", (object)member.ProfileImage ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreatedOn", (object)member.CreatedOn ?? DBNull.Value);
                        result = cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (var cmd = new SqlCommand(
                        @"UPDATE Member SET FullName=@FullName, EmailID=@EmailID,
                          Password=@Password, ProfileImage=@ProfileImage
                          WHERE MemberID=@MemberID", cn))
                    {
                        cmd.CommandTimeout = 30;
                        cmd.Parameters.AddWithValue("@FullName", member.FullName ?? "");
                        cmd.Parameters.AddWithValue("@EmailID", member.EmailID ?? "");
                        cmd.Parameters.AddWithValue("@Password", member.Password ?? "");
                        cmd.Parameters.AddWithValue("@ProfileImage", (object)member.ProfileImage ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@MemberID", member.MemberID);
                        result = cmd.ExecuteNonQuery();
                    }
                }

                return result;
            }
            finally
            {
                if (cn != null)
                {
                    try
                    {
                        if (cn.State == System.Data.ConnectionState.Open)
                            cn.Close();
                    }
                    catch { }

                    try
                    {
                        cn.Dispose();
                    }
                    catch { }
                }
            }
        }
    }
}