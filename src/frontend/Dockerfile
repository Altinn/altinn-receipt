# base image
FROM node:alpine3.24@sha256:e88a35be04478413b7c71c455cd9865de9b9360e1f43456be5951032d7ac1a66 AS generate-receipt-app

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
RUN npm install --ignore-scripts -g -- corepack@0.35.0 && yarn --immutable

# Build runtime
RUN yarn workspace receipt-react-app run build; exit 0
CMD ["echo", "done"]