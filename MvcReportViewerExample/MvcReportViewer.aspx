<%@ Page Language="C#" AutoEventWireup="true" Inherits="MvcReportViewer.MvcReportViewer, MvcReportViewer" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="reportForm" runat="server">
    <div>
        <asp:ScriptManager runat="server"></asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer" ClientIDMode="Predictable" runat="server"></rsweb:ReportViewer>
    </div>
    </form>

    <script type="text/javascript" src="//code.jquery.com/jquery-1.11.0.min.js"></script>
    <script type="text/javascript" src="//code.jquery.com/jquery-migrate-1.2.1.min.js"></script>
    <script type="text/javascript">
        // Detect is current browser is IE (MSIE is not used since IE 11)
        var isIE = /MSIE/i.test(navigator.userAgent) || /rv:11.0/i.test(navigator.userAgent);

        if (hasUserSetHeight && isIE) {
            Sys.Application.add_load(function () {
                var reportViewer = $find("ReportViewer");
                reportViewer.add_propertyChanged(viewerPropertyChanged);
            });

            function viewerPropertyChanged(sender, e) {
                var viewer = $find("ReportViewer");

                if (e.get_propertyName() === "isLoading" && !viewer.get_isLoading()) {
                    var reportViewerHeight = $('#ReportViewer').height();
                    var $uiRows = $('#ReportViewer_fixedTable > tbody > tr');
                    var controlsHeight = 0;
                    $uiRows.each(function (i, el) {
                        if (i != $uiRows.length - 1) {
                            controlsHeight += $(el).height();
                        }
                    });

                    var contentAreaHeight = reportViewerHeight - controlsHeight;
                    $('#VisibleReportContentReportViewer_ctl09').height(contentAreaHeight);
                }
            }
        }
    </script>
</body>
</html>
