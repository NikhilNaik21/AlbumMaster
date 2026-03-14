<%@ Page Title="Create Album" Language="C#" AutoEventWireup="true" 
    CodeBehind="CreateAlbum.aspx.cs" Inherits="AWSWebsite1.CreateAlbum" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Create Album</title>
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <link href="/CSS/createAlbum.css" rel="stylesheet" />
    <style>
        .form-container { max-width: 400px; margin: 50px auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px; }
        .form-group { margin-bottom: 15px; }
        .form-control { width: 100%; padding: 8px; border: 1px solid #ccc; border-radius: 4px; }
        .btn { padding: 8px 16px; border: none; border-radius: 4px; cursor: pointer; margin-right: 10px; }
        .btn-success { background: #28a745; color: white; }
        .btn-secondary { background: #6c757d; color: white; }
    </style>
</head>
<body>
    <form id="form1" runat="server" class="form-container">
        <h2>Create New Album</h2>
        
        <div class="form-group">
            <label>Album Name <span style="color:red;">*</span></label>
            <asp:TextBox ID="txtAlbumName" runat="server" CssClass="form-control" MaxLength="100" />
            <asp:RequiredFieldValidator ID="rfvName" runat="server" 
                ControlToValidate="txtAlbumName" ErrorMessage="Album name required"
                ForeColor="Red" Display="Dynamic" />
        </div>
        
        <asp:Button ID="btnSave" runat="server" Text="Create Album" 
            OnClick="btnSave_Click" CssClass="btn btn-success" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" 
            OnClick="btnCancel_Click" CssClass="btn btn-secondary" CausesValidation="false" />
    </form>
</body>
</html>
