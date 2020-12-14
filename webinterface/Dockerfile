FROM bash as init
ARG DEPLOY_MODE
ENV DEPLOY_MODE $DEPLOY_MODE
WORKDIR /build

COPY ./app/.env.dev .
COPY ./app/.env.prod .
COPY setup.sh .

# generates .env file
RUN dos2unix setup.sh
RUN ./setup.sh


FROM php:7.4-fpm

RUN apt-get update \
    && apt-get install -y --no-install-recommends vim curl debconf subversion git apt-transport-https apt-utils \
    build-essential locales acl mailutils wget zip unzip gnupg gnupg1 gnupg2

WORKDIR /home/wwwroot
COPY ./app/ ./sf4

COPY php.ini /etc/php/7.4/php.ini
COPY php-fpm-pool.conf /etc/php/7.4/pool.d/www.conf

COPY --from=init /build/.env ./sf4/.env

RUN curl -sSk https://getcomposer.org/installer | php -- --disable-tls && \
   mv composer.phar /usr/local/bin/composer

RUN rm -rf /var/lib/apt/lists/*
RUN echo "en_US.UTF-8 UTF-8" > /etc/locale.gen && \
    echo "fr_FR.UTF-8 UTF-8" >> /etc/locale.gen && \
    locale-gen

WORKDIR /home/wwwroot/sf4
RUN composer clear-cache
RUN composer install --no-dev --optimize-autoloader
WORKDIR /home/wwwroot

RUN mkdir -p /home/wwwroot/sf4/var/cache
RUN mkdir -p /home/wwwroot/sf4/var/log
RUN chmod -Rv 777 /home/wwwroot/sf4/var/cache/ /home/wwwroot/sf4/var/log/

EXPOSE 9000
CMD ["php-fpm"]
