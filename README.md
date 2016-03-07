MvcReportViewer HTML Helper
===========================

Why?
----

SQL Server Reporting Services is rich and popular reporting solution that you have for free with SQL Server. It is widely used in the industry: from small family businesses running on SQL Server 2008/2012 express to huge corporations with SQL Server clusters.

There is one issue with the solution. Microsoft has not release SSRS viewer for ASP.NET MVC yet. That is why people usually mixing modern ASP.NET MVC enterprise applications with ASP.NET Web Forms pages to view report.

The project solves this issue. We provided a simple ASP.NET Web Forms report viewer and ASP.NET MVC HTML helpers show it inside an iframe tag.

Installation
------------

* Install **Microsoft® System CLR Types for Microsoft® SQL Server® 2012** if needed. Go to https://www.microsoft.com/en-us/download/details.aspx?id=29065 and scroll page to **Microsoft® System CLR Types for Microsoft® SQL Server® 2012**.
* Install [Microsoft Report Viewer 2012](http://www.microsoft.com/en-us/download/details.aspx?id=35747). The library has be deployed to developer machines and to servers.
* Install **MvcReportViewer** package from NuGet.

Usage
-----

### Configuration
* Make sure you reference **Microsoft.ReportViewer.WebForm (Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91)** and **MvcReportViewer** assemblies in the application.

* Configure the ASP.NET Web Forms Report Viewer in the web.config.<br><br>
Add **&lt;add path="Reserved.ReportViewerWebControl.axd" verb="&#42;" type="Microsoft.Reporting.WebForms.HttpHandler, Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" validate="false"/&gt;** to **system.web/httpHandlers** section.<br><br>
Add **&lt;remove name="ReportViewerWebControlHandler" /&gt; &lt;add name="ReportViewerWebControlHandler" preCondition="integratedMode" verb="&#42;" path="Reserved.ReportViewerWebControl.axd" type="Microsoft.Reporting.WebForms.HttpHandler, Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"/&gt;** to **system.webServer/handlers** section.

* Configure MvcReportViewer HTML helper in the web.config. There are two ways of doind this. You can use application settings or MvcReportViewer configuration section.

#### MvcReportViewer configuration section

```xml
<configSections>
    <section name="MvcReportViewer" type="MvcReportViewer.Configuration.MvcReportViewerSettings, MvcReportViewer"/>
</configSections>
<MvcReportViewer reportServerUrl="http://localhost:60002/ReportServer"
    username="" 
    password=""
    aspxViewer="~/MvcReportViewer.aspx"
    aspxViewerJavaScript="~/Scripts/MvcReportViewer.js"
    errorPage="~/MvcReportViewerErrorPage.html"
    showErrorPage="false"
    isAzureSSRS="false"
    encryptParameters="true"
    localDataSourceProvider="MvcReportViewer.SessionLocalDataSourceProvider, MvcReportViewer" />
```

#### Application Settings 

```xml
<!-- Required by Microsoft ReportViewer control -->
<add key="MvcReportViewer.AspxViewer" value="~/MvcReportViewer.aspx" />
<add key="MvcReportViewer.AspxViewerJavaScript" value="~/Scripts/MvcReportViewer.js" />
<add key="MvcReportViewer.ErrorPage" value="~/MvcReportViewerErrorPage.html" />
<add key="MvcReportViewer.ShowErrorPage" value="False" />
<add key="MvcReportViewer.ReportServerUrl" value="http://localhost:60002/ReportServer" />
<add key="MvcReportViewer.Username" value="" />
<add key="MvcReportViewer.Password" value="" />
<add key="MvcReportViewer.EncryptParameters" value="True" />
<add key="MvcReportViewer.IsAzureSSRS" value="false" />
<add key="MvcReportViewer.LocalDataSourceProvider" value="MvcReportViewer.SqlLocalDataSourceProvider, MvcReportViewer" />
<add key="SqlLocalDataSourceProvider.ConnectionString" value="Products" />
```

#### Description

**MvcReportViewer.AspxViewer (aspxViewer)** - Path to the Report Viewer page shown in the iframe. Its name is **MvcReportViewer.aspx** and it is in the application's root by default.

**MvcReportViewer.AspxViewerJavaScript (aspxViewerJavaScript)** - Path to Report Viewer JavaScript code which adds additional features to Report Viewer control (e.g. printing support for non-IE browsers, etc.)

**MvcReportViewer.ErrorPage (errorPage)** - Path to Report Viewer Error page which is shown if an exception occurs. The exception is logged using [ASP.NET Trace functionality](http://msdn.microsoft.com/en-us/library/bb386420%28v=vs.100%29.aspx).

**MvcReportViewer.ShowErrorPage (showErrorPage)** - Enable/Disable custom error page.

**MvcReportViewer.ReportServerUrl (reportServerUrl)** - Default SSRS URL.

**MvcReportViewer.Username (username)** - Default SSRS username.

**MvcReportViewer.Password (password)** - Default SSRS password.

**MvcReportViewer.EncryptParameters (encryptParameters)** - Report Viewer parameters will be encrypted if it is set to True. It is False by default.

**MvcReportViewer.IsAzureSSRS (isAzureSSRS)** - Use SSRS service hosted on Windows Azure.

**MvcReportViewer.LocalDataSourceProvider (localDataSourceProvider)** - Local Report Data Source provider. The provider has to implement ILocalReportDataSourceProvider interface. You can also register ILocalReportDataSourceProvider via ASP.NET MVC Dependency Injection. See [NinjectWebCommon.cs](https://github.com/ilich/MvcReportViewer/blob/master/MvcReportViewerExample/App_Start/NinjectWebCommon.cs) for details.

**SqlLocalDataSourceProvider.ConnectionString** - Connection string used in SqlLocalDataSourceProvider to query data.

### Basic Interface

Reference **MvcReportViewer** in your view

```csharp
@using MvcReportViewer
```

Then you can use our bultin HtmlHelpers as follows:

```csharp
@Html.MvcReportViewer(
    "/Reports/TestReport",
    new { Parameter1 = "Hello World!", Parameter2 = DateTime.Now, Parameter3 = 12345 },
    new { Height = 900, Width = 900, style = "border: none" })
```
	
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

You **MUST** use this method if your report parameters contants underscores.

**@Html.MvcReportViewer(string reportPath, string reportServerUrl = null, string username = null, string password = null, IEnumerable<KeyValuePair<string, object>> reportParameters = null, ControlSettings controlSettings = null, object htmlAttributes = null, FormMethod method = FormMethod.Get)**

*reportPath* - The path to the report on the server.

*reportServerUrl* -  The URL for the report server.

*username* - The report server username.

*password* - The report server password.

*reportParameters* - The report parameter properties for the report.

*controlSettings* - The Report Viewer control's UI settings.

*htmlAttributes* - An object that contains the HTML attributes to set for the element.

*method* - Method for sending parametes to the iframe, either GET or POST.

You **MUST** use this method if your report parameters contants underscores.

### Fluent Interface

```csharp
@Html.MvcReportViewerFluent("/Reports/TestReport")
     .ReportParameters(new { Parameter1 = "Hello World!", Parameter2 = DateTime.Now, Parameter3 = 12345 })
     .Attributes(new { Height = 900, Width = 900, style = "border: none" })
```

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

**ReportParameters(IEnumerable&lt;ReportParameter&gt; reportParameters)** - Sets the report parameter properties for the report. You **MUST** use this method if your report parameters contants underscores.

**ControlSettings(ControlSettings settings)** - Sets ReportViewer control UI parameters

**Attributes(object htmlAttributes)** - Sets an object that contains the HTML attributes to set for the element.

**Method(FormMethod method)** - Sets the method for sending parameters to the iframe, either GET or POST. POST should be used to send long arguments, etc. Use GET otherwise.

**ProcessingMode(ProcessingMode mode)** - Sets ReportViewer report processing mode. Default processing mode is ProcessingMode.Remote.

**EventsHandlerType(Type type)** - Sets the type implementing IReportViewerEventsHandler interface. The instance of the type is responsible for processing Report Viewer Web Control's events, e.g. SubreportProcessing. Review [Subreport.cshtml](https://github.com/ilich/MvcReportViewer/blob/master/MvcReportViewerExample/Views/Home/Subreport.cshtml), [SubreportEventHandlers.cs](https://github.com/ilich/MvcReportViewer/blob/master/MvcReportViewerExample/Models/SubreportEventHandlers.cs) and [HomeController.cs](https://github.com/ilich/MvcReportViewer/blob/master/MvcReportViewerExample/Controllers/HomeController.cs) for the information how to use the feature.

**SetDataSourceCredentials(DataSourceCredentials[] credentials)** - Sets data source credentials for the report. 

**LocalDataSource&lt;T&gt;(string dataSourceName, T dataSource)** - Registers local report data source. The default local data source provider (MvcReportViewer.SessionLocalDataSourceProvider) stores data in user session. There is also MvcReportViewer.SqlLocalDataSourceProvider provider which stores SQL queries. You have to add SqlLocalDataSourceProvider.ConnectionString application configuration to your Web.config file to be able to use this provider. The configuration values is your connection string name. Please check MvcReportViewer example for further information.

### Controller Extensions

There is a possibility to download SSRS reports in MS Word, MS Excel, PDF or Image format directly from your controller. It is available via **Report** extension method for ASP.NET MVC **Controller** class. The method always returns an instance of **FileStreamResult** class.

1. **FileStreamResult Report(ReportFormat reportFormat, string reportPath, ProcessingMode mode = ProcessingMode.Remote, IDictionary&lt;string, DataTable&gt localReportDataSources = null, string filename = null)**
2. **FileStreamResult Report(ReportFormat reportFormat, string reportPath, object reportParameters, ProcessingMode mode = ProcessingMode.Remote, IDictionary&lt;string, DataTable&gt localReportDataSources = null, string filename = null)**
3. **FileStreamResult Report(ReportFormat reportFormat, string reportPath, string reportServerUrl, string username = null, string password = null, object reportParameters = null, ProcessingMode mode = ProcessingMode.Remote, IDictionary&lt;string, DataTable&gt localReportDataSources = null, string filename = null)**
4. **FileStreamResult Report(this Controller controller, ReportFormat reportFormat, string reportPath, IEnumerable&lt;KeyValuePair&lt;string, object&gt;&gt; reportParameters, ProcessingMode mode = ProcessingMode.Remote, IDictionary&lt;string, DataTable&gt localReportDataSources = null, string filename = null)**
5. **FileStreamResult Report(this Controller controller, ReportFormat reportFormat, string reportPath, string reportServerUrl, IEnumerable&lt;KeyValuePair&lt;string, object&gt;&gt; reportParameters, string username = null, string password = null, ProcessingMode mode = ProcessingMode.Remote, IDictionary&lt;string, DataTable&gt localReportDataSources = null, string filename = null)**

Where *reportFormat* might be *ReportFormat.Excel*, *ReportFormat.Word*, *ReportFormat.PDF* or *ReportFormat.Image*.

The following code allows user to download the report in MS Excel format.

```csharp
public ActionResult DownloadExcel()
{
    return this.Report(
        ReportFormat.Excel,
        "/Reports/TestReport",
        new { Parameter1 = "Hello World!", Parameter2 = DateTime.Now, Parameter3 = 12345 });
}
```

How to build source code
------------------------

* Install Visual Studio 2015.
* Install **Microsoft® System CLR Types for Microsoft® SQL Server® 2012** if needed. Go to https://www.microsoft.com/en-us/download/details.aspx?id=29065 and scroll page to **Microsoft® System CLR Types for Microsoft® SQL Server® 2012**.
* Install [Microsoft Report Viewer 2012](http://www.microsoft.com/en-us/download/details.aspx?id=35747).
* Download source code from GitHub.
* Open MvcReportViewer.sln and run the build inside Visual Studio 2015.

License
-------

The MIT License (MIT)

Copyright (c) 2013-2015 Ilya Verbitskiy

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
