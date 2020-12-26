<?php

namespace App\API;

use App\Config\Config;
use Exception;
use Symfony\Component\HttpClient\HttpClient;

class AutoModerationEventAPI
{
    public static function Select($COOKIES, $guildid): Response
    {
        try {
            $client = HttpClient::create();
            $url = Config::getAPIBaseURL() . '/api/v1/guilds/' . $guildid . '/automoderations';
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
