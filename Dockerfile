FROM node:24.18-alpine3.24@sha256:a0b9bf06e4e6193cf7a0f58816cc935ff8c2a908f81e6f1a95432d679c54fbfd AS build-receipt-frontend

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


FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine3.24@sha256:e602f99c22392a6782d7cd0bcb6e259bad2f29dc50efbdc1f5f1d72431894477 AS build
# Copy receipt backend
WORKDIR /Receipt/

COPY src/backend/Altinn.Receipt .

# Build and publish
RUN dotnet build Altinn.Platform.Receipt.csproj -c Release -o /app_output
RUN dotnet publish Altinn.Platform.Receipt.csproj -c Release -o /app_output


FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine3.24@sha256:0685cf40c9ba563c4ff9487f96ffbda3d7ee3ff43e80e313e31afc7f59996362 AS final
EXPOSE 5060
WORKDIR /app
COPY --from=build /app_output .
COPY --from=build-receipt-frontend /build/dist/receipt.js ./wwwroot/receipt/js/react/receipt.js
COPY --from=build-receipt-frontend /build/dist/receipt.css ./wwwroot/receipt/css/receipt.css

RUN apk upgrade --no-cache libcrypto3 libssl3

# setup the user and group
# the user will have no password, using shell /bin/false and using the group dotnet
RUN addgroup -g 3000 dotnet && adduser -u 1000 -G dotnet -D -s /bin/false dotnet
# update permissions of files if neccessary before becoming dotnet user
USER dotnet
RUN mkdir /tmp/logtelemetry

ENTRYPOINT ["dotnet", "Altinn.Platform.Receipt.dll"]
