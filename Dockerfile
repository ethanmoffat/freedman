FROM mcr.microsoft.com/dotnet/runtime:6.0
COPY bin/Release/net6.0/publish/ /app
WORKDIR /app
ENTRYPOINT ["dotnet", "freedman.dll"]
