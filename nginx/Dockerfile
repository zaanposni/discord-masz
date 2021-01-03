FROM node:14.15 AS compile-frontend
WORKDIR /angular

COPY frontend/ .

RUN npm install -g @angular/cli
RUN npm install

RUN ng build --prod


FROM bash as init
ARG DEPLOY_DOMAIN
ENV DEPLOY_DOMAIN $DEPLOY_DOMAIN
WORKDIR /build

COPY nginx-local.conf .
COPY nginx-prod.conf .
COPY build_config.sh .

# generates nginx.conf
RUN dos2unix build_config.sh
RUN ./build_config.sh


FROM nginx:alpine

RUN rm -rf /usr/share/nginx/html/*

COPY static/ /var/www/data/static/
COPY --from=init /build/nginx.conf /etc/nginx/nginx.conf
COPY --from=compile-frontend /angular/dist/masz/ /var/www/data/

# Set timezone
ENV TZ=Europe/Berlin
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

CMD ["nginx", "-g", "daemon off;"]

EXPOSE 80
