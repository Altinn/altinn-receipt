# base image
FROM node:alpine3.16@sha256:f9b54b46639a9017b39eba677cf44c8cb96760ca69dadcc1d4cbd1daea753225 AS generate-receipt-app

WORKDIR /build

# Context is ./src, see docker-compose.yaml in src\Altinn.Platform\Altinn.Platform.Receipt\docker-compose.yml
COPY ./frontend/package.json .
COPY ./frontend/yarn.lock .
COPY ./frontend/.yarn/ ./.yarn/
COPY ./frontend/.yarnrc.yml .

# Copy shared and receipt code.
COPY ./frontend/shared/ ./shared/
COPY ./frontend/receipt/ ./receipt/

# Install
RUN corepack enable
RUN yarn --immutable

# Build runtime
RUN yarn workspace receipt-react-app run build; exit 0
CMD ["echo", "done"]