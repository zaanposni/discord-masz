<?php

namespace App\API;

use App\Config\Config;
use Exception;
use Symfony\Component\HttpClient\HttpClient;

class GuildConfigAPI
{
    public static function Select($COOKIES, $guildid): Response
    {
        try {
            $client = HttpClient::create();
            $url = Config::getAPIBaseURL() . '/api/v1/configs/' . $guildid;
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

    public static function Post($COOKIES, $guildid, $data): Response
    {
        try {
            $client = HttpClient::create();
            $url = Config::getAPIBaseURL() . '/api/v1/configs/' . $guildid;
            $response = $client->request(
                'POST',
                $url,
                [
                    'headers' => [
                        'Cookie' => 'masz_access_token=' . $COOKIES["masz_access_token"],
                        'Content-Type' => 'application/json',
                        'Connection' => 'keep-alive',
                        'Content-Length' => strlen(json_encode($data))
                    ],
                    'body' => json_encode($data)
                ]
            );
            return new Response(true, $response->getStatusCode(), $response->getContent(false));
        } catch (Exception $e) {
            return new Response(false);
        }
    }

    public static function Update($COOKIES, $guildid, $data): Response
    {
        try {
            $client = HttpClient::create();
            $url = Config::getAPIBaseURL() . '/api/v1/configs/' . $guildid;
            $response = $client->request(
                'PUT',
                $url,
                [
                    'headers' => [
                        'Cookie' => 'masz_access_token=' . $COOKIES["masz_access_token"],
                        'Content-Type' => 'application/json',
                        'Connection' => 'keep-alive',
                        'Content-Length' => strlen(json_encode($data))
                    ],
                    'body' => json_encode($data)
                ]
            );
            return new Response(true, $response->getStatusCode(), $response->getContent(false));
        } catch (Exception $e) {
            return new Response(false);
        }
    }

    public static function Delete($COOKIES, $guildid): Response
    {
        try {
            $client = HttpClient::create();
            $url = Config::getAPIBaseURL() . '/api/v1/modcases/' . $guildid;
            $response = $client->request(
                'DELETE',
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