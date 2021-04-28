FROM maven:3.8.1-jdk-11

WORKDIR /app

COPY src/ .

RUN mvn install

ENTRYPOINT [ "mvn", "exec:java" ]
