<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% if (Request.IsAuthenticated) { %>
    <table border="0" style="border-collapse: collapse; width: 100%">
        <tr>
            <td colspan="2">
                <span class="title2">.:: LOL ACCOUNT ::.</span>
            </td>
        </tr>
        <tr>
            <td colspan="2"><hr /></td>
        </tr>
        <tr>
            <td colspan="2"><a href="<%=Url.Action("Index", "Manager")%>">Trang chính</a></td>
        </tr>
        <tr>
            <td colspan="2"><hr /></td>
        </tr>
        <tr>
            <td colspan="2"><a href="<%=Url.Action("ViewReports", "Manager", new { @id=1 })%>">Online Users</a></td>
        </tr>
        <tr>
            <td colspan="2"><hr /></td>
        </tr>
        <tr>
            <td colspan="2"><a href="<%=Url.Action("ViewReports", "Manager", new { @id=2 })%>">Register Users</a></td>
        </tr>
        <tr>
            <td colspan="2"><hr /></td>
        </tr>
    </table>
<% } %>
