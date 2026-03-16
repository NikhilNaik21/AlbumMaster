<%@ Page Title="My Profile" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="AWSWebsite1.Profile" 
    EnableEventValidation="false" 
    ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHTMLHead" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />
    <link href="Scripts/plupload/js/jquery.ui.plupload/css/jquery.ui.plupload.css" rel="stylesheet" />
    <link href="~/CSS/profile.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="server">
    <div style="padding: 40px 20px;">
        <div class="row justify-content-center">
            <div class="col-lg-10">
                <!-- Two Column Profile Card -->
                <div class="profile-card-wrap">
                    <div class="row g-0">

                        <!-- LEFT: Gradient Panel -->
                        <div class="col-lg-4 gradient-custom text-center text-white profile-left-panel">
                            <a id="lnkProfilePic" title="Upload Profile Picture" style="cursor:pointer;">
                                <div class="profile-avatar" id="profileAvatarContainer" style="margin-top: 30px;">
                                    <% if (!string.IsNullOrEmpty(profileImage)) { %>
                                        <img src="<%= profileImage %>" id="imgProfile" alt="Profile" class="profile-img" onerror="showNoImagePlaceholder()"/>
                                    <% } else { %>
                                        <img src="<%= noImagePlaceholder %>" id="imgProfile" alt="No Profile Image" class="profile-img"/>
                                    <% } %>
                                    <div id="imgProgress"></div>
                                </div>
                            </a>
                        </div>

                        <!-- RIGHT: Dark Info Panel -->
                        <div class="col-lg-8 profile-right-panel">
                            <div class="card-body p-4" style="margin-left: -75px; margin-right:145px">
                                <!-- Information Section -->
                                <h6 class="section-heading">Information</h6>
                                <hr class="section-divider mt-0 mb-3" />
                                <div class="row pt-1">
                                    <div class="col-6 mb-3">
                                        <h6 class="info-label">Email</h6>
                                        <p class="info-value">
                                            <%= member != null ? member.EmailID : "N/A" %>
                                        </p>
                                    </div>
                                    <div class="col-6 mb-3">
                                        <h6 class="info-label">Member Since</h6>
                                        <p class="info-value">
                                            <%= member != null && member.CreatedOn.HasValue 
                                                ? member.CreatedOn.Value.ToString("MMM yyyy") 
                                                : "N/A" %>
                                        </p>
                                    </div>
                                </div>

                                <!-- Account Details Table -->
                                <h6 class="section-heading">Account Details</h6>
                                <hr class="section-divider mt-0 mb-3" />
                                <div class="profile-details-table mb-4">
                                    <form id="frmDetailView" runat="server">
                                        <asp:DetailsView 
                                            ID="dtlProfile" 
                                            runat="server" 
                                            AutoGenerateRows="False"
                                            CssClass="table profile-dv-table"
                                            >
                                            <Fields>
                                                <asp:BoundField DataField="Name" HeaderText="Full Name" />
                                                <asp:BoundField DataField="Email" HeaderText="Email Address" />
                                                <asp:BoundField DataField="MemberID" HeaderText="Member ID" />
                                                <asp:BoundField DataField="Joined" HeaderText="Date Joined" />
                                            </Fields>
                                        </asp:DetailsView>
                                    </form>
                                </div>

                                <!-- Projects / Albums Section -->
                                <h6 class="section-heading">Projects / Albums</h6>
                                <hr class="section-divider mt-0 mb-3" />
                                
                                <!-- Social Icons + Album Button -->
                                <div class="d-flex justify-content-between align-items-center flex-wrap gap-3">
                                    <div class="social-icons">
                                        <a href="#!" class="social-link"><i class="fab fa-facebook-f fa-lg"></i></a>
                                        <a href="#!" class="social-link"><i class="fab fa-twitter fa-lg"></i></a>
                                        <a href="#!" class="social-link"><i class="fab fa-instagram fa-lg"></i></a>
                                    </div>
                                    <a href="/MyAlbum.aspx" class="btn-album">
                                        View My Albums <i class="fas fa-arrow-right ms-2"></i>
                                    </a>
                                </div>

                            </div>
                        </div>
                        <!-- END RIGHT PANEL -->

                    </div>
                </div>
                <!-- END CARD -->
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphFooter" runat="server">
    <script src="Scripts/plupload/js/moxie.min.js"></script>
    <script src="Scripts/plupload/js/plupload.full.min.js"></script>

    <script>
        var noImagePath = '<%= noImagePlaceholder %>';

        function showNoImagePlaceholder() {
            var img = document.getElementById('imgProfile');
            if (img) {
                img.src = noImagePath;
            }
        }

        $(document).ready(function () {
            function initUploader() {
                var mediaUploader = new plupload.Uploader({
                    runtimes: 'html5',
                    browse_button: 'lnkProfilePic',
                    url: 'UploadHandler.ashx',
                    multipart: true,
                    file_data_name: "file",
                    filters: {
                        max_file_size: '5mb',
                        mime_types: [{ title: "Images", extensions: "jpg,jpeg,png" }]
                    },
                    init: {
                        FilesAdded: function (up, files) { up.start(); },
                        UploadProgress: function (up, file) {
                            $("#imgProgress").css("width", file.percent + "%");
                        },
                        FileUploaded: function (up, file, response) {
                            var data = JSON.parse(response.response);
                            if (data.IsSuccess) {
                                var img = document.getElementById('imgProfile');
                                img.src = data.Message + "?t=" + new Date().getTime();
                                $("#imgProgress").css("width", "0");
                                up.splice(); up.destroy(); initUploader();
                            } else {
                                alert(data.Message);
                            }
                        },
                        Error: function (up, err) {
                            alert("Upload error: " + err.message);
                            up.splice();
                        }
                    }
                });
                mediaUploader.init();
            }
            initUploader();
        });
    </script>
</asp:Content>
