<?php


namespace App\Controller;


use Exception;
use Symfony\Component\HttpClient\HttpClient;
use Symfony\Component\Routing\Annotation\Route;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;


class IndexController extends AbstractController
{

    /**
     * @Route("/")
     */
    public function index() {
        if (!isset($_COOKIE["masz_access_token"])) {
            return $this->render('index.html.twig');
        }
         // not logged in
        try {
            $client = HttpClient::create();
            $url = 'http://127.0.0.1:5565/api/v1/discord/users/@me'; // change api url here
            $response = $client->request(
                'GET',
                $url,
                [
                    'headers' => [
                        'Cookie' => 'masz_access_token=' . $_COOKIE["masz_access_token"],
                    ],
                ]
            );
            $statusCode = $response->getStatusCode();

            if ($statusCode != 200) {
                return $this->render('index.html.twig', [
                    'error' => [
                        'messages' => ['Failed to login. Statuscode from API: '.$statusCode]
                    ]
                ]);
            }
        }
        catch (Exception $e) {
            return $this->render('index.html.twig', [
                'error' => [
                    'messages' => ['Failed to login.']
                ]
            ]);
        }

        try {
            $userInfoContent = $response->getContent();
            $userInfo = json_decode($userInfoContent, true);  // TODO: handle error
        } catch(Exception $e) {
            return $this->render('index.html.twig', [
                'error' => [
                    'messages' => ['Failed to fetch user info.']
                ]
            ]);
        }

        try {
            $guilds = array();
            foreach ($userInfo['guilds'] as &$guildid) {
                $url = 'http://127.0.0.1:5565/api/v1/discord/guilds/' . $guildid; // change api url here
                $response = $client->request(
                    'GET',
                    $url,
                    [
                        'headers' => [
                            'Cookie' => 'masz_access_token=' . $_COOKIE["masz_access_token"],
                        ],
                    ]
                );
                $guildContent = $response->getContent();
                $guild = json_decode($guildContent, true);  // TODO: handle error

                $url = 'http://127.0.0.1:5565/api/v1/stats/' . $guildid; // change api url here
                $response = $client->request(
                    'GET',
                    $url,
                    [
                        'headers' => [
                            'Cookie' => 'masz_access_token=' . $_COOKIE["masz_access_token"],
                        ],
                    ]
                );
                $statsContent = $response->getContent();
                $stats = json_decode($statsContent, true);  // TODO: handle error

                $guilds[] = [
                    'obj' => $guild,
                    'stats' => $stats
                ];
            }
            return $this->render('guilds/show.html.twig', [
                'guilds' => $guilds
            ]);
        } catch (Exception $e) {
            return $this->render('guilds/show.html.twig', [
                'guilds' => [],
                'error' => [
                    'messages' => ['Failed to fetch guild info.']
                ]
            ]);
        }



    }

}