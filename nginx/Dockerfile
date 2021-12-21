FROM node:14.15 AS compile-frontend
WORKDIR /angular

COPY MASZ/ .

RUN npm install -g @angular/cli
RUN npm install

RUN ng build --configuration production --output-hashing=all

FROM nginx:alpine

RUN rm -rf /usr/share/nginx/html/*

COPY static/ /var/www/data/static/
COPY nginx.conf /etc/nginx/nginx.conf
COPY --from=compile-frontend /angular/dist/masz/ /var/www/data/

# Set timezone
ENV TZ=Europe/Berlin
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

CMD ["nginx", "-g", "daemon off;"]

EXPOSE 80
