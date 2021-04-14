import requests

import pytest


def test_ping_frontend():
    r = requests.get("http://127.0.0.1:5565/")
    assert r.status_code == 200

def test_ping_backend():
    r = requests.get("http://127.0.0.1:5565/api/v1/status")
    assert r.status_code == 200

def test_ping_legal():
    r = requests.get("http://127.0.0.1:5565/legal.html")
    assert r.status_code == 200

def test_ping_favicon():
    r = requests.get("http://127.0.0.1:5565/favicon.ico")
    assert r.status_code == 200

def test_ping_robots_txt():
    r = requests.get("http://127.0.0.1:5565/robots.txt")
    assert r.status_code == 200

def test_ping_patchnotes():
    r = requests.get("http://127.0.0.1:5565/static/patchnotes.json")
    assert r.status_code == 200
    data = r.json()  # should not throw error
    assert isinstance(data, list)

def test_ping_version():
    r = requests.get("http://127.0.0.1:5565/static/version.json")
    assert r.status_code == 200
    data = r.json()  # should not throw error
    assert isinstance(data, dict)

def test_healthcheck_backend():
    headers = {
        "Accept": "application/json"
    }
    r = requests.get("http://127.0.0.1:5565/api/v1/status", headers=headers)
    assert r.status_code == 200
    json_value = r.json()
    assert str(json_value["status"]).lower() == "ok"
