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
            <td colspan="2"><%= Html.ActionLink("Trang chính", "Index", "Home")%></td>
        </tr>
        <tr>
            <td colspan="2"><hr /></td>
        </tr>
        <tr>
            <td colspan="2"><%= Html.ActionLink("Log Tài khoản", "Account", "GameLog")%></td>
        </tr>
        <tr>
            <td colspan="2"><hr /></td>
        </tr>
        <tr>
            <td colspan="2"><%= Html.ActionLink("Log Nhân vật", "Role", "GameLog")%></td>
        </tr>
        <tr>
            <td colspan="2"><hr /></td>
        </tr>
        <tr>
            <td colspan="2"><%= Html.ActionLink("Log Giao dịch", "Trade", "GameLog")%></td>
        </tr>
        <tr>
            <td colspan="2"><hr /></td>
        </tr>
        <tr>
            <td colspan="2"><%= Html.ActionLink("Log Gửi thư", "SendMail", "GameLog")%></td>
        </tr>
        <tr>
            <td colspan="2"><hr /></td>
        </tr>
        <tr>
            <td colspan="2"><%= Html.ActionLink("Log Càn Khôn (GShop)", "GShop", "GameLog")%></td>
        </tr>
        <tr>
            <td colspan="2"><hr /></td>
        </tr>
        <tr>
            <td colspan="2"><%= Html.ActionLink("Log Chuyển Server", "MoveRole", "GameLog")%></td>
        </tr>
        <tr>
            <td colspan="2"><hr /></td>
        </tr>
        <tr>
            <td colspan="2"><%= Html.ActionLink("Download RoleLog", "Download", "GameLog")%></td>
        </tr>
        <tr>
            <td colspan="2"><hr /></td>
        </tr>
    </table>
<% } %>
