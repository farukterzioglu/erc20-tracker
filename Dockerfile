FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /
COPY . .
WORKDIR /erc20-tracker
RUN dotnet publish -c Release -o /app

FROM microsoft/dotnet:2.2-aspnetcore-runtime AS runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "erc20-tracker.dll"]