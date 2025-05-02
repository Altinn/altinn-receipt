FROM node:22.15-alpine3.21@sha256:ad1aedbcc1b0575074a91ac146d6956476c1f9985994810e4ee02efd932a68fd AS build-receipt-frontend

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


FROM mcr.microsoft.com/dotnet/sdk:9.0.203-alpine3.21@sha256:33be1326b4a2602d08e145cf7e4a8db4b243db3cac3bdec42e91aef930656080 AS build

# Copy receipt backend
WORKDIR /Receipt/

COPY src/backend/Altinn.Receipt .

# Build and publish
RUN dotnet build Altinn.Platform.Receipt.csproj -c Release -o /app_output
RUN dotnet publish Altinn.Platform.Receipt.csproj -c Release -o /app_output


FROM mcr.microsoft.com/dotnet/aspnet:9.0.4-alpine3.21@sha256:3fce6771d84422e2396c77267865df61174a3e503c049f1fe242224c012fde65 AS final
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
