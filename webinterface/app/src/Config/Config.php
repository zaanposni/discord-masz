<?php

namespace App\Config;

use Exception;
use Symfony\Component\HttpClient\HttpClient;

class Config
{
    public static function GetBaseUrl() {
        // return 'http://127.0.0.1:5565';  // for local development
        return 'http://masz_backend:80';  // for docker deployment
    }
}