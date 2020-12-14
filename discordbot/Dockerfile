FROM python:3.8

WORKDIR /app

COPY requirements.txt .

RUN python -m pip install -r requirements.txt

COPY src/ .

ENTRYPOINT [ "python", "-u", "main.py" ]
