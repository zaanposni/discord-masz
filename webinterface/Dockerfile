FROM php:7.4-apache


# Set timezone
ENV TZ=Europe/Berlin
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

RUN php -r "copy('https://getcomposer.org/installer', 'composer-setup.php');" \
    && php composer-setup.php --filename=composer \
    && php -r "unlink('composer-setup.php');" \
    && mv composer /usr/local/bin/composer \
    && chmod +x /usr/local/bin/composer \
    && apt-get update && apt-get install -y git libzip-dev unzip nodejs npm \
    && docker-php-ext-install zip \
    && a2enmod rewrite headers

COPY . /var/www/html

WORKDIR /var/www/html/app

RUN /usr/local/bin/composer require symfony/webpack-encore-bundle
RUN /usr/local/bin/composer require symfony/debug-bundle
RUN /usr/local/bin/composer install --no-dev --optimize-autoloader

RUN chmod -Rv 777 var/cache/ var/log/

EXPOSE 80
