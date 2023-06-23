FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env
WORKDIR /App

COPY . ./
RUN dotnet restore
RUN dotnet test ./Libro.Test
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /App
COPY --from=build-env /App/out .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "Libro.WebApi.dll"]
