FROM node:20 AS compile-frontend
WORKDIR /svelte

COPY masz-svelte/package.json .
COPY masz-svelte/package-lock.json .

RUN npm install

COPY masz-svelte/ .

RUN npm run build

FROM nginx:alpine

RUN rm -rf /usr/share/nginx/html/*

COPY static/ /var/www/data/static/
COPY nginx.conf /etc/nginx/nginx.conf
COPY --from=compile-frontend /svelte/dist/ /var/www/data/

# Set timezone
ENV TZ=UTC
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

CMD ["nginx", "-g", "daemon off;"]

EXPOSE 80
