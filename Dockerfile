FROM node:22.21-alpine3.21@sha256:af8023ec879993821f6d5b21382ed915622a1b0f1cc03dbeb6804afaf01f8885 AS build-receipt-frontend

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


FROM mcr.microsoft.com/dotnet/sdk:9.0.315-alpine3.23@sha256:2412b9f7db4aa7ceac5a8c9d41385616cbd7bb8d18c298334cafad22f7647e04 AS build

# Copy receipt backend
WORKDIR /Receipt/

COPY src/backend/Altinn.Receipt .

# Build and publish
RUN dotnet build Altinn.Platform.Receipt.csproj -c Release -o /app_output
RUN dotnet publish Altinn.Platform.Receipt.csproj -c Release -o /app_output


FROM mcr.microsoft.com/dotnet/aspnet:9.0.17-alpine3.23@sha256:99a749b0dadd9e11d30d3804d94c8f1edb06db00148df52814219d5ff838f551 AS final
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
