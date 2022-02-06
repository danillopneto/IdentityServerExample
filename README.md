# IdentityServerExample
Simple example of IdentityServer project.

# How to run the application

> `docker run --name sqlserver -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=DockerSql2019!" -e "MSSQL_PID=Developer" --cap-add SYS_PTRACE -p 1433:1433 -v sqlvolume:/var/opt/mssql -d mcr.microsoft.com/mssql/server:2019-latest`

# References
[Creating your First IdentityServer4 Solution](https://www.youtube.com/watch?v=HJQ2-sJURvA)

[How to add ASP.NET Identity and Entity Framework Support for your IdentityServer4 Solution](https://www.youtube.com/watch?v=Sw1rScI20xM)

[kevinrjones/SettingUpIdentityServer](https://github.com/kevinrjones/SettingUpIdentityServer/tree/master/step-by-step-demo/)
