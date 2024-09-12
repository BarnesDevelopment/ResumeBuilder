FROM node:alpine AS builder

WORKDIR /usr/src/app

COPY ./App/resume-builder/ .

RUN npm ci && npm run build --prod

FROM trion/nginx-angular:latest AS final

EXPOSE 8080

COPY --from=builder /usr/src/app/dist/resume-builder/ /usr/share/nginx/html
