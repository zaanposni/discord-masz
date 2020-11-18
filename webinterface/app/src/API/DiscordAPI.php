<?php

namespace App\API;

use App\Config\Config;
use Exception;
use Symfony\Component\HttpClient\HttpClient;

class DiscordAPI
{
    public static function GetCurrentUser($COOKIES): Response
    {
        try {
            $client = HttpClient::create();
            $url = Config::getAPIBaseURL() . '/api/v1/discord/users/@me';
            $response = $client->request(
                'GET',
                $url,
                [
                    'headers' => [
                        'Cookie' => 'masz_access_token=' . $COOKIES["masz_access_token"],
                        'Connection' => 'keep-alive'
                    ]
                ]
            );
            return new Response(true, $response->getStatusCode(), $response->getContent(false));
        } catch (Exception $e) {
            return new Response(false);
        }
    }

    public static function GetGuild($COOKIES, $guildid): Response
    {
        try {
            $client = HttpClient::create();
            $url = Config::getAPIBaseURL() . '/api/v1/discord/guilds/' . $guildid;
            $response = $client->request(
                'GET',
                $url,
                [
                    'headers' => [
                        'Cookie' => 'masz_access_token=' . $COOKIES["masz_access_token"],
                        'Connection' => 'keep-alive'
                    ]
                ]
            );
            return new Response(true, $response->getStatusCode(), $response->getContent(false));
        } catch (Exception $e) {
            return new Response(false);
        }
    }

    public static function GetUser($COOKIES, $userid): Response
    {
        try {
            $client = HttpClient::create();
            $url = Config::getAPIBaseURL() . '/api/v1/discord/users/' . $userid;
            $response = $client->request(
                'GET',
                $url,
                [
                    'headers' => [
                        'Cookie' => 'masz_access_token=' . $COOKIES["masz_access_token"],
                        'Connection' => 'keep-alive'
                    ]
                ]
            );
            return new Response(true, $response->getStatusCode(), $response->getContent(false));
        } catch (Exception $e) {
            return new Response(false);
        }
    }
}