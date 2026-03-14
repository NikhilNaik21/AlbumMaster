<%@ Page Title="Manage Album" Language="C#" AutoEventWireup="true" 
    CodeBehind="ManageAlbum.aspx.cs" Inherits="AWSWebsite1.ManageAlbum" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Manage Album</title>
    <style>
        .album-container { max-width: 900px; margin: 20px auto; padding: 20px; }
        .upload-area { border: 2px dashed #ccc; padding: 30px; text-align: center; margin: 20px 0; }
        .photo-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(150px, 1fr)); gap: 15px; }
        .photo-thumb { border: 1px solid #ddd; border-radius: 8px; overflow: hidden; cursor: pointer; position: relative; transition: transform 0.2s; }
        .photo-thumb:hover { transform: scale(1.05); }
        .photo-thumb img { width: 100%; height: 120px; object-fit: cover; }
        .photo-overlay { position: absolute; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0, 0, 0, 0.5); display: flex; align-items: center; justify-content: center; gap: 10px; opacity: 0; transition: opacity 0.3s; }
        .photo-thumb:hover .photo-overlay { opacity: 1; }
        .btn-action { color: white; padding: 10px 20px; border: none; border-radius: 4px; cursor: pointer; font-weight: bold; font-size: 14px; transition: background 0.2s; }
        .btn-view { background: #007bff; }
        .btn-view:hover { background: #0056b3; }
        .btn-delete { background: #dc3545; }
        .btn-delete:hover { background: #c82333; }
        
        .btn { padding: 8px 16px; border: none; border-radius: 4px; cursor: pointer; margin: 5px; }
        .btn-primary { background: #007bff; color: white; }
        .btn-secondary { background: #6c757d; color: white; }
        
        /* Lightbox styles */
        .lightbox-overlay { display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0, 0, 0, 0.8); z-index: 1000; align-items: center; justify-content: center; }
        .lightbox-overlay.active { display: flex; }
        .lightbox-content { position: relative; max-width: 90%; max-height: 90%; }
        .lightbox-image { max-width: 100%; max-height: 80vh; object-fit: contain; }
        .lightbox-close { position: absolute; top: 20px; right: 20px; width: 40px; height: 40px; background: white; border: none; border-radius: 50%; font-size: 24px; cursor: pointer; display: flex; align-items: center; justify-content: center; font-weight: bold; color: #333; z-index: 1001; }
        .lightbox-close:hover { background: #f0f0f0; }

        /* Confirmation Modal Styles */
        .modal-overlay { display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0, 0, 0, 0.6); z-index: 2000; align-items: center; justify-content: center; }
        .modal-overlay.active { display: flex; }
        .modal-content { background: white; border-radius: 10px; padding: 30px; max-width: 400px; box-shadow: 0 5px 30px rgba(0, 0, 0, 0.3); text-align: center; }
        .modal-header { font-size: 20px; font-weight: bold; color: #333; margin-bottom: 15px; }
        .modal-message { font-size: 16px; color: #666; margin-bottom: 25px; }
        .modal-buttons { display: flex; gap: 10px; justify-content: center; }
        .modal-btn { padding: 10px 25px; border: none; border-radius: 5px; cursor: pointer; font-weight: bold; font-size: 14px; transition: background 0.2s; }
        .modal-btn-confirm { background: #dc3545; color: white; }
        .modal-btn-confirm:hover { background: #c82333; }
        .modal-btn-cancel { background: #6c757d; color: white; }
        .modal-btn-cancel:hover { background: #545b62; }
    </style>

    <link href="/css/manageAlbum.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server" class="album-container">
        <asp:Button ID="btnBack" runat="server" Text="← Back to Albums" 
            OnClick="btnBack_Click" CssClass="btn btn-secondary" />
        <h2 id="lblAlbumTitle" runat="server">Album Photos</h2>
        
        <div class="upload-area">
            <asp:FileUpload ID="fuPhotos" runat="server" AllowMultiple="true" />
            <br /><br />
            <asp:Button ID="btnUpload" runat="server" Text="Upload Photos" 
                OnClick="btnUpload_Click" CssClass="btn btn-primary" />
        </div>
        
        <div class="photo-grid">
            <asp:Repeater ID="rptPhotos" runat="server">
                <ItemTemplate>
                    <div class="photo-thumb">
                        <img src='<%# ResolveUrl("~/AlbumPhotos/" + Eval("PhotoName")) %>' alt="Album Photo" />
                        <div class="photo-overlay">
                            <button type="button" class="btn-action btn-view" onclick="openLightbox('<%# ResolveUrl("~/AlbumPhotos/" + Eval("PhotoName")) %>'); return false;">View</button>
                            <button type="button" class="btn-action btn-delete" onclick="showDeleteConfirm('<%# Eval("Id") %>', '<%# Eval("PhotoName") %>'); return false;">Delete</button>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <!-- Lightbox Modal -->
        <div class="lightbox-overlay" id="lightboxOverlay">
            <div class="lightbox-content">
                <button type="button" class="lightbox-close" onclick="closeLightbox()">&times;</button>
                <img id="lightboxImage" class="lightbox-image" alt="Enlarged Photo" />
            </div>
        </div>

        <!-- Confirmation Modal -->
        <div class="modal-overlay" id="deleteConfirmModal">
            <div class="modal-content">
                <div class="modal-header">Delete Photo</div>
                <div class="modal-message">Are you sure you want to delete this photo? This action cannot be undone.</div>
                <div class="modal-buttons">
                    <button type="button" class="modal-btn modal-btn-confirm" onclick="confirmDelete()">Delete</button>
                    <button type="button" class="modal-btn modal-btn-cancel" onclick="cancelDelete()">Cancel</button>
                </div>
            </div>
        </div>

        <!-- Hidden fields for deletion -->
        <asp:HiddenField ID="hfDeletePhotoId" runat="server" />
        <asp:HiddenField ID="hfDeletePhotoName" runat="server" />
        <asp:HiddenField ID="hfDeleteAction" runat="server" />
    </form>

    <script>
        let pendingPhotoId = null;
        let pendingPhotoName = null;

        function openLightbox(imageSrc) {
            const overlay = document.getElementById('lightboxOverlay');
            const image = document.getElementById('lightboxImage');
            image.src = imageSrc;
            overlay.classList.add('active');
        }

        function closeLightbox() {
            const overlay = document.getElementById('lightboxOverlay');
            overlay.classList.remove('active');
        }

        function showDeleteConfirm(photoId, photoName) {
            pendingPhotoId = photoId;
            pendingPhotoName = photoName;
            const modal = document.getElementById('deleteConfirmModal');
            modal.classList.add('active');
        }

        function confirmDelete() {
            document.getElementById('<%= hfDeletePhotoId.ClientID %>').value = pendingPhotoId;
            document.getElementById('<%= hfDeletePhotoName.ClientID %>').value = pendingPhotoName;
            document.getElementById('<%= hfDeleteAction.ClientID %>').value = 'delete';
            document.getElementById('form1').submit();
        }

        function cancelDelete() {
            pendingPhotoId = null;
            pendingPhotoName = null;
            const modal = document.getElementById('deleteConfirmModal');
            modal.classList.remove('active');
        }

        // Close modal when clicking outside the modal content
        document.addEventListener('DOMContentLoaded', function () {
            const lightboxOverlay = document.getElementById('lightboxOverlay');
            lightboxOverlay.addEventListener('click', function (event) {
                if (event.target === lightboxOverlay) {
                    closeLightbox();
                }
            });

            const deleteModal = document.getElementById('deleteConfirmModal');
            deleteModal.addEventListener('click', function (event) {
                if (event.target === deleteModal) {
                    cancelDelete();
                }
            });

            // Close lightbox on Escape key
            document.addEventListener('keydown', function (event) {
                if (event.key === 'Escape') {
                    closeLightbox();
                    cancelDelete();
                }
            });
        });
    </script>
</body>
</html>