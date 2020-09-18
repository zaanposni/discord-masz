import requests

import pytest


def test_ping_frontend():
    r = requests.get("http://127.0.0.1:5565/")
    assert r.status_code == 200

def test_ping_backend():
    r = requests.get("http://127.0.0.1:5565/api/v1/status")
    assert r.status_code == 200

def test_healthcheck_backend():
    headers = {
        "Accept": "application/json"
    }
    r = requests.get("http://127.0.0.1:5565/api/v1/status", headers=headers)
    assert r.status_code == 200
    json_value = r.json()
    assert str(json_value["status"]).lower() == "ok"
