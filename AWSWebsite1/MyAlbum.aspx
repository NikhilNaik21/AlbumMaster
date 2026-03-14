<%@ Page Title="My Album" Language="C#" MasterPageFile="~/MasterPage.master" 
    AutoEventWireup="true" CodeBehind="MyAlbum.aspx.cs" Inherits="AWSWebsite1.MyAlbums" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <link href="/CSS/album.css" rel="stylesheet" />
    <form id="formMyAlbums" runat="server">
    <h2 class="album-title">My Albums</h2>

        <div style="margin-left:165px; margin-bottom:-110px; ">
            <asp:Button
                ID="btnNewAlbum"
                runat="server"
                Text="Create New Album"
                OnClick="btnNewAlbum_Click"
              />
            <span style="margin-left: 550px;">
              <asp:Button 
                 ID="btnBackToProfile"
                 runat="server"
                 Text="← Back to Profile Page"
                 CssClass="btn-back"
                 PostBackUrl="~/Profile.aspx"
                 />
            </span>
         </div>
    <br /><br />

    <asp:GridView
        ID="gvAlbums"
        runat="server"
        AutoGenerateColumns="False"
        EmptyDataText="No albums created yet"
        style="margin-left:250px; margin-top:90px;"
        OnRowCommand="gvAlbums_RowCommand"
        DataKeyNames="Id">
        
            <Columns>

                <asp:BoundField
                    DataField="AlbumName"
                    HeaderText="Album Name" />

                <asp:BoundField
                    DataField="PhotoCount"
                    HeaderText="Photos" />

                <asp:BoundField
                    DataField="CreatedDate"
                    HeaderText="Created Date"
                    DataFormatString="{0:dd-MMM-yyyy}" />

                <asp:HyperLinkField
                    HeaderText="Manage"
                    Text="View"
                    DataNavigateUrlFields="Id"
                    DataNavigateUrlFormatString="ManageAlbum.aspx?albumId={0}" />

                <asp:ButtonField
                    HeaderText="Delete"
                    ButtonType="Link"
                    Text="Delete"
                    CommandName="DeleteAlbum" />

                </Columns>
    </asp:GridView>
        </form>
</asp:Content>