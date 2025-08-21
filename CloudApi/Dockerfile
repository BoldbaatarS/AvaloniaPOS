# 1. Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5001

# 2. SDK / Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# ---- Restore cache-д csproj-уудыг тусад нь хуулна ----
COPY ./CloudApi/CloudApi.csproj ./CloudApi.csproj/
COPY ./Shared/Shared.csproj ./Shared/
COPY ./Shared.Protos/Shared.Protos.csproj ./Shared.Protos/
COPY ./Infrastructure.sqlite/Infrastructure.Sqlite.csproj ./Infrastructure.Sqlite/

RUN dotnet restore ./CloudApi.csproj

# ---- Бүх кодоо хуулж билдлэх ----
COPY . .
WORKDIR /src/CloudApi
RUN dotnet publish -c Release -o /app/out

# 3. Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "CloudApi.dll"]
