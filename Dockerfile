FROM node:24.18-alpine3.24@sha256:f70403e87646dc51b45295f4b8b70cdad0b63d2297c4c9899119b03f7af7a6b3 AS build-receipt-styles

WORKDIR /styles

COPY src/styles/package.json src/styles/package-lock.json src/styles/copy-styles.mjs ./

# Fetch the Designsystemet stylesheets and place them in the folder the Razor views serve them from
RUN npm ci --ignore-scripts
RUN node copy-styles.mjs /designsystemet


FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine3.24@sha256:edf9c86295a212261d9c74c5dfa24b35b186d8fb512f078795f42d66cf0d7878 AS build
# Copy receipt backend
WORKDIR /Receipt/

COPY src/backend/Altinn.Receipt .

# Publish
RUN dotnet publish Altinn.Platform.Receipt.csproj -c Release -o /app_output


FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine3.24@sha256:5dbc97def14ef05703726eb4ceaab4780700884c59b78f82dc43a8b0cc419fec AS final
EXPOSE 5060
WORKDIR /app
COPY --from=build /app_output .
COPY --from=build-receipt-styles /designsystemet ./wwwroot/receipt/css/designsystemet

# tzdata lets the receipt present timestamps in Norwegian local time
RUN apk upgrade --no-cache libcrypto3 libssl3 && apk add --no-cache tzdata

# setup the user and group
# the user will have no password, using shell /bin/false and using the group dotnet
RUN addgroup -g 3000 dotnet && adduser -u 1000 -G dotnet -D -s /bin/false dotnet
# update permissions of files if neccessary before becoming dotnet user
USER dotnet
RUN mkdir /tmp/logtelemetry

ENTRYPOINT ["dotnet", "Altinn.Platform.Receipt.dll"]
