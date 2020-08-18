FROM nginx:alpine

RUN rm -rf /usr/share/nginx/html/*
COPY ./nginx.conf /etc/nginx/

CMD ["nginx", "-g", "daemon off;"]

EXPOSE 80
