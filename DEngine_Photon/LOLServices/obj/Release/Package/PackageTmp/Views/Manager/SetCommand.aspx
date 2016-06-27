<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <span class="title1">LEAGUE OF LORDS - MANAGER SYSTEM</span><hr />
    <% using (Html.BeginForm(null, null, new { @ReturnUrl = Request.QueryString["ReturnUrl"] }, FormMethod.Post, new { @id = "mainForm", @name = "mainForm", @autocomplete = "off" }))
       { %>
    <table class="tableform">
        <tr><td colspan="2"><span class="title3">Set Command</span></td></tr>
        <tr>
            <td></td>
            <td><%=ViewData["Message"]%><%= Html.ValidationSummary("SetCommand Failed!")%></td>
        </tr>
        <tr>
            <td>WorldId</td>
            <td>
                <%= Html.TextBox("worldid", "", new { @style = "width:160px" })%>
            </td>
        </tr>
        <tr>
            <td>UserId</td>
            <td>
                <%= Html.TextBox("userid", "", new { @style = "width:160px" })%>
            </td>
        </tr>
        <tr>
            <td>CommandCode</td>
            <td>
                <%= Html.DropDownList("cmdcode", null, new { @style = "width:160px;" })%>
            </td>
        </tr>
        <tr>
            <td>Parameters</td>
            <td>
                <%= Html.TextBox("cmdparams", "", new { @style = "width:160px" })%>
            </td>
        </tr>
        <tr>
            <td></td>
            <td><input id="Submit" type="submit" value="Send Command" /></td>
        </tr>
    </table>
    <% } %>
</asp:Content>
