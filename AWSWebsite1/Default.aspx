<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="AWSWebsite1.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHTMLHead" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="server">
    <div class="container">
        <header>
            <h1>
                Welcome to Photo Album Management Website 
                <span>on AWS EC2</span>
            </h1>
        </header>
        <section>
            <div id="container_demo">
                <!-- hidden anchor to stop jump -->
                <a class="hiddenanchor" id="toregister"></a>
                <a class="hiddenanchor" id="tologin"></a>
                <div id="wrapper">
                    <!-- LOGIN FORM -->
                    <div id="login" class="animate form">
                        <form id="frmLogin" autocomplete="on">
                            <h1>Log in</h1>
                            <p>
                                <label for="username" class="uname" data-icon="u">
                                    Your email
                                </label>
                                <input id="username" name="username"
                                       required="required" type="text"
                                       placeholder="mymail@mail.com" />
                            </p>
                            <p>
                                <label for="password" class="youpasswd" data-icon="p">
                                    Your password
                                </label>
                                <input id="password" name="password"
                                       required="required" type="password"
                                       placeholder="eg. X8df!90EO" />
                            </p>
                            <p class="login button">
                                <input type="submit" value="Login" />
                            </p>
                            <p class="change_link">
                                Not a member yet ?
                                <a href="#toregister" class="to_register">
                                    Join us
                                </a>
                            </p>
                        </form>
                    </div>

                    <!-- REGISTER FORM -->
                    <div id="register" class="animate form">
                        <form id="frmRegister" autocomplete="on">
                            <h1>Sign up</h1>
                            <p>
                                <label for="usernamesignup" class="uname" data-icon="u">
                                    Full Name
                                </label>
                                <input id="usernamesignup" name="FullName"
                                       required="required" type="text"
                                       placeholder="First & Last Name" />
                            </p>
                            <p>
                                <label for="emailsignup" class="youmail" data-icon="e">
                                    Your email
                                </label>
                                <input id="emailsignup" name="EmailID"
                                       required="required" type="email"
                                       placeholder="mysupermail@mail.com" />
                            </p>
                            <p>
                                <label for="passwordsignup" class="youpasswd" data-icon="p">
                                    Your password
                                </label>
                                <input id="passwordsignup" name="Password"
                                       required="required" type="password"
                                       placeholder="eg. X8df!90EO" />
                            </p>
                            <p>
                                <label for="passwordsignup_confirm" class="youpasswd"
                                       data-icon="p">
                                    Please confirm your password
                                </label>
                                <input id="passwordsignup_confirm"
                                       name="Password_Confirm"
                                       required="required" type="password"
                                       placeholder="eg. X8df!90EO" />
                            </p>
                            <p class="signin button">
                                <input type="submit" value="Sign up" />
                            </p>
                            <p class="change_link">
                                Already a member ?
                                <a href="#tologin" class="to_register">
                                    Go and log in
                                </a>
                            </p>
                        </form>
                    </div>
                </div>
            </div>
        </section>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphFooter" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            //login call
            $("#frmLogin").submit(function () {
                $.post("Default.aspx", "method=login&" + $(this).serialize(), function (resp) {
                    if (resp.IsSuccess) {
                        window.location = "profile.aspx";
                    }
                    else alert(resp.Message);
                });
                return false;
            });

            //sign up call
           
            $("#frmRegister").submit(function () {
                $.ajax({
                    url: "Default.aspx",
                    type: "POST",
                    data: "method=signup&" + $(this).serialize(),
                    dataType: "json",  // ✅ tells jQuery to auto-parse JSON
                    success: function (resp) {
                        if (resp.IsSuccess) {
                            alert("Registration successful! Please login.");
                            window.location.hash = "tologin";
                        } else {
                            alert(resp.Message);
                        }
                    },
                    error: function (xhr) {
                        alert("Server error: " + xhr.status + " - " + xhr.responseText);
                    }
                });
                return false;
            });
        });
    </script>
</asp:Content>
