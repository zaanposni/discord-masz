<?php

namespace App\Helpers;

use App\Config\Config;
use Exception;
use Symfony\Component\HttpClient\HttpClient;

class Helpers
{
    public static function GetCurrentUser($COOKIES)
    {
        try {
            $client = HttpClient::create();
            $url = Config::getAPIBaseURL().'/api/v1/discord/users/@me'; // change api url here
            $response = $client->request(
                'GET',
                $url,
                [
                    'headers' => [
                        'Cookie' => 'masz_access_token=' . $COOKIES["masz_access_token"],
                    ],
                ]
            );
            $statusCode = $response->getStatusCode();

            if ($statusCode != 200) {
                return null;
            }

            $userInfoContent = $response->getContent();
            return json_decode($userInfoContent, true);
        }
        catch (Exception $e) {
            return null;
        }
    }

    public static function GetGuildById($guildid, $COOKIES) {
        try {
            $client = HttpClient::create();
            $url = Config::getAPIBaseURL() . '/api/v1/discord/guilds/' . $guildid; // change api url here
            $response = $client->request(
                'GET',
                $url,
                [
                    'headers' => [
                        'Cookie' => 'masz_access_token=' . $COOKIES["masz_access_token"],
                    ],
                ]
            );
            $guildContent = $response->getContent();
            return json_decode($guildContent, true);
        }
        catch (Exception $e) {
            return null;
        }
    }

    public static function GetGuildStatsById($guildid, $COOKIES) {
        try {
            $client = HttpClient::create();
            $url = Config::getAPIBaseURL() . '/api/v1/stats/' . $guildid; // change api url here
            $response = $client->request(
                'GET',
                $url,
                [
                    'headers' => [
                        'Cookie' => 'masz_access_token=' . $COOKIES["masz_access_token"],
                    ],
                ]
            );
            $guildContent = $response->getContent();
            return json_decode($guildContent, true);
        }
        catch (Exception $e) {
            return null;
        }
    }

    public static function GetNavbarStaticData() {
        $data = [];
        try {
            $client = HttpClient::create();
            $url = Config::getBaseURL() . '/static/patchnotes.json'; // change api url here
            $response = $client->request(
                'GET',
                $url
            );
            $patchnotes = $response->getContent();
            $data['patchnotes'] = json_decode($patchnotes, true);
        }
        catch (Exception $e) {
            return [];
        }
        return $data;
    }
}