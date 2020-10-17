<?php

namespace App\Helpers;

use Exception;
use Symfony\Component\HttpClient\HttpClient;

class Helpers
{
    public static function GetCurrentUser($COOKIES)
    {
        try {
            $client = HttpClient::create();
            $url = 'http://127.0.0.1:5565/api/v1/discord/users/@me'; // change api url here
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
}