<?php

namespace App\API;

use App\Config\Config;
use Exception;
use Symfony\Component\HttpClient\HttpClient;

class ModCaseAPI
{
    public static function Select($COOKIES, $guildid, $caseid): Response
    {
        try {
            $client = HttpClient::create();
            $url = Config::getAPIBaseURL() . '/api/v1/modcases/' . $guildid . '/' . $caseid;
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

    public static function SelectAll($COOKIES, $guildid): Response
    {
        try {
            $client = HttpClient::create();
            $url = Config::getAPIBaseURL() . '/api/v1/modcases/' . $guildid . '/all';
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

    public static function Post($COOKIES, $guildid, $data, $sendNotification, $handlePunishment): Response
    {
        try {
            $client = HttpClient::create();
            $url = Config::getAPIBaseURL() . '/api/v1/modcases/' . $guildid;
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
                    'body' => json_encode($data),
                    'query' => [
                        'sendNotification' => $sendNotification,
                        'handlePunishment' => $handlePunishment
                    ]
                ]
            );
            return new Response(true, $response->getStatusCode(), $response->getContent(false));
        } catch (Exception $e) {
            return new Response(false);
        }
    }

    public static function Update($COOKIES, $guildid, $caseid, $data, $sendNotification, $handlePunishment): Response
    {
        try {
            $client = HttpClient::create();
            $url = Config::getAPIBaseURL() . '/api/v1/modcases/' . $guildid . '/' . $caseid;
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
                    'body' => json_encode($data),
                    'query' => [
                        'sendNotification' => $sendNotification,
                        'handlePunishment' => $handlePunishment
                    ]
                ]
            );
            return new Response(true, $response->getStatusCode(), $response->getContent(false));
        } catch (Exception $e) {
            return new Response(false);
        }
    }

    public static function Delete($COOKIES, $guildid, $caseid, $sendNotification): Response
    {
        try {
            $client = HttpClient::create();
            $url = Config::getAPIBaseURL() . '/api/v1/modcases/' . $guildid . '/' . $caseid;
            $response = $client->request(
                'DELETE',
                $url,
                [
                    'headers' => [
                        'Cookie' => 'masz_access_token=' . $COOKIES["masz_access_token"],
                        'Connection' => 'keep-alive'
                    ],
                    'query' => [
                        'sendNotification' => $sendNotification
                    ]
                ]
            );
            return new Response(true, $response->getStatusCode(), $response->getContent(false));
        } catch (Exception $e) {
            return new Response(false);
        }
    }
}