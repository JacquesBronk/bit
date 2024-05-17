FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the configuration file
COPY ["Bit/Bit.csproj", "Bit/"]
COPY ["Bit.Lib/Bit.Lib.csproj", "Bit.Lib/"]
COPY ["Bit.Log.Lib/Bit.Log.Lib.csproj", "Bit.Log.Lib/"]

# Restore dependencies
RUN dotnet restore "Bit/Bit.csproj"

# Copy the entire source tree
COPY . .

WORKDIR "/src/Bit"
# Build the project
RUN dotnet build "Bit.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
# Publish the project
RUN dotnet publish "Bit.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Copy the published files from the build stage
COPY --from=publish /app/publish .

# Ensure the configuration file is copied to the right place
COPY ["Bit/bit.ini", "./conf/"]

ENTRYPOINT ["dotnet", "Bit.dll"]
