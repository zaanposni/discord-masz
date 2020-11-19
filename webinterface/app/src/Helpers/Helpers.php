<?php

namespace App\Helpers;

use App\API\DiscordAPI;
use App\Config\Config;
use Exception;
use Symfony\Component\HttpClient\HttpClient;

class Helpers
{
    public static function GetPatchNotes() {
        try {
            $client = HttpClient::create();
            $url = Config::getBaseURL() . '/static/patchnotes.json'; // change api url here
            $response = $client->request(
                'GET',
                $url
            );
            $patchnotes = $response->getContent();
            return json_decode($patchnotes, true);
        }
        catch (Exception $e) {
            return [];
        }
    }

    public static function GetCurrentUser($COOKIES) {
        $response = DiscordAPI::GetCurrentUser($COOKIES);
        if ($response->success) {
            if ($response->statuscode === 200) {
                return $response->body;
            }
        }

        return null;
    }
}