<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <span class="title1">LEAGUE OF LORDS - ACCOUNT SYSTEM</span><hr />
    <% using (Html.BeginForm(null, null, new { @ReturnUrl = Request.QueryString["ReturnUrl"] }, FormMethod.Post, new { @id = "mainForm", @name = "mainForm", @autocomplete = "off" }))
       { %>
    <table class="tableform">
        <tr><td colspan="2"><span class="title3">Account Login</span></td></tr>
        <tr>
            <td></td>
            <td><%= Html.ValidationSummary("Đăng nhập thất bại!!!")%></td>
        </tr>
        <tr>
            <td>UserName</td>
            <td>
                <%= Html.TextBox("username", "", new { @style = "width:160px", @autocomplete = "off" })%>
                <%= Html.ValidationMessage("username", "*")%>
            </td>
        </tr>
        <tr>
            <td>Password</td>
            <td>
                <%= Html.Password("password", "", new { @style = "width:160px" })%>
                <%= Html.ValidationMessage("password", "*")%>
            </td>
        </tr>
        <tr>
            <td></td>
            <td><input id="Submit" type="submit" value="Sign In" /></td>
        </tr>
        <tr>
            <td></td>
            <td>
                <%= Html.ActionLink("Forgot Pass?", "ForgotPass")%>
                <%= Html.ActionLink("Register", "Register")%>
            </td>
        </tr>
    </table>
    <% } %>
</asp:Content>
