<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<WMGameLog.Helpers.PaginatedList<String>>" %>

<% if (Model.HasPreviousPage) { %>
    <a href="javascript:gotoPage(1)" title="First Page"><img alt="" src="/Images/icon_first.gif" border="0" style="cursor: hand; vertical-align:bottom;" /></a>&nbsp;&nbsp;
    <a href="javascript:gotoPage(<%=Model.PageIndex %>)" title="Previous Page"><img alt="" src="/Images/icon_prev.gif" border="0" style="cursor: hand; vertical-align:bottom;" /></a>
<% } %>
  &nbsp;&nbsp;Page <%= Model.PageIndex + 1%> / <%= Model.TotalPages %>&nbsp;&nbsp;
<% if (Model.HasNextPage) { %>
    <a href="javascript:gotoPage(<%= Model.PageIndex + 2 %>)" title="Next Page"><img alt="" src="/Images/icon_next.gif" border="0" style="cursor: hand; vertical-align:bottom;" /></a>&nbsp;&nbsp;
    <a href="javascript:gotoPage(<%= Model.TotalPages %>)" title="Last Page"><img alt="" src="/Images/icon_last.gif" border="0" style="cursor: hand; vertical-align:bottom;" /></a>
<% } %>
<script type="text/javascript" language="javascript">
<!--
  var theForm = document.forms['mainForm'];
  if (!theForm) theForm = document.mainForm;

  function gotoPage(page) {
      theForm.Page.value = page-1;
      theForm.submit();
  }
-->
</script>
