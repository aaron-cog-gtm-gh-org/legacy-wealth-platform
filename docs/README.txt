PORTFOLIO VIEWER
================

Internal holdings/settlement viewer for the ops desk. Reads the daily
custodian statement + confirmation files (MT535 / MT54x) dropped to the
archive share, and the reference data in SQL Server.

RJM 2014-01. Last edited KP 2017-03.


BUILD
-----
Open PortfolioViewer.sln in Visual Studio 2013 or 2015. Build the solution,
set PV.Web as the startup project, F5. IIS Express is configured in the
project file.

From the command line use build.bat (assumes VS2017 Pro is installed at the
default path; otherwise set MSBUILD to a full MSBuild.exe path).


DATABASE
--------
Reference data lives in SQL Server. Prod is on PVSQL01 (SQL Server 2008 R2).
For local dev use SQL Express and run the scripts in db\schema then db\seed
in name order (00_create_database.sql creates the DB and the pv_app login).

Connection string is in src\PV.Web\Web.config under "PVRef". Point it at
your local instance if it isn't localhost.


ARCHIVE
-------
The nightly batch copies custodian files off the SFTP drop to
\\PVFS01\swift\archive - see PV.Batch. The web app parses the whole archive
at startup; if files land during the day use Admin > Reload archive.

Layout:
    archive\mt535\MT535_<safe>_<yyyyMMdd>_<seq>.txt
    archive\mt54x\<yyyyMMdd>\MT54n_<safe>_<yyyyMMdd>_<seq>.txt

The app expects the archive under <site>\..\..\archive i.e. the archive\
folder at the repo root. appSettings "ArchiveRoot" in web.config.


LOGINS
------
Users are signed on automatically as the DefaultUser in web.config - access
is role based (ADVISER / OPS / ADMIN). Position Check, Message Browser and
Admin are not visible to advisers.


PROJECTS
--------
PV.Core   SWIFT parsing, archive index, position + settlement logic
PV.Data   SQL access (SqlHelper + per-table classes)
PV.Web    Web Forms front end
PV.Tests  NUnit 2.6.4 tests (run via build.bat or the NUnit runners)


KNOWN ISSUES
------------
- First page after an app pool recycle is slow while the archive loads.
- New ISINs don't show names until the app is restarted.
- Message Browser date box wants dd/MM/yyyy, Holdings uses the archive date.
