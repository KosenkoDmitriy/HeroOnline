<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% if (Request.IsAuthenticated) { %>
  Chào bạn <span class="acc_code"><%= Html.Encode(Page.User.Identity.Name) %></span> |
  <b><%= Html.ActionLink("SignOut", "SignOut", "Home") %></b>
<% } %>
