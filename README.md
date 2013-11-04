MvcReportViewer HTML Helper
===========================

Why?
----

SQL Server Reporting Services is rich and popular reporting solution that you have free with SQL Server. It is widely used in the industry: from small family businesses running on SQL Server 2008/2012 express to huge corporations with SQL Server clusters.

There is one issue with the solution. Microsoft has not release SSRS viewer for ASP.NET MVC yet. That is why people usually mixing modern ASP.NET MVC enterprise applications with ASP.NET Web Forms pages to view report.

The project solves this issue. We provided a simple ASP.NET Web Forms report viewer and ASP.NET MVC HTML helpers show it inside an iframe tag.

Installation
------------

Usage
-----

### Configuration
* Make sure you reference Microsoft.ReportViewer.WebForm (Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91) and MvcReportViewer assemblies in the application.
* Configure the ASP.NET Web Forms Report Viewer in the web.config.
** Add *<add path="Reserved.ReportViewerWebControl.axd" verb="*" type="Microsoft.Reporting.WebForms.HttpHandler, Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" validate="false"/>* to *system.web/httpHandlers* section.
** Add *<remove name="ReportViewerWebControlHandler" /> <add name="ReportViewerWebControlHandler" preCondition="integratedMode" verb="*" path="Reserved.ReportViewerWebControl.axd" type="Microsoft.Reporting.WebForms.HttpHandler, Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"/>* to *system.webServer/handlers* section.
* Configure MvcReportViewer HTML helper in the web.config.

	<!-- Required by Microsoft ReportViewer control -->
    <add key="MvcReportViewer.AspxViewer" value="/MvcReportViewer.aspx" />
    <add key="MvcReportViewer.ReportServerUrl" value="http://localhost/ReportServer_SQLEXPRESS" />
    <add key="MvcReportViewer.Username" value="" />
    <add key="MvcReportViewer.Password" value="" />
    <add key="MvcReportViewer.ShowParameterPrompts" value="False" />
	

### Basic Interface

### Fluent Interface
