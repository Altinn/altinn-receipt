# base imabrew install corepackge
FROM node:alpine3.24@sha256:a0b9bf06e4e6193cf7a0f58816cc935ff8c2a908f81e6f1a95432d679c54fbfd AS generate-receipt-app

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