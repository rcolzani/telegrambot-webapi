FROM mcr.microsoft.com/dotnet/runtime:8.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ["Telegram.WebAPI/Telegram.WebAPI.csproj", "Telegram.WebAPI/"] 
COPY ["Telegram.WebAPI.Shared/Telegram.WebAPI.Shared.csproj",  "Telegram.WebAPI.Shared/"]
COPY ["Telegram.WebAPI.Data/Telegram.WebAPI.Data.csproj",  "Telegram.WebAPI.Data/"]
COPY ["Telegram.WebAPI.Domain/Telegram.WebAPI.Domain.csproj",  "Telegram.WebAPI.Domain/"]
COPY ["Telegram.WebAPI.Application/Telegram.WebAPI.Application.csproj",  "Telegram.WebAPI.Application/"]
RUN dotnet restore "Telegram.WebAPI/Telegram.WebAPI.csproj"

# Copy everything else and build
COPY . ./
RUN dotnet build -c Release -o out
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build-env /app/out .

#Necessário criar um usuário. No heroku não é possível usar o usuário root
RUN useradd -m myappusar
USER myappusar

#Heroku- necessário chamar com CMD não com ENTRYPOINT
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Telegram.WebAPI.dll