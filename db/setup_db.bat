@echo off
rem SB 2018-05: stand up the reference DB in a container for dev/test.
rem Creates container "pvsql" if needed, waits for it, applies schema then seed.
setlocal

set IMG=mcr.microsoft.com/mssql/server:2022-latest
set NAME=pvsql
set SA_PASS=Pv!Local2018
set SQLCMD=/opt/mssql-tools18/bin/sqlcmd

docker inspect %NAME% >nul 2>&1
if errorlevel 1 (
    echo Creating container %NAME%...
    docker run -d --name %NAME% -e ACCEPT_EULA=Y -e MSSQL_PID=Express -e "MSSQL_SA_PASSWORD=%SA_PASS%" -p 1433:1433 %IMG%
) else (
    docker start %NAME% >nul 2>&1
)

echo Waiting for SQL Server...
:waitloop
docker exec %NAME% %SQLCMD% -S localhost -U sa -P "%SA_PASS%" -C -Q "select 1" >nul 2>&1
if errorlevel 1 (
    timeout /t 3 /nobreak >nul
    goto waitloop
)

pushd %~dp0
for %%f in (schema\*.sql) do (
    echo Applying %%f
    docker exec -i %NAME% %SQLCMD% -S localhost -U sa -P "%SA_PASS%" -C -b < "%%f"
    if errorlevel 1 ( popd & echo FAILED on %%f & exit /b 1 )
)
for %%f in (seed\*.sql) do (
    echo Applying %%f
    docker exec -i %NAME% %SQLCMD% -S localhost -U sa -P "%SA_PASS%" -C -b < "%%f"
    if errorlevel 1 ( popd & echo FAILED on %%f & exit /b 1 )
)
popd
echo Done.
