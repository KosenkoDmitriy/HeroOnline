<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="Recaptcha.Web.Mvc" %>
<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <span class="title1">League of Lords - Register</span><hr />
    <% using (Html.BeginForm(null, null, FormMethod.Post, new { @id = "mainForm", @name = "mainForm", @autocomplete = "off", onsubmit = "" }))
       { %>
    <table class="tableform">
        <tr>
            <td colspan="2">
                <%= Html.ValidationSummary()%>
            </td>
        </tr>
        <tr>
            <td style="width:160px">User Name</td>
            <td>
                <%= Html.TextBox("username") %>
            </td>
        </tr>
        <tr>
            <td>Password
            </td>
            <td>
                <%= Html.TextBox("password") %>
            </td>
        </tr>
        <tr>
            <td>Password (confirm)
            </td>
            <td>
                <%= Html.TextBox("passwordconfirm") %>
            </td>
        </tr>
        <tr>
            <td></td>
            <td>
                <!-- Html.Recaptcha(theme:Recaptcha.Web.RecaptchaTheme.White) -->
            </td>
        </tr>
        <tr>
            <td></td>
            <td>
                <input type="submit" value="Đăng ký" style="width: 180px; height: 50px" />
            </td>
        </tr>
    </table>
    <% } %>
    <script type="text/javascript" src="/Scripts/jquery-2.0.2.min.js"></script>
    <script type="text/javascript">
        var theForm = document.forms['mainForm'];
        if (!theForm) theForm = document.mainForm;

        //function check_submit() {
        //    return true;
        //}
    </script>
</asp:Content>
