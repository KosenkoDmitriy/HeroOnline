<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Báo cáo</h2>
    <% if (Model != null)
       { %>
    <table class="tablelist">
        <% foreach (var item in Model)
           { %>
        <tr>
            <%= item%>
        </tr>
        <% } %>
    </table>
    <% } %>
</asp:Content>
