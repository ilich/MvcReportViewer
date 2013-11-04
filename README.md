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
* Configure the ASP.NET Web Forms Report Viewer in the web.config.<br><br>
Add *<add path="Reserved.ReportViewerWebControl.axd" verb="*" type="Microsoft.Reporting.WebForms.HttpHandler, Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" validate="false"/>* to *system.web/httpHandlers* section.<br><br>
Add *<remove name="ReportViewerWebControlHandler" /> <add name="ReportViewerWebControlHandler" preCondition="integratedMode" verb="*" path="Reserved.ReportViewerWebControl.axd" type="Microsoft.Reporting.WebForms.HttpHandler, Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"/>* to *system.webServer/handlers* section.
* Configure MvcReportViewer HTML helper in the web.config.
```
<!-- Required by Microsoft ReportViewer control -->

<add key="MvcReportViewer.AspxViewer" value="/MvcReportViewer.aspx" />

<add key="MvcReportViewer.ReportServerUrl" value="http://localhost/ReportServer_SQLEXPRESS" />

<add key="MvcReportViewer.Username" value="" />

<add key="MvcReportViewer.Password" value="" />

<add key="MvcReportViewer.ShowParameterPrompts" value="False" />
```

### Basic Interface

### Fluent Interface

License
-------

The MIT License (MIT)

Copyright (c) 2013 Ilya Verbitskiy

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.