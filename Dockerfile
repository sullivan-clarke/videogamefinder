FROM mcr.microsoft.com/dotnet/sdk:9.0 AS base

WORKDIR /app/VideoGameFinder

COPY . .

RUN dotnet restore
RUN dotnet publish -c Release -o out 

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS build
WORKDIR /src

COPY --from=base /app/VideoGameFinder/out ./

EXPOSE 8080


ENTRYPOINT ["dotnet", "VideoGameFinder.dll"] 