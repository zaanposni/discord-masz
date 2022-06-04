FROM node:16 AS compile-frontend
WORKDIR /svelte

COPY masz-svelte/ .

RUN npm install
RUN npm run build

RUN apt update && apt install -y python3-pip
RUN python3 -m pip install rich
RUN python3 hashbuild.py

FROM nginx:alpine

RUN rm -rf /usr/share/nginx/html/*

COPY static/ /var/www/data/static/
COPY nginx.conf /etc/nginx/nginx.conf
COPY --from=compile-frontend /svelte/public/ /var/www/data/

# Set timezone
ENV TZ=UTC
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

CMD ["nginx", "-g", "daemon off;"]

EXPOSE 80
