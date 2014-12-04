(function () {
    // ASP.NET Report Viewer Web Control Enhancements

    $(document).ready(function () {
        // Detect is current browser is IE (MSIE is not used since IE 11)
        var isIE = /MSIE/i.test(navigator.userAgent) || /rv:11.0/i.test(navigator.userAgent);

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

        function printReport() {
            var styles = $('#ReportViewer_ctl09_ReportControl_styles').html();
            styles = 'body{ margin: 10px; padding: 0; } ' + styles;

            var report = $('#ReportViewer_ctl09').html();

            // Open report popup
            var popup = window.open("", "_blank", "location=no, menubar=no, resizable=yes, scrollbars=yes, status=no, toolbar=no, title=yes, widht=300, height=500");
            var content = popup.document;
            content.open();
            content.write(
                '<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">' +
                '<html>' +
                '<head><title>Printing Report</title><style type="text/css">' +
                styles +
                '</style></head>' +
                '<body onload="window.print();">' +
                report +
                '</body></html>');

            content.close();
            popup.focus();
        }

        // 1. Fix Report Scrolling in IE 10 and IE 11
        if (window.hasUserSetHeight && isIE) {
            Sys.Application.add_load(function () {
                var reportViewer = $find("ReportViewer");
                reportViewer.add_propertyChanged(viewerPropertyChanged);
            });
        }

        // 2. Add Print button for non-IE browsers
        if (!isIE) {
            var buttonHtml = $('#non-ie-print-button').html();
            $('#ReportViewer_ctl05 > div').append(buttonHtml);
            $('#print-button').click(function (e) {
                e.preventDefault();
                printReport();
            })
        }
    });
    
})();