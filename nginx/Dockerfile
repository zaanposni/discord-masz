FROM nginx:alpine

RUN rm -rf /usr/share/nginx/html/*

COPY static/ /var/www/data/static/
COPY ./nginx.conf /etc/nginx/

# Set timezone
ENV TZ=Europe/Berlin
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

CMD ["nginx", "-g", "daemon off;"]

EXPOSE 80
