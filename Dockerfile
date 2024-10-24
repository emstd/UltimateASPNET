FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /home/app
COPY ./*.sln ./
COPY ./*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ./${file%.*}/ && mv $file ./${file%.*}/; done
RUN dotnet restore
COPY . .
RUN dotnet test ./Tests/Tests.csproj
RUN dotnet publish ./UltimateASPNET/UltimateASPNET.csproj -o /publish
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /publish
COPY --from=build /publish ./
ENV ASPNETCORE_URLS=https://+:5001;http://+:5000
ENTRYPOINT [ "dotnet", "UltimateASPNET.dll" ]