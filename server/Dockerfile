FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .

#Necessário criar um usuário. No heroku não é possível usar o usuário root
RUN useradd -m myappusar
USER myappusar

#Heroku- necessário chamar com CMD não com ENTRYPOINT
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Telegram.WebAPI.dll