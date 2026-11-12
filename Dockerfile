FROM node:24.22-alpine3.24@sha256:8a32c8cf649a49915a66e81e7a609223c708d0adabf288ee3f4077d765542f85 AS build-receipt-frontend

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


FROM mcr.microsoft.com/dotnet/sdk:10.0.404-resolute@sha256:f685dd6b5a970778b53cead9a11cf54206c4b4fc3c97e4424a59b8bff9aa43d7 AS build
# Copy receipt backend
WORKDIR /Receipt/

COPY src/backend/Altinn.Receipt .

# Build and publish
RUN dotnet publish Altinn.Platform.Receipt.csproj -c Release -o /app_output


FROM mcr.microsoft.com/dotnet/aspnet:10.0.15-resolute-chiseled@sha256:ee8204b710f5e15d563176ee354364e29043d8e7125d69390521265cfb93fd72 AS final
EXPOSE 5060
WORKDIR /app
COPY --from=build /app_output .
COPY --from=build-receipt-frontend /build/dist/receipt.js ./wwwroot/receipt/js/react/receipt.js
COPY --from=build-receipt-frontend /build/dist/receipt.css ./wwwroot/receipt/css/receipt.css
USER $APP_UID

ENTRYPOINT ["dotnet", "Altinn.Platform.Receipt.dll"]
