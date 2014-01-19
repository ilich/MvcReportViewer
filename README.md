MvcReportViewer HTML Helper
===========================

Why?
----

SQL Server Reporting Services is rich and popular reporting solution that you have free with SQL Server. It is widely used in the industry: from small family businesses running on SQL Server 2008/2012 express to huge corporations with SQL Server clusters.

There is one issue with the solution. Microsoft has not release SSRS viewer for ASP.NET MVC yet. That is why people usually mixing modern ASP.NET MVC enterprise applications with ASP.NET Web Forms pages to view report.

The project solves this issue. We provided a simple ASP.NET Web Forms report viewer and ASP.NET MVC HTML helpers show it inside an iframe tag.

Installation
------------

Install **MvcReportViewer** package from NuGet.

Usage
-----

### Configuration
* Make sure you reference **Microsoft.ReportViewer.WebForm (Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91)** and **MvcReportViewer** assemblies in the application.

* Configure the ASP.NET Web Forms Report Viewer in the web.config.<br><br>
Add **&lt;add path="Reserved.ReportViewerWebControl.axd" verb="&#42;" type="Microsoft.Reporting.WebForms.HttpHandler, Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" validate="false"/&gt;** to **system.web/httpHandlers** section.<br><br>
Add **&lt;remove name="ReportViewerWebControlHandler" /&gt; &lt;add name="ReportViewerWebControlHandler" preCondition="integratedMode" verb="&#42;" path="Reserved.ReportViewerWebControl.axd" type="Microsoft.Reporting.WebForms.HttpHandler, Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"/&gt;** to **system.webServer/handlers** section.

* Configure MvcReportViewer HTML helper in the web.config.

<pre><code>&lt;!-- Required by Microsoft ReportViewer control --&gt;
&lt;add key="MvcReportViewer.AspxViewer" value="/MvcReportViewer.aspx" /&gt;
&lt;add key="MvcReportViewer.ReportServerUrl" value="http://localhost/ReportServer_SQLEXPRESS" /&gt;
&lt;add key="MvcReportViewer.Username" value="" /&gt;
&lt;add key="MvcReportViewer.Password" value="" /&gt;
</code></pre>

**MvcReportViewer.AspxViewer** - Path to the Report Viewer page shown in the iframe. Its name is **MvcReportViewer.aspx** and it is in the application's root by default.

**MvcReportViewer.ReportServerUrl** - Default SSRS URL.

**MvcReportViewer.Username** - Default SSRS username.

**MvcReportViewer.Password** - Default SSRS password.

### Basic Interface

<pre><code>@Html.MvcReportViewer(
    "/Reports/TestReport",
    new { Parameter1 = "Hello World!", Parameter2 = DateTime.Now, Parameter3 = 12345 },
    new { Height = 900, Width = 900, style = "border: none" })</code></pre>
	
**@Html.MvcReportViewer(string reportPath, object htmlAttributes)**

*reportPath* - The path to the report on the server.

*htmlAttributes* - An object that contains the HTML attributes to set for the element.

**@Html.MvcReportViewer(string reportPath, object reportParameters, object htmlAttributes)**

*reportPath* - The path to the report on the server.

*reportParameters* - The report parameter properties for the report.

*htmlAttributes* - An object that contains the HTML attributes to set for the element.

**@Html.MvcReportViewer(string reportPath, IEnumerable&lt;KeyValuePair&lt;string, object&gt;&gt; reportParameters, object htmlAttributes)**

*reportPath* - The path to the report on the server.

*reportParameters* - The report parameter properties for the report.

*htmlAttributes* - An object that contains the HTML attributes to set for the element.

**@Html.MvcReportViewer(string reportPath, string reportServerUrl = null, string username = null, string password = null, object reportParameters = null, ControlSettings controlSettings = null, object htmlAttributes = null, FormMethod method = FormMethod.Get)**

*reportPath* - The path to the report on the server.

*reportServerUrl* -  The URL for the report server.

*username* - The report server username.

*password* - The report server password.

*reportParameters* - The report parameter properties for the report.

*controlSettings* - The Report Viewer control's UI settings.

*htmlAttributes* - An object that contains the HTML attributes to set for the element.

*method* - Method for sending parametes to the iframe, either GET or POST.

### Fluent Interface

<pre><code>@Html.MvcReportViewerFluent("/Reports/TestReport")
     .ReportParameters(new { Parameter1 = "Hello World!", Parameter2 = DateTime.Now, Parameter3 = 12345 })
     .Attributes(new { Height = 900, Width = 900, style = "border: none" })</pre></code>

**@Html.MvcReportViewerFluent(string reportPath)**

*reportPath* - The path to the report on the server.

The method return Fluent interface to show to Report Viewer configuration.

_**Fluent Interface Methods**_

**ReportPath(string reportPath)** - Sets the path to the report on the server.

**ReportServerUrl(string reportServerUrl)** - Sets the URL for the report server.

**Username(string username)** - Sets the report server username.

**Password(string password)** - Sets the report server password.

**ReportParameters(object reportParameters)** - Sets the report parameter properties for the report.

**ReportParameters(IEnumerable&lt;KeyValuePair&lt;string, object&gt;&gt; reportParameters)** - Sets the report parameter properties for the report.

**ControlSettings(ControlSettings settings)** - Sets ReportViewer control UI parameters

**Attributes(object htmlAttributes)** - Sets an object that contains the HTML attributes to set for the element.

**Method(FormMethod method)** - Sets the method for sending parameters to the iframe, either GET or POST. POST should be used to send long arguments, etc. Use GET otherwise.

### Controller Extensions

There is a possibility to download SSRS reports in MS Word, MS Excel, PDF or Image format directly from your controller. It is available via **Report** extension method for ASP.NET MVC **Controller** class. The method always returns an instance of **FileStreamResult** class.

1. **FileStreamResult Report(ReportFormat reportFormat, string reportPath)**
2. **FileStreamResult Report(ReportFormat reportFormat, string reportPath, object reportParameters)**
3. **FileStreamResult Report(ReportFormat reportFormat, string reportPath, string reportServerUrl, string username = null, string password = null, object reportParameters = null)**
4. **FileStreamResult Report(this Controller controller, ReportFormat reportFormat, string reportPath, IEnumerable&lt;KeyValuePair&lt;string, object&gt;&gt; reportParameters)**
5. **FileStreamResult Report(this Controller controller, ReportFormat reportFormat, string reportPath, string reportServerUrl, IEnumerable&lt;KeyValuePair&lt;string, object&gt;&gt; reportParameters, string username = null, string password = null)**

Where *reportFormat* might be *ReportFormat.Excel*, *ReportFormat.Word*, *ReportFormat.PDF* or *ReportFormat.Image*.

The following code allows user to download the report in MS Excel format.

<pre><code>public ActionResult DownloadExcel()
{
    return this.Report(
	    ReportFormat.Excel,
		"/Reports/TestReport",
		new { Parameter1 = "Hello World!", Parameter2 = DateTime.Now, Parameter3 = 12345 });
}</pre></code>

License
-------

The MIT License (MIT)

Copyright (c) 2013-2014 Ilya Verbitskiy

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