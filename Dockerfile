FROM node:22.20-alpine3.21@sha256:3bbc8aa1c0db220a63d9f42aa6e8117b305d72a6527039e99fced68c9ab96c8e AS build-receipt-frontend

WORKDIR /build

COPY src/frontend/package.json .
COPY src/frontend/yarn.lock .
COPY src/frontend/.yarn/ ./.yarn/
COPY src/frontend/.yarnrc.yml .

COPY src/frontend/ ./

# Install
RUN corepack enable
RUN yarn --immutable

# Build runtime
RUN yarn run build


FROM mcr.microsoft.com/dotnet/sdk:9.0.306-alpine3.22@sha256:f271ed7d0fd9c5a7ed0acafed8a2bc978bb65c19dcd2eeea0415adef142ffc87 AS build

# Copy receipt backend
WORKDIR /Receipt/

COPY src/backend/Altinn.Receipt .

# Build and publish
RUN dotnet build Altinn.Platform.Receipt.csproj -c Release -o /app_output
RUN dotnet publish Altinn.Platform.Receipt.csproj -c Release -o /app_output


FROM mcr.microsoft.com/dotnet/aspnet:9.0.10-alpine3.22@sha256:5e8dca92553951e42caed00f2568771b0620679f419a28b1335da366477d7f98 AS final
EXPOSE 5060
WORKDIR /app
COPY --from=build /app_output .
COPY --from=build-receipt-frontend /build/dist/receipt.js ./wwwroot/receipt/js/react/receipt.js
COPY --from=build-receipt-frontend /build/dist/receipt.css ./wwwroot/receipt/css/receipt.css

# setup the user and group
# the user will have no password, using shell /bin/false and using the group dotnet
RUN addgroup -g 3000 dotnet && adduser -u 1000 -G dotnet -D -s /bin/false dotnet
# update permissions of files if neccessary before becoming dotnet user
USER dotnet
RUN mkdir /tmp/logtelemetry

ENTRYPOINT ["dotnet", "Altinn.Platform.Receipt.dll"]
